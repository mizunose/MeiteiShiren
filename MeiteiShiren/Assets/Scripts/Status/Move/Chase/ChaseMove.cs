/*=====
<ChaseMove.cs>

-author
	mizunose

-about
	追跡移動を実装

-note
・追跡対象が見つかるまでは探索し、見つかったら見失うまで追跡を優先します。どちらにおいても攻撃できる(する意味がある)場合、移動しません。
=====*/

// 名前空間宣言
using System.Collections.Generic;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>入力移動</para>
/// </summary>
public class ChaseMove : Move
{
	// 定数定義
	private const int _SPLIT_DIRECTION = 8; // 移動の方向候補数

	// 変数宣言
	[SerializeField, Tooltip("データ")] private ChaseData _data;
	private Transform _chase_target;	// 追跡相手の姿勢
	private Transform _room_out;	// 室内で目指す出口


	/// <summary>
	/// <para>試算処理</para>
	/// </summary>
	/// <returns>試算結果</returns>
	public override (bool is_actionable, SimulatedData result) Simulate()
	{
		// 変数宣言
		SimulatedData _result = new();	// 演算結果格納用
		Mass _current_mass = GetCurrentMass();	// 現在のマス
		Vector2Int _current_mass_idx = Map.PositionToMass(_current_mass.transform.position);	// 現在マスの番号
		Room _room = _current_mass.transform.parent.GetComponent<Room>();	// 現在の部屋

		// 視界	//TODO:部屋にいるときは視界を部屋中に拡張
		if (_room)	// 室内
		{
			for (int _idx = 0; _idx < _room.transform.childCount; _idx++)	// 部屋の持つオブジェクト単位でのループ
			{
				// 追跡管理
				ViewCheckChase(_room.transform.GetChild(_idx).GetComponent<Mass>());	// 視界内の追跡対象を捉える
			}
		}
		else	// 室外
		{
			for (int _y_idx = _current_mass_idx.y - _data.ViewRange; _y_idx < _current_mass_idx.y + _data.ViewRange + 1; _y_idx++)	// 行単位でのループ
			{
				// 保全
				if (_y_idx < 0 || _y_idx >= Dungeon.Instance.FloorData.MapData.Masses.GetLength(0))	// y軸方向に見てマップ外のマス
				{
					continue;	// マスがないので処理できない
				}

				// 処理
				for (int _x_idx = _current_mass_idx.x - _data.ViewRange; _x_idx < _current_mass_idx.x + _data.ViewRange + 1; _x_idx++)	// マス単位でのループ
				{
					// 保全
					if (_x_idx < 0 || _x_idx >= Dungeon.Instance.FloorData.MapData.Masses.GetLength(1))	// x軸方向に見てマップ外のマス
					{
						continue;	// マスがないので処理できない
					}

					// 追跡管理
					ViewCheckChase(Dungeon.Instance.FloorData.MapData.Masses[_y_idx, _x_idx]);	// 視界内の追跡対象を捉える
				}
			}
		}


		// 変数宣言
		Attack _attack = GetComponent<Attack>();	// 攻撃機能

		// 状態遷移
		if (_attack && _attack.Simulate().AreThereAttackable)	// 攻撃できる場合はそれを最優先とする
		{
			// 終了
			return (false, _result);	// 攻撃できる位置から移動する必要がないため中断
		}
		else if (_chase_target)	// 追跡相手がいる場合はA*探索する
		{
			// 変数宣言
			Transform _goal = null;	// 追跡処理で実際に向かう場所

			// 初期化
			if (_attack)	// 攻撃機能がある
			{
				// 変数宣言
				var _goals_to_attack = _attack.CalculateAttackableMasses(false, _chase_target, Attack.AttackableAngles);	// 攻撃可能な場所一覧を取得

				// 最短攻撃可能マスを走査
				foreach (var _attackable in _goals_to_attack.attackables)	// 攻撃方向単位でのループ
				{
					foreach (var _target_mass_object in _attackable.results)	// 攻撃可能位置単位でのループ
					{
						// 変数宣言
						Mass _target_mass = _target_mass_object.GetComponent<Mass>();	// 扱うマス

						// 保全
						if (!_target_mass) // ヌルチェック
						{
							// 終了
							continue;	// マスのオブジェクトを取得できるはずなのでこれは異常
						}

						// 更新
						if (!_goal || Vector3.Distance(_current_mass.transform.position, _target_mass.transform.position) < Vector3.Distance(_current_mass.transform.position, _goal.GetComponent<Mass>().transform.position))	// 目標地点をより近くに更新できる
						{
							_goal = _target_mass_object.transform;	// 目標を更新
						}
					}
				}

				// 保全
				if (!_goal)	// ヌルチェック
				{
					_goal = _chase_target;	// 対象を代わりに目標とする
				}
			}
			else	// 対象へとまっすぐ目指す
			{
				_goal = _chase_target;	// 対象をそのまま目標とする
			}

			// 更新
			_result.next_mass = MoveOnMapWithAStar(_goal);	// 追跡探索

			// 終了
			if(!_result.next_mass)	// 探索不可能
			{
				_chase_target = null;	// 追跡出来なくなったので解放
				return (false, _result);	// 追跡できなくなった	※改めて他の移動処理をしようとするとまた同じ追跡対象を視認して無限ループする恐れあり
			}
			else if(_result.next_mass == transform)	// 移動していない
			{
				return (false, _result);	// 移動できるようになるまで待機する
			}
		}
		else if(_room)	// 室内なら通路を目指す
		{
			// 更新
			_result.next_mass = MoveOnRoom();	// 室内探索

			// 終了
			if(!_result.next_mass || _result.next_mass == transform)	// 異常値もしくは移動していない
			{
				return (false, _result);	// 処理しない
			}
		}
		else	// 通路なら直進を繰り返す
		{
			// 更新
			_result.next_mass = MoveOnLoad();	// 室内探索

			// 終了
			if(!_result.next_mass || _result.next_mass == transform)	// 異常値もしくは移動していない
			{
				return (false, _result);	// 処理しない
			}
		}

		// 変数宣言
		_result.direction = Vector3.Angle(Vector3.forward, _result.next_mass.transform.position - _current_mass.transform.position);	// 終了時点での向き

		// 補正
		if ((_result.next_mass.transform.position - _current_mass.transform.position).x < 0.0f)	// 取得できる角度は 0to180 なので反対方向は補正してあげる必要がある
		{
			_result.direction = _ROUND_DEGREE - _result.direction;	// 180to360の部分の角度として補正する
		}

		// 提供
		return (true, _result);	// 演算結果
	}


