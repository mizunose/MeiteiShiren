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
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
	private const uint _AREA_SPLIT_RAND_RANGE = 100;	// 空間分割の乱数幅

	// 変数宣言
	[Header("空間分割・部屋作成 ステータス")]
	[SerializeField, Tooltip("最低部屋作成数"), Min(1)]private uint _min_rooms = 1;
	[SerializeField, Tooltip("最小部屋サイズ(幅,　高さ)"), Min(1)]private Vector2Int _smallest_room = new Vector2Int(1, 1);
	[SerializeField, Tooltip("空間の分割数(横, 縦)"), Min(1)]private Vector2Int _area_split_base = new Vector2Int(3, 3);
	[SerializeField, Tooltip("部屋の周囲のゆとり"), Min(0)]private int _room_margin = 1;
	[SerializeField, Tooltip("空間分割の打ち切り率"), Range(0, _AREA_SPLIT_RAND_RANGE)]private int _area_split_threshold = 2;


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

		// 初期化
		for(uint _y_idx = 0; _y_idx < _size.y; _y_idx++)	// 列単位でのループ
		{
			_area_infos[_y_idx] = new Mass.TYPE[_size.x];	// 行を生成
			for(uint _x_idx = 0; _x_idx < _size.x; _x_idx++)	// マス単位でのループ
			{
				_area_infos[_y_idx][_x_idx] = _INITIAL_PACK;	// 中身を壁で仮置きする
			}
		}

		// 変数宣言
		List<RectInt> _areas = new List<RectInt>();	// 部屋を作るスペース
		_areas.Add(new RectInt(0, 0, _size.x, _size.y ));	// エリア全体はすでに確保されている
		List<RectInt> _rooms = new List<RectInt>();	// 部屋

		// 保全
		if(_size.x / _area_split_base.x < _smallest_room.x || _size.y / _area_split_base.y < _smallest_room.y)	// 分割後に部屋を割り振れない
		{
#if UNITY_EDITOR
			Debug.LogError("部屋を作成できませんでした。分割数を減らすか部屋の最小サイズを小さくしてください");
#endif	// end UNITY_EDITOR
			return;	// 処理中断
		}
		if(_size.x * _size.y < _smallest_room.x * _smallest_room.y * _min_rooms)	// 面積が不足している
		{
#if UNITY_EDITOR
			Debug.LogError("部屋を作成するスペースがありません");
#endif	// end UNITY_EDITOR
			return;	// 処理中断
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
				if(_split_interval < _smallest_room[_target_axis] + 2 * _room_margin)	// サイズが十分に設けられなかった
				{
					if (_split_interval < _smallest_room[1 ^ _target_axis] + 2 * _room_margin)	// 分割軸を変えればサイズは足りる
					{
						_target_axis ^= 1;	// まだスペースがあるのでそれで進める
					}
					else
					{
					break;	// この先作るスペースがない可能性があるので打ち止めにする
					}
				}
				if(UnityEngine.Random.Range(0, _AREA_SPLIT_RAND_RANGE) < _area_split_threshold)	// 閾値チェックに失敗
				{
					break;	// 終了条件を満たす
				}
			}
			
			// 保全
			if(_split_interval < _smallest_room[_target_axis] + 2 * _room_margin)	// サイズが十分に設けられなかった
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

			// 部屋データ作成
			_room.size = new Vector2Int(_area.width - 2 *_room_margin, _area.height- 2 *_room_margin);
			//_room.size = new Vector2Int(UnityEngine.Random.Range(_smallest_room.x, _area.width - 2 * _room_margin + 1),
			//	UnityEngine.Random.Range(_smallest_room.y, _area.height - 2 * _room_margin + 1));   // 部屋のサイズを決定	※ここのRangeMaxは選択肢なので含まれる値
			_room.position = new Vector2Int(_area.xMin + UnityEngine.Random.Range(0, _area.width - _room.width - 2 * _room_margin) + _room_margin,
				_area.yMin + UnityEngine.Random.Range(0, _area.height - _room.height - 2 * _room_margin) + _room_margin);	// 部屋の位置を決定	※ここのRangeMaxは閾値なので含まれない値
			_rooms.Add(_room);	// 作成した部屋の情報を登録

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
						_area_infos[_y_idx][_x_idx] = Mass.TYPE.GROUND;	// マスの情報を設定	※部屋は床と同じ
					}
				}
			}
		}

		// 通路作成



		// 商店作成



		// マス作成
		for (int _y_idx = 0;  _y_idx < _area_infos.Length; _y_idx++)	// 列単位でのループ
		{
			for (int _x_idx = 0;  _x_idx < _area_infos[_y_idx].Length; _x_idx++)	// マス単位でのループ
			{
				// マスの生成
				switch (_area_infos[_y_idx][_x_idx])	// マスの種類によって分岐
				{
					// 床
					case Mass.TYPE.GROUND:
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
					// 床
					case Mass.TYPE.GROUND:
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