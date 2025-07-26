/*=====
<DynamicMap.cs>

-author
	mizunose

-about
	動的マップ(自動生成するマップ)のデータを実装
=====*/

// 名前空間宣言
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UIElements;

// クラス定義
/// <summary>
/// 自動生成マップデータ
/// </summary>
[CreateAssetMenu(menuName = _MENU_TAB_NAME + _NAME, fileName = _NAME)]
class DynamicMap : MapData
{
	// 定数定義
	private const string _NAME = "DynamicMap";	// タブ名称
	private const Mass.TYPE _INITIAL_PACK = Mass.TYPE.WALL;	// 最初にエリアを仮埋めするマス種
	private const float _MASS_SIZE = 1.0f;	// 1マスあたりの大きさ
	private static readonly Vector3[] _MASS_VERTICES = {	// マスメッシュの頂点情報
			new Vector3(-_MASS_SIZE * 0.5f, 0.0f, _MASS_SIZE * 0.5f),	// 左上
			new Vector3(_MASS_SIZE * 0.5f, 0.0f, _MASS_SIZE * 0.5f),	// 右上
			new Vector3(-_MASS_SIZE * 0.5f, 0.0f, -_MASS_SIZE * 0.5f),	// 左下
			new Vector3(_MASS_SIZE * 0.5f, 0.0f, -_MASS_SIZE * 0.5f),	// 右下
		};
	private static readonly int[] _MASS_INDICES = {	// マスメッシュの頂点インデックス
			0, 1, 2,	// 左側三角形
			1, 3, 2,	// 右側三角形
		};
	private static readonly Vector2[] _MASS_UVS = {	// マスメッシュのテクスチャ座標
			new Vector2(0.0f, 0.0f),	// 左上
			new Vector2(1.0f, 0.0f),	// 右上
			new Vector2(0.0f, 1.0f),	// 左下
			new Vector2(1.0f, 1.0f),	// 右下
		};
	private const uint _RATIO_RAND_RANGE_MAX = 100;	// 空間分割の乱数幅

	// 変数宣言
	[Header("空間分割・部屋作成 ステータス")]
	[SerializeField, Tooltip("最低部屋作成数"), Min(1)]private uint _min_rooms = 1;
	[SerializeField, Tooltip("最小部屋サイズ(幅,　高さ)"), Min(3)]private Vector2Int _smallest_room = new Vector2Int(3, 3);	// 通路や削りの関係で2x2以下は動作不可
	[SerializeField, Tooltip("空間の分割数(横, 縦)"), Min(1)]private Vector2Int _area_split_base = new Vector2Int(3, 3);
	[SerializeField, Tooltip("空間の周囲のゆとり(壁)"), Min(0)]private int _area_margin = 1;	// ※0だと周囲に壁がない
	[SerializeField, Tooltip("部屋の周囲のゆとり"), Min(0)]private int _room_margin = 1;	// ※0だと部屋と接合する
	[SerializeField, Tooltip("空間分割の打ち切り率"), Range(0, _RATIO_RAND_RANGE_MAX)]private int _area_split_threshold = 2;
	[SerializeField, Tooltip("部屋掘削の打ち切り率"), Range(0, _RATIO_RAND_RANGE_MAX)]private int _room_sharpen_threshold = 2;
	[Header("通路 ステータス")]
	[SerializeField, Tooltip("幅"), Min(0)]private int _road_width = 1;