	/// <summary>
	/// <para>引数のマスから追跡対象を認識し、必要なら切り替える</para>
	/// </summary>
	/// <param name="_view_mass">視界にあるマス</param>
	private void ViewCheckChase(Mass _view_mass)
	{
		// 変数宣言
		Mass _current_mass = GetCurrentMass();	// 現在のマス

		// 追跡対象を認識
		if (_view_mass)	// ヌルチェック
		{
			for (int _object_idx = 0; _object_idx < _view_mass.transform.childCount; _object_idx++)	// マスが持つオブジェクト単位でのループ
			{
				// 変数宣言
				Camp _my_camp = GetComponent<Camp>();	// 自身の陣営
				GameObject _view_object = _view_mass.transform.GetChild(_object_idx).gameObject;	// 扱うオブジェクト
				Camp _view_camp = _view_object.GetComponent<Camp>();	// 対象オブジェクトの陣営

				// 追跡条件検査
				if(_view_camp && (!_my_camp || _my_camp.Type != _view_camp.Type))	// 違う陣営の所属	※無所属はアイテムなどの追跡しないオブジェクトとして考え、そもそも対象としない
				{
#if UNITY_EDITOR
					if (!_my_camp)	// 自身が無所属
					{
						Debug.LogError("所属陣営が不明なため無所属として処理します");
					}
#endif	// end UNITY_EDITOR

					// 更新
					if (!_chase_target	// 追跡対象が未設定か
						|| Vector3.Distance(_view_mass.transform.position, _current_mass.transform.position)
							< Vector3.Distance(_chase_target.position, _current_mass.transform.position))	// より近い追跡対象を見つけた時
					{
						_chase_target = _view_object.transform;	// 追跡対象を切り替える
					}
				}
			}
		}
	}



	/// <summary>
	/// <para>A*追跡処理</para>
	/// </summary>
	/// <param name="goal">追跡対象</param>
	/// <param name="is_ignore_actor">行動オブジェクトを無視して演算するか</param>
	/// <returns>最適な一手の移動先。現時点では移動できず待機する場合は現地点を、そもそも移動経路が塞がれてしまった場合にはnullを返す。</returns>
	private Transform MoveOnMapWithAStar(Transform goal, bool is_ignore_actor = true)
	{
		// 定数定義
		const int _SCORE_ERAGUER = -1;	// スコアの異常値(初期値)

		// 変数宣言
		Vector2Int _current_mass_idx = Map.PositionToMass(GetCurrentMass().transform.position);	// 現在マスの番号
		Vector2Int _goal_idx = Map.PositionToMass(goal.position);	// 目標地点のマス番号

		// 保全
		if (_goal_idx == _current_mass_idx)	// すでに目標地点にいる
		{
			return transform;	// 移動せずとも目標地点である
		}

		// 変数宣言
		(int score, List<Vector2Int> movables, Vector2Int shift_mass)[,] _nodes = new (int score, List<Vector2Int> movable, Vector2Int parent)[Dungeon.Instance.FloorData.MapData.Masses.GetLength(0), Dungeon.Instance.FloorData.MapData.Masses.GetLength(1)];	// A*処理用マス探索情報

		// 初期化
		for (int _y_idx = 0; _y_idx < _nodes.GetLength(0); _y_idx++)	// 行単位でのループ
		{
			for (int _x_idx = 0; _x_idx < _nodes.GetLength(1); _x_idx++)	// マス単位でのループ
			{
				_nodes[_y_idx, _x_idx] = (_SCORE_ERAGUER, new(), Vector2Int.zero);	// 初期値設定
			}
		}

		// 変数宣言
		int _search_depth = 0;	// 探索開始時点からの相対深度
		Vector2Int _node_idx = _current_mass_idx;	// ノードのマス番号

		// 追跡経路探索
		while(true)	// マス単位でのループ
		{
			if (_nodes[_node_idx.y, _node_idx.x].movables.Count > 0)	// 移動先候補を所持
			{
				// 変数宣言
				var _initial_idx = _nodes[_node_idx.y, _node_idx.x].movables[0];	// 移動先候補の初項
				(int value, List<Vector2Int> idxes) _min_score = new (_nodes[_initial_idx.y, _initial_idx.x].score, new());	// 最小スコア

				// 初期化
				foreach (var _moveable in _nodes[_node_idx.y, _node_idx.x].movables)	// 移動先候補ごとのループ
				{
					if (_min_score.value == _nodes[_moveable.y, _moveable.x].score)	// 現在最小スコアと同格
					{
						_min_score.idxes.Add(_moveable);	// 該当番号に登録
					}
					else if (_nodes[_moveable.y, _moveable.x].score < _min_score.value)	// 現在最少スコアを更新可能
					{
						_min_score.idxes.Clear();	// 最小スコアの更新に伴い該当番号をリセット
						_min_score.value = _nodes[_moveable.y, _moveable.x].score;	// 最少スコアを更新する
						_min_score.idxes.Add(_moveable);	// 最少スコアの該当番号に登録
					}
				}

				// 変数宣言
				Vector2Int _min_score_idx = _min_score.idxes[UnityEngine.Random.Range(0, _min_score.idxes.Count)];	// 最少スコアのノードをランダムに選択

				// 探索ノード更新
				_nodes[_node_idx.y, _node_idx.x].movables.Remove(_min_score_idx);	// 選択したノード番号が次から移動の選択肢に現れないよう、除去する
				_node_idx = _min_score_idx;	// 選択ノードを探索ノードに設定
				if (_node_idx == _goal_idx)	// 目標地点に到達
				{
					break;	// 探索終了
				}
				else
				{
					_search_depth++;	// 深度更新
				}
			}
			else	// 移動先候補を未所持
			{
				// 移動先候補を算出する
				for (int _y_idx = _node_idx.y - 1; _y_idx < _node_idx.y + 1 + 1; _y_idx++)	// 行単位でのループ
				{
					for (int _x_idx = _node_idx.x - 1; _x_idx < _node_idx.x + 1 + 1; _x_idx++)	// マス単位でのループ
					{
						// 変数宣言
						Vector2Int _moved_idx = new(_x_idx, _y_idx);	// 移動先のマス番号

						// 保全
						if (Mathf.Min(_x_idx, _y_idx) < 0 || _x_idx > _nodes.GetLength(1) || _y_idx > _nodes.GetLength(0))	// マップ外のマス
						{
							continue;	// 移動不可能
						}

						// 変数宣言
						Mass _node_mass = Dungeon.Instance.FloorData.MapData.Masses[_y_idx, _x_idx];	// 移動先マス

						// 保全
						if (_node_mass == null)	// 移動先がない
						{
							continue;	// 移動不可能
						}

						// 検査
						if (!is_ignore_actor)	// オブジェクトがある場合移動しない
						{
							// 終了
							if (!IsMovable(_node_mass))	// 移動できない
							{
								continue;	// 経路が成り立たないため候補に含めない
							}
						}
						else if (_moved_idx != _goal_idx)	// 行動オブジェクトはいずれ空くマスと見做し、経路に含める
						{
							// 変数宣言
							bool _is_movable_without_actor = true;	// 行動オブジェクトを無視した場合移動できるか

							// 移動可否検査
							for (int _idx = 0; _idx < _node_mass.transform.childCount; _idx++)	// マスの持つオブジェクト単位でのループ
							{
								if (!_node_mass.transform.GetChild(_idx).GetComponent<Camp>())	// アクターでない=障害物
								{
									// 終了
									_is_movable_without_actor = false;	// 移動できない
									break;	// 移動できないことが分かったのでこれ以上の演算は不要
								}
							}

							// 終了
							if (!_is_movable_without_actor)	// 移動できない
							{
								continue;	// 経路が成り立たないため候補に含めない
							}
						}

						// 移動先のノード演算
						if (_nodes[_y_idx, _x_idx].score == _SCORE_ERAGUER)	// 未開拓マス
						{
							// 変数宣言
							Vector2Int _shift = _moved_idx - _node_idx;	// 移動量

							// 更新
							_nodes[_y_idx, _x_idx].score = _search_depth + Mathf.Max(Mathf.Abs(_goal_idx.x - _x_idx), Mathf.Abs(_goal_idx.y - _y_idx));	// スコア値算出
							_nodes[_y_idx, _x_idx].shift_mass = _shift;	// 移動量設定

							// リスト更新
							_nodes[_node_idx.y, _node_idx.x].movables.Add(_moved_idx);	// 移動先候補に設定
						}
					}
				}

				// 終了
				if (_nodes[_node_idx.y, _node_idx.x].movables.Count == 0)	// 経路を構築できない
				{
					// 1つ戻り経路を選択しなおす
					_node_idx -= _nodes[_node_idx.y, _node_idx.x].shift_mass;	// 移動前に戻る
					_search_depth--;	// 1つ戻る
					if (_search_depth < 0)	// 探索開始時点から経路が成立しない
					{
						// 提供
						return null;	// 追跡不可能
					}
				}
			}
		}

		// 変数宣言
		List<Mass> _route = new List<Mass>();	// 経路格納用

		// 経路構築
		while (_nodes[_node_idx.y, _node_idx.x].shift_mass != Vector2Int.zero)	// 経路マス単位でのループ
		{
			// リスト更新
			_route.Insert(0, Dungeon.Instance.FloorData.MapData.Masses[_node_idx.y, _node_idx.x]);	// 経路に登録
		
			// 変数宣言
			Vector2Int _next_idx = _node_idx - _nodes[_node_idx.y, _node_idx.x].shift_mass;	// 次の経路マス番号を算出

			// 次ループ情報の設定
			if (_next_idx == _current_mass_idx)	// 経路の開始地点まで戻った
			{
				break;	// 経路構築完了
			}
			else
			{
				_node_idx = _next_idx;	// 次経路を指す
			}
		}

		// 経路に沿って移動先を提供
		if (IsMovable(_route[0]))	// 実際の移動に影響はない
		{
			// 提供
			return _route[0].transform;	// 移動先提供
		}
		else	// 実際に移動できるわけではない
		{
			// 経路上の到達可能マスまで迂回する
			for (int _idx = 1; _idx < _route.Count; _idx++)	// 経路上マス単位でのループ
			{
				// 迂回演算
				if (IsMovable(_route[_idx]))	// 経路上の空きを発見
				{
					// 変数宣言
					var _turned_next_mass = MoveOnMapWithAStar(_route[_idx].transform, false);	// 経路上の空きを一旦の目標とし、最適な迂回路演算のもと移動先を抽出

					// 提供
					if (_turned_next_mass)	// 迂回に成功
					{
						return _route[_idx].transform;	// 迂回路上の移動先
					}
				}
			}

			// 提供
			return transform;	// 待機するしかない
		}
	}