	/// <summary>
	/// <para>マップを作成</para>
	/// <para>
	/// 生成物は<br></br>
	/// ・部屋<br></br>
	/// ・通路<br></br>
	/// ・プレイヤー生成位置(スタート地点)<br></br>
	/// ・階段(ゴール地点)<br></br>
	/// ・ショップ(部屋設定)<br></br>
	/// </para>
	/// </summary>
	public override void Generate()
	{
		// 変数宣言
		Mass.TYPE[][] _area_infos = new Mass.TYPE[_size.y][];	// 生成管理用辞書
		bool[][] _road_infos = new bool[_size.y][];	// 通路候補マス(trueで通路にでき、falseで不可)

		// 初期化
		for(uint _y_idx = 0; _y_idx < _size.y; _y_idx++)	// 列単位でのループ
		{
			// 行を作成
			_area_infos[_y_idx] = new Mass.TYPE[_size.x];	// エリア用
			_road_infos[_y_idx] = new bool[_size.x];	// 通路用
			
			// パッキング
			for(uint _x_idx = 0; _x_idx < _size.x; _x_idx++)	// マス単位でのループ
			{
				_area_infos[_y_idx][_x_idx] = _INITIAL_PACK;	// 中身を仮置きする
				_road_infos[_y_idx][_x_idx] = false;	// すべて候補から外しておく
			}
		}

		// 変数宣言
		List<RectInt> _areas = new List<RectInt>();	// 部屋を作るスペース
		_areas.Add(new RectInt(0, 0, _size.x, _size.y ));	// エリア全体はすでに確保されている
		List<(RectInt room, RectInt parent_area)> _rooms = new List<(RectInt room, RectInt parent_area)>();	// 部屋
		Vector2Int _smallest_area = new Vector2Int(_smallest_room.x + _room_margin * 2 + _road_width
			, _smallest_room.y + _room_margin * 2 + _road_width);	// 各エリアに求められる最小限の容量	※最小ルーム, その周囲のゆとり(端の場合は壁), 通路(右端/下端 に設定するため1本分でOK)

		// 保全
		if(_size.x < _smallest_area.x || _size.y < _smallest_area.y)	// 部屋を格納できるスペースがない
		{
#if UNITY_EDITOR
			Debug.LogError("部屋を作成するために十分なスペースを確保してください");
#endif	// end UNITY_EDITOR
			return;	// 処理中断
		}
		else
		{
			// 変数宣言
			Vector2Int _max_split = Vector2Int.one;	// 各軸で最大何分割できるか

			// x方向の容量を算出
			for(float _split_interval_x = _size.x / _area_split_base.x; _split_interval_x >= _smallest_area.x; _split_interval_x /= _area_split_base.x)	// x方向の分割回単位でのループ
			{
				_max_split.x *= _area_split_base.x;	// 分割に成功
			}
			// y方向の容量を算出
			for(float _split_interval_y = _size.y / _area_split_base.y; _split_interval_y >= _smallest_area.y; _split_interval_y /= _area_split_base.y)	// y方向の分割回単位でのループ
			{
				_max_split.y *= _area_split_base.y;	// 分割に成功
			}

#if UNITY_EDITOR
			// 出力
			Debug.Log("現在のマップ設定では" + _max_split.x * _max_split.y + "部屋まで作成できます");	// 編集しやすいように何部屋までOKかを出力
#endif	// end UNITY_EDITOR
			
			// 保全
			if(_max_split.x * _max_split.y < _min_rooms)	// 部屋を格納しきれない
			{
#if UNITY_EDITOR
				Debug.LogError("部屋を作成しきれません。最低生成数を減らす, 最小サイズを縮める, マップを広げるなどの対策が必要です");
#endif	// end UNITY_EDITOR
				return;	// 処理中断
			}
		}

		// スペースを作成
		while(true)	// 分割処理のループ
		{			
			// 変数宣言
			int _target_idx = UnityEngine.Random.Range(0, _areas.Count);	// 分割の対象をランダムに選択
			int _target_axis = UnityEngine.Random.Range(0, 2);	// 分割軸の選択
			float _split_interval = 0.0f;	// 分割幅
			int _split_idx = UnityEngine.Random.Range(1, _area_split_base[_target_axis]);	// 分割番号

			// 分割幅の初期化
			switch(_target_axis)	// 選択された軸によって分岐
			{
				// 横
				case 0:
					_split_interval = _areas[_target_idx].width / _area_split_base.x;	// 横の分割幅
					break;	// 分岐処理完了

				// 縦
				case 1:
					_split_interval = _areas[_target_idx].height / _area_split_base.y;	// 横の分割幅
					break;	// 分岐処理完了

				// その他
				default:
#if UNITY_EDITOR
			Debug.LogError("非想定の軸が選択されました");
#endif	// end UNITY_EDITOR
					break;	// 分岐処理完了
			}

			// 分割終了
			if (_areas.Count >= _min_rooms)	// 最低数は生成済
			{
				if(_split_interval < _smallest_area[_target_axis])	// サイズが十分に設けられなかった
				{
					if (_split_interval < _smallest_area[1 ^ _target_axis])	// 分割軸を変えればサイズは足りる
					{
						_target_axis ^= 1;	// まだスペースがあるのでそれで進める
					}
					else
					{
					break;	// この先作るスペースがない可能性があるので打ち止めにする
					}
				}
				if(UnityEngine.Random.Range(0, _RATIO_RAND_RANGE_MAX) < _area_split_threshold)	// 閾値チェックに失敗
				{
					break;	// 終了条件を満たす
				}
			}
			
			// 保全
			if(_split_interval < _smallest_area[_target_axis])	// サイズが十分に設けられなかった
			{
				continue;	// 再抽選する
			}

			// 分割
				// 新規生成物
				RectInt _new_area = _areas[_target_idx];	// 分割で二つになるため、複製体を前もって用意しておく
				var _new_area_position = _new_area.position;	// 位置を変更するため構造体を取り出す(CS1612エラーの回避)
				_new_area_position[_target_axis] += (int)(_split_interval * _split_idx) + 1;	// 位置を調整	※分割幅の整数値は複製元の端として設定するため、こちらはさらに1ずらす
				_new_area.position = _new_area_position;	// 調整した位置を設定
				var _new_area_max = _new_area.max;	// サイズを変更するため構造体を取り出す(CS1612エラーの回避)
				_new_area_max[_target_axis] = _areas[_target_idx].max[_target_axis] ;	// サイズを調整(maxは閾値として扱うので実際には範囲外の値となる)
				_new_area.max = _new_area_max;	// 調整したサイズを設定
				_areas.Add(_new_area);	// 生成したエリア情報を登録
				
				// 分割による縮小
				var _base_area = _areas[_target_idx];	// エリア情報を変更するため構造体を取り出す(CS1612エラーの回避)
				var _base_area_max = _base_area.max;	// サイズを変更するため構造体を取り出す(CS1612エラーの回避)
				_base_area_max[_target_axis] = _new_area.min[_target_axis];	// 分割された分小さくなる
				_base_area.max = _base_area_max;	// 調整したサイズを設定
				_areas[_target_idx] = _base_area;	// 調整したエリア情報を登録
		}

		// 部屋を作成
		foreach (var _area in _areas)	// エリア単位でのループ
		{
			// 変数宣言
			RectInt _room = new RectInt();	// 部屋の情報
			Vector2Int _max_room_size_rand_range = _area.size - _smallest_area + _smallest_room;	// 部屋をおける範囲の大きさ

			// 部屋データ作成
			_room.size = _max_room_size_rand_range;
			//_room.size = new Vector2Int(UnityEngine.Random.Range(_smallest_room.x, _max_room_size_rand_range.x + 1),
			//	UnityEngine.Random.Range(_smallest_room.y, _max_room_size_rand_range.y + 1));	// 部屋のサイズを決定	※ここのRangeMaxは選択肢なので含まれる値
			_room.position = new Vector2Int(_area.xMin + UnityEngine.Random.Range(0, _max_room_size_rand_range.x - _room.width) + _room_margin,
				_area.yMin + UnityEngine.Random.Range(0, _max_room_size_rand_range.y - _room.height) + _room_margin);	// 部屋の位置を決定	※ここのRangeMaxは閾値なので含まれない値
			_rooms.Add((_room, _area));	// 作成した部屋の情報を登録

			// データから階層の情報を更新
			for(int _y_idx = _room.yMin; _y_idx < _room.yMax; _y_idx++)	// 列単位でのループ
			{
				for(int _x_idx = _room.xMin; _x_idx < _room.xMax; _x_idx++)	// マス単位でのループ
				{
					if(_y_idx >= _area_infos.Length || _x_idx >= _area_infos[_y_idx].Length)	// アクセスチェック
					{
#if UNITY_EDITOR
						Debug.LogError("フィールドに配置できないマスがあります");
#endif	// end UNITY_EDITOR
					}
					else
					{
						_area_infos[_y_idx][_x_idx] = Mass.TYPE.ROOM;	// マスを部屋に設定
					}
				}
			}

			// 変数宣言
			Vector2Int[] _room_corners = { _room.min, new Vector2Int(_room.xMin, _room.yMax - 1), new Vector2Int(_room.xMax - 1, _room.yMin), _room.max - Vector2Int.one};	// 部屋の角(左上, 左下, 右上,右下)
			List<(Vector2Int position, Vector2Int normal)> _sharpenables = new List<(Vector2Int position, Vector2Int normal)>();	// 削る角の候補
			
			// 初期化
			foreach (var _room_corner in _room_corners)	// 角単位でのループ
			{
				_sharpenables.Add((_room_corner, _room_corner.x == _room.xMin ? Vector2Int.left : Vector2Int.right));	// x方面外向きの角
				_sharpenables.Add((_room_corner, _room_corner.y == _room.yMin ? Vector2Int.down : Vector2Int.up));	// y方面外向きの角
			}

			// 部屋の角を削る
			while (_sharpenables.Count > 0)	// 削り出し単位でのループ
			{
				// 変数宣言
				int _sharpen_idx = UnityEngine.Random.Range(0, _sharpenables.Count);	// 削る角を選択
				int _sharpen_rand_range_max = 0;	// 掘削値の最大乱数

				// 掘削終了
				if (UnityEngine.Random.Range(0, _RATIO_RAND_RANGE_MAX) < _room_sharpen_threshold)	// 閾値チェックに失敗
				{
					break;	// 終了条件を満たす
				}

				// 初期化
				if (_sharpenables[_sharpen_idx].normal.x == 0)	// y方向に削る
				{
					_sharpen_rand_range_max = _room.height;	// 部屋の高さが削れる範囲
				}
				else if (_sharpenables[_sharpen_idx].normal.y == 0)	// x方向に削る
				{
					_sharpen_rand_range_max = _room.width;	// 部屋の幅が削れる範囲
				}
#if UNITY_EDITOR
				else	// 非想定
				{
					Debug.LogError("掘削値を求められませんでした");
				}
#endif	// end UNITY_EDITOR

				// 変数宣言
				int _sharpen_range = UnityEngine.Random.Range(0, _sharpen_rand_range_max);	// 掘削値決定

				// 掘削処理
				for (int _idx = 0; _idx < _sharpen_range; _idx++)	// 対象マス単位でのループ
				{
					// 変数宣言
					Vector2Int _sharpen_position = _sharpenables[_sharpen_idx].position - _sharpenables[_sharpen_idx].normal * _idx;	// 削るマスの位置	※入り込むので法線とは逆方向になる

					// 階層の情報を更新
					_area_infos[_sharpen_position.y][_sharpen_position.x] = _INITIAL_PACK;	// 部屋を削り元に戻す
				}
				_sharpenables.RemoveAt(_sharpen_idx);	// すでに削ったので候補から免除
			}

			// エリアと部屋の領域から通路の候補を演算
			if (_area.xMin == _area_margin && _area.xMin +_room_margin + _road_width < _room.xMin)	// 左隣にエリアがなく、エリアの左端に通路を設置しても必要なゆとりを保てる
			{
				for (int _idx = _area.yMin; _idx < _area.yMax; _idx++)	// エリア端のマス単位でのループ
				{
					_road_infos[_idx][_area.xMin] = true;	// 通路の候補にできる
				}
			}
			if(_area.yMin == _area_margin && _area.yMin + _room_margin + _road_width < _room.yMin)	// 上隣にエリアがなく、エリアの上端に通路を設置しても必要なゆとりを保てる
			{
				for (int _idx = _area.xMin; _idx < _area.xMax; _idx++)	// エリア端のマス単位でのループ
				{
					_road_infos[_area.yMin][_idx] = true;	// 通路の候補にできる
				}
			}
			for (int _idx = _area.yMin; _idx < _area.yMax; _idx++)	// エリア端のマス単位でのループ
			{
				_road_infos[_idx][_area.xMax - 1] = true;	// 空間・部屋設計時に_room_margin として予約済
			}
			for (int _idx = _area.xMin; _idx < _area.xMax; _idx++)	// エリア端のマス単位でのループ
			{
				_road_infos[_area.yMax - 1][_idx] = true;	// 空間・部屋設計時に_room_margin として予約済
			}
		}
		
		// 変数宣言
		List<Vector2Int> _road_points = new List<Vector2Int>();	// 通路の軌跡上に配置された接続待ち端子

		// 派生通路作成	※全エリアの通路候補地割り出しが終わってから処理
		foreach (var _room in _rooms)
		{
			// 変数宣言
			List<Edge> _edges = new List<Edge>{ Edge.Right, Edge.Bottom };	// 辺の選択肢

			// 初期化
			if (_room.parent_area.xMin > _area_margin || _room.parent_area.xMin - _room.room.xMin > _room_margin + _road_width)	// 左隣にエリアがあるか、エリアの左端に通路を設置しても必要なゆとりを保てる
			{
				_edges.Add(Edge.Left);	// 左辺を選択肢に含める
			}
			if (_room.parent_area.yMin > _area_margin || _room.parent_area.yMin - _room.room.yMin > _room_margin + _road_width)	// 上隣にエリアがあるか、エリアの上端に通路を設置しても必要なゆとりを保てる
			{
				_edges.Add(Edge.Top);	// 上辺を選択肢に含める
			}
			
			// 部屋の入り口を作成
			while (_edges.Count > 0)	// 辺単位でのループ
			{
				// 変数宣言
				int edge_idx = UnityEngine.Random.Range(0, _edges.Count);	// 辺を選択
				List<Vector2Int> _entryables = new List<Vector2Int>();	// 入口の候補
				List<Vector2Int> _lost_masses = new List<Vector2Int>();	// 削られた元部屋の部分

				// 辺の情報整理
				switch (_edges[edge_idx])	// 選択された辺によって分岐
				{
					// 左辺
					case Edge.Left:
						for(int _idx = _room.room.yMin + 1; _idx < _room.room.yMax - 1; _idx++)	// 辺のうち角以外のマス単位でのループ
						{
							if (_area_infos[_idx][_room.room.xMin] == Mass.TYPE.ROOM)	// 削り取られていないマス
							{
								_entryables.Add(new Vector2Int(_room.room.xMin, _idx));	// 入口の候補として優先される
							}
							else
							{
								_lost_masses.Add(new Vector2Int(_room.room.xMin, _idx));	// 削られているが元部屋として登録しておく
							}
						}
						break;	// 分岐処理完了

					// 右辺
					case Edge.Right:
						for(int _idx = _room.room.yMin + 1; _idx < _room.room.yMax - 1; _idx++)	// 辺のうち角以外のマス単位でのループ
						{
							if (_area_infos[_idx][_room.room.xMax - 1] == Mass.TYPE.ROOM)	// 削り取られていないマス
							{
								_entryables.Add(new Vector2Int(_room.room.xMax - 1, _idx));	// 入口の候補として優先される
							}
							else
							{
								_lost_masses.Add(new Vector2Int(_room.room.xMax - 1, _idx));	// 削られているが元部屋として登録しておく
							}
						}
						break;	// 分岐処理完了

					// 上辺
					case Edge.Top:
						for(int _idx = _room.room.xMin + 1; _idx < _room.room.xMax - 1; _idx++)	// 辺のうち角以外のマス単位でのループ
						{
							if (_area_infos[_room.room.yMin][_idx] == Mass.TYPE.ROOM)	// 削り取られていないマス
							{
								_entryables.Add(new Vector2Int(_idx, _room.room.yMin));	// 入口の候補として優先される
							}
							else
							{
								_lost_masses.Add(new Vector2Int(_idx, _room.room.yMin));	// 削られているが元部屋として登録しておく
							}
						}
						break;	// 分岐処理完了

					// 下辺
					case Edge.Bottom:
						for(int _idx = _room.room.xMin + 1; _idx < _room.room.xMax - 1; _idx++)	// 辺のうち角以外のマス単位でのループ
						{
							if (_area_infos[_room.room.yMax - 1][_idx] == Mass.TYPE.ROOM)	// 削り取られていないマス
							{
								_entryables.Add(new Vector2Int(_idx, _room.room.yMax - 1));	// 入口の候補として優先される
							}
							else
							{
								_lost_masses.Add(new Vector2Int(_idx, _room.room.yMax - 1));	// 削られているが元部屋として登録しておく
							}
						}
						break;	// 分岐処理完了

					// その他
					default:
#if UNITY_EDITOR
						Debug.LogError("非想定の辺が選択されました");
						break;	// 分岐処理完了
#endif	// end UNITY_EDITOR
				}
				
				// 変数宣言
				Vector2Int _entrance = new Vector2Int();	// 部屋の入り口	//TODO:リスト化

				// 入口を選択
				if(_entryables.Count > 0)	// 候補が存在
				{
					_entrance = _entryables[UnityEngine.Random.Range(0, _entryables.Count)];	// 入口を選択する
				}
				else if(_lost_masses.Count > 0)	// 壁で埋められ候補がない
				{
					_entrance = _lost_masses[UnityEngine.Random.Range(0, _lost_masses.Count)];	// 一か所ランダムで選択する
					_area_infos[_entrance.y][_entrance.x] = Mass.TYPE.ROOM;	// ここだけ部屋のままだったことにする
				}
#if UNITY_EDITOR
				else	// 領域自体がない
				{
					Debug.LogError("辺の取得に失敗しました");
				}
#endif	// end UNITY_EDITOR

				// 入口から法線方向に通路を進め、通路の構築軌道上に接続させる
				switch (_edges[edge_idx])	// 選択された辺によって分岐
				{
					// 左辺
					case Edge.Left:
						for (int _idx = _entrance.x - 1; _idx > _area_margin; _idx--)	// 入口につながる通路(部屋の外)のマス単位でのループ
						{
							// 階層の情報を更新
							_area_infos[_entrance.y][_idx] = Mass.TYPE.GROUND;	// マスを通路に設定

							// 終了
							if (_road_infos[_entrance.y][_idx])	// 通路の軌跡上に乗った
							{
								_road_points.Add(new Vector2Int(_idx, _entrance.y));	// 接続待ちに登録
								break;	// ループ処理完了
							}
						}
						break;	// 分岐処理完了

					// 右辺
					case Edge.Right:
						for (int _idx = _entrance.x + 1; _idx < _size.x; _idx++)	// 入口につながる通路(部屋の外)のマス単位でのループ
						{
							// 階層の情報を更新
							_area_infos[_entrance.y][_idx] = Mass.TYPE.GROUND;	// マスを通路に設定

							// 終了
							if (_road_infos[_entrance.y][_idx])	// 通路の軌跡上に乗った
							{
								_road_points.Add(new Vector2Int(_idx, _entrance.y));	// 接続待ちに登録
								break;	// ループ処理完了
							}
						}
						break;	// 分岐処理完了

					// 上辺
					case Edge.Top:
						for (int _idx = _entrance.y - 1; _idx > _area_margin; _idx--)	// 入口につながる通路(部屋の外)のマス単位でのループ
						{
							// 階層の情報を更新
							_area_infos[_idx][_entrance.x] = Mass.TYPE.GROUND;	// マスを通路に設定

							// 終了
							if (_road_infos[_idx][_entrance.x])	// 通路の軌跡上に乗った
							{
								_road_points.Add(new Vector2Int(_entrance.x, _idx));	// 接続待ちに登録
								break;	// ループ処理完了
							}
						}
						break;	// 分岐処理完了

					// 下辺
					case Edge.Bottom:
						for (int _idx = _entrance.y + 1; _idx < _size.y; _idx++)	// 入口につながる通路(部屋の外)のマス単位でのループ
						{
							// 階層の情報を更新
							_area_infos[_idx][_entrance.x] = Mass.TYPE.GROUND;	// マスを通路に設定

							// 終了
							if (_road_infos[_idx][_entrance.x])	// 通路の軌跡上に乗った
							{
								_road_points.Add(new Vector2Int(_entrance.x, _idx));	// 接続待ちに登録
								break;	// ループ処理完了
							}
						}
						break;	// 分岐処理完了

					// その他
					default:
#if UNITY_EDITOR
						Debug.LogError("法線方向の演算に失敗しました");
						break;	// 分岐処理完了
#endif	// end UNITY_EDITOR
				}

				// リスト更新
				_edges.RemoveAt(edge_idx);	// 選択済なので次回以降は免除

				// 終了
				//TODO:確率で打ち切り
			}
		}

		// 接続待ちの端子同士を繋げる
		while (_road_points.Count > 0)	// 接続待ちのものがなくなるまで	※Remove処理をするのでfor文でない
		{
			// 変数宣言
			Vector2Int _road_point = _road_points[UnityEngine.Random.Range(0, _road_points.Count)];	// 任意の端子
			List<bool[]> _search_map = new List<bool[]>();	// 通路の開拓用マップ

			// 配列コピー
			foreach (var _road_info in _road_infos)	// 列単位でコピー
			{
				// 変数宣言
				bool[] _line = new bool[_road_info.Length];	// マップ1行分のデータ格納用

				// コピー
				Array.Copy(_road_info, _line, _road_info.Length);	// 列の情報をコピー
				_search_map.Add(_line);	// コピー体を格納
			}

			// リスト更新
			_road_points.Remove(_road_point);	// この先この値が重複するのを防ぐため取り出した風に見せる

			// 接続チェック
			if (_road_points.Contains(_road_point))	// すでに端子と接続済
			{
				continue;	// 接続処理は必要ないので次の処理へ
			}

			// 変数宣言
			Vector2Int _searcher = _road_point;	// 探索位置
			List<(Vector2Int position, Vector2Int stride)> _junctions = new();	// 探索分岐点

			// 探索
			_junctions.Add((_road_point, Vector2Int.zero));	// 最初の地点の情報を登録
			while (true)	// 移動判断単位でのループ
			{
				// 変数宣言
				var _last_junction = _junctions[_junctions.Count - 1];	// 最後に経由した分岐点
				Vector2Int _moved_position = _searcher + _last_junction.stride;	// 移動先位置(シミュレーション)

				// 移動
				if(_last_junction.stride == Vector2.zero
					|| _moved_position.x < 0 || _moved_position.x >= _size.x || _moved_position.y < 0 || _moved_position.y >= _size.y
					|| !_search_map[_moved_position.y][_moved_position.x])	// 移動できない
				{
					// 変数宣言
					Vector2Int[] _directions = { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down};	// 移動方向の選択肢
					List<Vector2Int> _movable_directions = new();	// 実際に移動方向として選択できるもの
					
					// 移動方向のチェック
					foreach (var _direction in _directions)	// 候補単位でのループ
					{
						// 変数宣言
						_moved_position = _searcher + _direction;	// 移動先位置

						// 検査
						if (_moved_position.x >= 0 && _moved_position.x < _size.x && _moved_position.y >= 0 && _moved_position.y < _size.y
							&& _search_map[_moved_position.y][_moved_position.x])	// 移動できる方向
						{
							_movable_directions.Add(_direction);	// 選択可能とする
						}
					}

					// 行動判断
					if(_movable_directions.Count > 0)	// 移動の選択ができるならそれを優先する
					{
						// 変数宣言
						int _next_direction_idx = UnityEngine.Random.Range(0, _movable_directions.Count);	// ランダムに移動先を決定
						Vector2Int _next_direction = _movable_directions[_next_direction_idx];	// 決定した移動方向

						// リスト更新
						_junctions.Add((_searcher, _next_direction));	// 方向を変更したので分岐点として登録する
					}
					else	// 移動ができないなら戻って別の選択をする
					{
						// 順路更新
						_junctions.Remove(_last_junction);	// この分岐点は経由していないことにする

						// 戻る
						if (_junctions.Count > 0)	// まだ戻れる
						{
							_last_junction = _junctions[_junctions.Count - 1];	// 分岐点を戻す
							_searcher = _last_junction.position;	// 探索位置を戻す
						}
						else
						{
#if UNITY_EDITOR
							Debug.LogError("通路の作成に失敗しました");
#endif	// end UNITY_EDITOR
							break;	// 分岐処理完了
						}
					}
				}
				else	// 移動できるならば優先して行う
				{
					// 移動
					_searcher += _last_junction.stride;	// 安全性が確認されたので実際に移動する
					_search_map[_searcher.y][_searcher.x] = false;	// 探索済の経路はすでに通路の候補として破綻している

					// 終了
					if (_road_points.Contains(_searcher))	// 端子と接続
					{
						// リスト更新
						while (_road_points.Contains(_searcher))	// 重複を含め該当端子単位でループ
						{
							_road_points.Remove(_searcher);	// 対象端子は接続済として今後免除する
						}

						// 終了
						break;	// 接続完了
					}
					if (_area_infos[_searcher.y][_searcher.x] == Mass.TYPE.GROUND)	// 通路と接続
					{
						// 終了
						break;	// 接続完了
					}
				}
			}

			// 復元による確定処理
			if(_junctions.Count > 0)	// 順路が成立している
			{
				for (int _junction_idx = _junctions.Count - 1; _junction_idx > -1; _junction_idx--)	// 逆順の分岐点単位でループ
				{
					while(_searcher != _junctions[_junction_idx].position)	// 終点から始点までマス単位でのループ
					{
						// 進む
						_searcher -= _junctions[_junction_idx].stride;	// 逆方向に進めば戻ることになる

						// 階層の情報を更新
						_area_infos[_searcher.y][_searcher.x] = Mass.TYPE.GROUND;	// マスを通路に設定
					}
				}
			}
			else	// 順路が破綻している
			{
#if UNITY_EDITOR
				Debug.LogError("通路の生成に失敗しました");
#endif	// end UNITY_EDITOR
			}
		}





		// 商店作成

		// プレイヤー初期位置作成

		// 階段作成




		// マス作成
		for (int _y_idx = 0;  _y_idx < _area_infos.Length; _y_idx++)	// 列単位でのループ
		{
			for (int _x_idx = 0;  _x_idx < _area_infos[_y_idx].Length; _x_idx++)	// マス単位でのループ
			{
				// マスの生成
				switch (_area_infos[_y_idx][_x_idx])	// マスの種類によって分岐
				{
					// 通路
					case Mass.TYPE.GROUND:
						MakeMass(new Vector3(_x_idx, 0.0f,  _y_idx) * _MASS_SIZE);	// マス作成
						break;

					// 部屋
					case Mass.TYPE.ROOM:
						MakeMass(new Vector3(_x_idx, 0.0f,  _y_idx) * _MASS_SIZE);	// マス作成
						break;

					// 壁
					case Mass.TYPE.WALL:
						break;

					// その他
					default:
#if UNITY_EDITOR
						Debug.LogError("マスへの対応が定義されていません");
#endif	// end UNITY_EDITOR
						break;	// 分岐処理完了
				}
			}
		}

		// テクスチャ作成
		MapTexture = new Texture2D(_size.x, _size.y, TextureFormat.RGBA32, false);	// インスタンス作成
		MapTexture.filterMode = FilterMode.Point;	// ぼかさない(ドット表現)
		MapTexture.wrapMode = TextureWrapMode.Clamp;	// 繰り返さない

		// 変数宣言
		 Color[] pixels = new Color[_size.x * _size.y];	// カラーバッファ

		// カラーバッファ作成
		for (int _y_idx = 0; _y_idx < _size.y; _y_idx++)	// 列単位でのループ
		{
			for (int _x_idx = 0; _x_idx < _size.x; _x_idx++)	// マス単位でのループ
			{
				switch (_area_infos[_y_idx][_x_idx])	// マスの種類によって分岐
				{
					// 通路
					case Mass.TYPE.GROUND:
						pixels[_y_idx * _size.x + _x_idx] = new Color(0.6f, 0.95f, 0.9f, 1.0f);
						break;

					// 部屋
					case Mass.TYPE.ROOM:
						pixels[_y_idx * _size.x + _x_idx] = new Color(0.6f, 0.95f, 0.9f, 1.0f);
						break;

					// 壁
					case Mass.TYPE.WALL:
						pixels[_y_idx * _size.x + _x_idx] = Color.clear;
						break;
					// その他
					default:
#if UNITY_EDITOR
						Debug.LogError("マスへの対応が定義されていません");
#endif	// end UNITY_EDITOR
						break;	// 分岐処理完了
				}
			}
		}
		MapTexture.SetPixels(pixels);	// カラーバッファ登録
		MapTexture.Apply();	// 登録した情報を確定
	}


	/// <summary>
	/// <para>マスをインスタンスとして作成</para>
	/// </summary>
	/// <param name="position">生成位置</param>
	private void MakeMass(Vector3 position)
	{
		// 変数宣言
		Mesh _mesh = new Mesh();	//メッシュ本体

		// 作成
		GameObject _object = new GameObject();	// マスのインスタンス作成
		var _mesh_filter = _object.AddComponent<MeshFilter>();	// メッシュ管理機能追加
		_object.AddComponent<MeshRenderer>().material = _ground_texture;	// メッシュの描画機能を追加し、その参照マテリアルをマップに合わせて変更
		//TODO:Mass機能を作成、中身は必ず"壁"で初期化	_INITIAL_PACK

		// 初期化
		_object.transform.position = position;	// 生成位置を設定
		_mesh.vertices = _MASS_VERTICES;	// メッシュの頂点情報を設定
		_mesh.triangles = _MASS_INDICES;	// メッシュの頂点インデックスを設定
		_mesh.RecalculateNormals();	// 法線を再計算
		_mesh.uv = _MASS_UVS;	// テクスチャ座標を設定
		_mesh_filter.sharedMesh = _mesh;	// 作成したメッシュを登録
	}
}