	/// <summary>
	/// <para>部屋探索処理</para>
	/// </summary>
	/// <returns>移動先</returns>
	private Transform MoveOnRoom()
	{
		// 変数宣言
		Mass _current_mass = GetCurrentMass();	// 現在マス

		// 目標地点を設定
		if (!_room_out)	// ヌルチェック
		{
			// 変数宣言
			var _room = _current_mass.transform.parent.GetComponent<Room>();	// 部屋
			List<Transform> _gateways = new();	// 出口一覧

			// 初期化
			for (int _idx = 0; _idx < _room.transform.childCount; _idx++)	// 部屋の持つオブジェクト単位でのループ
			{
				// 変数宣言
				var _mass = _room.transform.GetChild(_idx).GetComponent<Mass>();	// 管理マス

				// 保全
				if (!_mass) // ヌルチェック
				{
					continue;	// マスではないので扱わない
				}

				// 変数宣言
				Vector2Int _arround_mass_idx = Map.PositionToMass(_mass.transform.position);	// 扱うマスの番号

				// 周囲マスを参照し、扱っているマスが出口か判定
				for (int _y_idx = _arround_mass_idx.y - 1; _y_idx < _arround_mass_idx.y + 1 + 1; _y_idx++)	// 行単位でのループ
				{
					// 保全
					if (_y_idx < 0 || _y_idx >= Dungeon.Instance.FloorData.MapData.Masses.GetLength(0))	// y軸方向に見てマップ外のマス
					{
						continue;	// マスがないので処理できない
					}

					// 走査
					for (int _x_idx = _arround_mass_idx.x - 1; _x_idx < _arround_mass_idx.x + 1 + 1; _x_idx++)	// マス単位でのループ
					{
						// 保全
						if (_x_idx < 0 || _x_idx >= Dungeon.Instance.FloorData.MapData.Masses.GetLength(1))	// x軸方向に見てマップ外のマス
						{
							continue;	// マスがないので処理できない
						}

						// 変数宣言
						Mass _arround_mass = Dungeon.Instance.FloorData.MapData.Masses[_y_idx, _x_idx];	// マス番号からマス本体を取得

						// 検査
						if (_arround_mass && _arround_mass.transform.parent != _room.transform)	// 室外のマス
						{
							// 変数宣言
							bool _is_throughable = true;	// 出口として機能しているか

							// 通過性の検査
							for (int _object_idx = 0; _object_idx < _arround_mass.transform.childCount; _object_idx++)	// マスの持つオブジェクト単位でのループ
							{
								if (!_arround_mass.transform.GetChild(_object_idx).GetComponent<Camp>())	// アクターでない=障害物
								{
									_is_throughable = false;	// 通れない
									break;	// 通過性の検査は完了したためこれ以上の処理は不要
								}
							}

							// リスト更新
							if (_is_throughable)	// 通過可能
							{
								_gateways.Add(_arround_mass.transform);	// 出口として登録
							}
						}
					}
				}
			}

			// 保全
			if ( _gateways.Count == 0)	// 出口がない
			{
				// 終了
				return null;	// 移動先を求められない
			}

			// 目標地点を選択
			_room_out = _gateways[UnityEngine.Random.Range(0, _gateways.Count)].transform;	// 出口をランダムに目標とする
		}
		
		// 変数宣言
		var _result = MoveOnMapWithAStar(_room_out);	// 目標地点へ向かい探索する

		// 保全
		if (_result == null || _result.transform == _room_out)	// 経路構築に失敗、もしくは移動完了
		{
			_room_out = null;	// 目標地点を放棄する(新規目標は次ターンに設定し、このターンは動かない)
		}

		// 提供
		return _result;	// 演算結果
	}


	/// <summary>
	/// <para>通路探索処理</para>
	/// </summary>
	/// <returns>移動先</returns>
	private Transform MoveOnLoad()
	{
		// 変数宣言
		Mass _current_mass = GetCurrentMass();	// 現在マス
		Vector2 _shift = CalculateMoveDirection();	// 進行方向
		Mass _next_mass = CalculateMovedMass(_shift);	// 進行先のマス

		// 探索
		if (_next_mass == _current_mass)	// 移動に失敗
		{
			// 変数宣言
			List<Vector2> _next_directions = new();	// 進行方向と逸れる移動方向

			// 移動方向の候補取得
			if (_shift.x == 0.0f && _shift.y != 0.0f)	// y軸に沿って演算していた
			{
				_next_directions.Add(Vector2.left);	// -x方向
				_next_directions.Add(Vector2.right);	// +x方向
			}
			else if(_shift.x != 0.0f && _shift.y == 0.0f)	// x軸に沿って演算していた
			{
				_next_directions.Add(Vector2.up);	// +y方向
				_next_directions.Add(Vector2.down);	// -y方向
			}

			// 移動方向の選定
			while (_next_directions.Count > 0)	// 新規移動方向単位でのループ
			{
				// 変数宣言
				int _next_direction_idx = UnityEngine.Random.Range(0, _next_directions.Count);	// ランダムに選択した移動方向の番号
				Vector2 _next_direction = _next_directions[_next_direction_idx];	// 選択された移動方向
				
				// リスト更新
				_next_directions.RemoveAt(_next_direction_idx);	// 選択されたものは次からの抽選で現れないようにする

				// 移動可否検査
				_next_mass = CalculateMovedMass(_next_direction);	// 移動結果
				if( _next_mass != _current_mass)	// 移動に成功
				{
					// 終了
					break;	// 移動方向を確定
				}
			}
			if (_next_mass == _current_mass)	// 行き止まり
			{
				// 戻る
				_next_mass = CalculateMovedMass(-_shift);	// 来歴と合わせると4象限を網羅しているため移動ロジックを定められない
			}
		}

		// 提供
		return _next_mass.transform;	// 移動先を確定
	}


	/// <summary>
	/// <para>自身の向きを算出する</para>
	/// </summary>
	/// <returns>正面を示すベクトル</returns>
	private Vector2 CalculateMoveDirection()
	{
		// 初期化
		switch ((int)((transform.eulerAngles.y + _ROUND_DEGREE / _SPLIT_DIRECTION / 2) / (_ROUND_DEGREE / _SPLIT_DIRECTION)))	// 攻撃方向によって分岐
		{
			// 0 / 8
			case 0:
				return Vector2.up;	// 上

			// 1 / 8
			case 1:
				return Vector2.one;	// 右上

			// 2 / 8
			case 2:
				return Vector2.right;	// 右

			// 3 / 8
			case 3:
				return new Vector2(1.0f, -1.0f);	// 右下

			// 4 / 8
			case 4:
				return Vector2.down;	// 下

			// 5 / 8
			case 5:
				return -Vector2.one;	// 左下

			// 6 / 8
			case 6:
				return Vector2.left;	// 左

			// 7 / 8
			case 7:
				return new Vector2(-1.0f, 1.0f);	// 左上

			// その他
			default:
#if UNITY_EDITOR
				Debug.LogError("移動方向に対応が定義されていません");
#endif	// end UNITY_EDITOR
				return Vector2.up;	// 仮データ
		}
	}


	/// <summary>
	/// <para>移動先マス算出</para>
	/// </summary>
	/// <param name="shift_mass">マス上の移動値</param>
	/// <returns>移動先のマス</returns>
	private Mass CalculateMovedMass(Vector2 shift_mass)
	{
		// 変数宣言
		Mass _current_mass = GetCurrentMass();	// 現在マス
		Vector2Int _current_mass_idx = Map.PositionToMass(_current_mass.transform.position);	// 現在マスの番号
		Vector3 _world_moved = _current_mass.transform.position;	// 移動先のワールド座標

		// 初期化
		_world_moved.x += shift_mass.x * Settings.Instance.Map.MassSize;	// 入力値のx射影に沿って移動
		_world_moved.z += shift_mass.y * Settings.Instance.Map.MassSize;	// 入力値のy射影に沿って移動

		// 変数宣言
		Vector2Int _next_mass_idx = Map.PositionToMass(_world_moved);	// 移動先のワールド座標から該当マスの番号を取得

		// 保全
		if (_next_mass_idx.x < 0 || _next_mass_idx.x >= Dungeon.Instance.FloorData.MapData.Masses.GetLength(1))	// x軸方向に見てマップ外のマス
		{
			_next_mass_idx.x = _current_mass_idx.x;	// 移動先として選択させない
		}
		if (_next_mass_idx.y < 0 ||_next_mass_idx.y >= Dungeon.Instance.FloorData.MapData.Masses.GetLength(0))	// y軸方向に見てマップ外のマス
		{
			_next_mass_idx.y = _current_mass_idx.y;	// 移動先として選択させない
		}

		// 変数宣言
		Mass _next_mass = Dungeon.Instance.FloorData.MapData.Masses[_next_mass_idx.y, _next_mass_idx.x];	// 移動先のマス番号からマス本体を取得

		// 移動可否検査
		if(!IsMovable(_next_mass))	// 移動不可能
		{
			if (shift_mass.x != 0.0f && shift_mass.y != 0.0f) // 斜め移動で演算していた
			{
				// 更新
				_next_mass = Dungeon.Instance.FloorData.MapData.Masses[_current_mass_idx.y, _next_mass_idx.x];	// x成分に沿った移動で再度試す

				// 検査
				if (!IsMovable(_next_mass))	// x方向にも移動できない
				{
					// 更新
					_next_mass = Dungeon.Instance.FloorData.MapData.Masses[_next_mass_idx.y, _current_mass_idx.x];	// y成分に沿った移動で再度試す

					// 検査
					if (!IsMovable(_next_mass))	// y方向にも移動できない
					{
						// 更新
						_next_mass = _current_mass;	// 移動出来ないので移動先を元のマスに戻しておく
					}
				}
			}
			else	// 軸に沿って演算していた
			{
				// 更新
				_next_mass = _current_mass;	// 移動出来ないので移動先を元のマスに戻しておく
			}
		}

		// 提供
		return _next_mass;	// 演算結果
	}
}