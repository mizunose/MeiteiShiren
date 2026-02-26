/*=====
<Attack.cs>

-author
	mizunose

-about
	攻撃を定義
=====*/

// 名前空間宣言
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>攻撃</para>
/// </summary>
[DisallowMultipleComponent]
public abstract class Attack : MonoBehaviour
{
	// 構造体定義
	/// <summary>
	/// <para>試算データ</para>
	/// </summary>
	public struct SimulatedData
	{
		// 変数宣言
		public List<(float angle, List<GameObject> results)> attackables;	// 攻撃に含める相手

		// プロパティ定義

		/// <value>有効な攻撃ならtrue, そうでなければfalse</value>
		public bool AreThereAttackable
		{
			get
			{
				// 変数宣言
				bool _result = false;	// 演算結果格納用

				// 検査
				foreach (var _data in attackables)	// 方向単位でのループ
				{
					if (_data.results.Count > 0)	// 攻撃対象を確認
					{
						_result = true;	// 攻撃対象を保証できる
						break;	// 目的を果たしたのでこれ以降のループは不要
					}
				}
			
				// 提供
				return _result;	// 演算結果
			}
		}
	}

	// 定数定義
	private static readonly SplitedDirections _attackable_directions = new EightDirections();	// 攻撃可能な方向

	// 変数宣言
	[SerializeField, Tooltip("データ")] private AttackData _data;

	// プロパティ定義

	/// <value>各攻撃方向の角度</value>
	public static float[] AttackableAngles => _attackable_directions.SplitedAngles;

	/// <returns>試算結果</returns>
	public abstract SimulatedData Simulate();

	/// <value>現在シーンがダンジョンならインスタンスを取得</value>
	protected Dungeon DungeonScene => SceneLoader.Instance.CurrentScene as Dungeon;


	/// <summary>
	/// <para>攻撃モーション処理</para>
	/// </summary>
	/// <param name="data">試算データ</param>
	/// <returns>遅延処理用のインターフェース</returns>
	public IEnumerator AttackMotion(SimulatedData data)
	{
		// 変数宣言
		List<GameObject> _targets = new();	// 攻撃対象一覧

		// 選択
		if (data.attackables.Count > 0)
		{
			data.attackables.Sort((first, second) => second.results.Count - first.results.Count);	// 対象の多い順に並べ替え

			// 変数宣言
			Move _move = GetComponent<Move>();	// 移動機能
			var _selected_data = data.attackables[0];	// 最も対象の多いものを選択

			// 回転処理
			if (_move && _selected_data.angle != transform.eulerAngles.y)	// 攻撃方向へ回転できる
			{
				yield return _move.MoveMotion(new Move.SimulatedData{next_mass = transform, direction = _selected_data.angle});	// 攻撃方向に回転
			}

			// 初期化
			_targets = _selected_data.results;	// 攻撃対象を設定
		}

		//TODO:モーション再生
		//if (PlayableGraph _playable_graph.IsPlaying) { yield return null; }	// モーション中は待機

		// 代替処理
			// 定数定義
			const float _MOTION_SPEND = 1.0f;	// モーションの再生時間
		
			// 変数宣言
			Vector3 _at = transform.forward.normalized;	// 到達地点
			Vector3 _from = Vector3.zero;	// 出発地点

			// 変数宣言
			float _time = 0.0f;	// 経過時間

			// 前隙モーションを取る
			while (true)	// フレーム単位でのループ
			{
				// 更新
				_time += Time.deltaTime;	// 経過時間を測定

				// 変数宣言
				float _timerate = _time / (_MOTION_SPEND * 0.5f);	// 経過時間の割合

				// 補正
				if(_timerate > 1.0f)	// 時間経過が過剰
				{
					_timerate = 1.0f;	// 割合を丸め込む
				}

				// 攻撃
				transform.localPosition = Vector3.Lerp(_from, _at,  - (Mathf.Cos(Mathf.PI * _timerate) - 1.0f ) /2.0f);	// イージング攻撃

				// 終了
				if (_timerate == 1.0f)	// モーション完了
				{
					break;	// 処理完了
				}
				else
				{
					// 待機
					yield return null;	// 次フレームを待つ
				}
			}

			// 内容処理
			DoAttack(_targets);	// 攻撃の効果発生タイミング

			// 初期化
			_time = 0.0f;	// 経過時間

			// 後隙モーションを取る
			while (true)	// フレーム単位でのループ
			{
				// 更新
				_time += Time.deltaTime;	// 経過時間を測定

				// 変数宣言
				float _timerate = _time / (_MOTION_SPEND * 0.5f);	// 経過時間の割合

				// 補正
				if(_timerate > 1.0f)	// 時間経過が過剰
				{
					_timerate = 1.0f;	// 割合を丸め込む
				}

				// 攻撃
				transform.localPosition = Vector3.Lerp(_at, _from,  - (Mathf.Cos(Mathf.PI * _timerate) - 1.0f ) /2.0f);	// イージング攻撃

				// 終了
				if (_timerate == 1.0f)	// モーション完了
				{
					break;	// 処理完了
				}
				else
				{
					// 待機
					yield return null;	// 次フレームを待つ
				}
			}
	}


	/// <summary>
	/// <para>攻撃モーションを取り終えた時の攻撃処理部分</para>
	/// </summary>
	/// <param name="targets">攻撃対象</param>
	protected void DoAttack(List<GameObject> targets)
	{
		// 効果発動
		foreach (GameObject target in targets)	// 発動対象ごとのループ
		{
			_data.Affects.BootAffects(gameObject, target);	// 設置物に効果発動
		}
	}


	/// <summary>
	/// <para>攻撃可能マス算出</para>
	/// </summary>
	/// <param name="activity">能動側の演算ならtrue, 受動側の演算ならfalse</param>
	/// <param name="base_transform">計算基準の姿勢情報</param>
	/// <param name="result_datas">算出した情報を格納する領域</param>
	public SimulatedData CalculateAttackableMasses(in bool activity, in Transform base_transform, in float[] angles)
	{
		// 変数宣言
		Mass _base_mass = null;	// 基準のマス
		SimulatedData _result;	// 演算結果格納用

		// 初期化
		_result.attackables = new();	// 領域確保
		if (base_transform.parent)	// ヌルチェック
		{
			_base_mass = base_transform.parent.GetComponent<Mass>();	// 基準マスの取得

#if UNITY_EDITOR
			// 保全
			if(!_base_mass)	// ヌルチェック
			{
				Debug.LogError("親がマスではありません");
			}
#endif	// end UNITY_EDITOR
		}
#if UNITY_EDITOR
		else
		{
			Debug.LogError("管理者がいない独立したオブジェクトなため、マスに所属していません");
		}
#endif	// end UNITY_EDITOR

		// 保全
		if (!_data)	// ヌルチェック
		{
#if UNITY_EDITOR
			Debug.LogError("攻撃のデータが設定されていません");
#endif	// end UNITY_EDITOR
			return _result;	// 処理不可能なため終了
		}

		// 変数宣言
		Vector2Int _base_idx = Map.PositionToMass(_base_mass.transform.position);	// 基準マスの番号
		List<((float angle, List<GameObject> targets) result_data, List<Mass> target_masses)> _target_datas = new();	// 方向別の効果対象一覧

		// 初期化
		switch (_data.Range.Type)	// 範囲の定義によって分岐
		{
			// 限定範囲
			case MassRange.RangeType.RANGED:
			{
				// 範囲登録
				foreach (var _angle in angles)	// 方向単位でのループ
				{
					// 変数宣言
					((float angle, List<GameObject> targets) result_data, List<Mass> target_masses) _target_data = ((_angle, new()), new());	// 範囲格納場所

					// 走査
					foreach (Vector2Int _shift in _data.Range.Range)	// マス単位でのループ
					{
						// 変数宣言
						Vector2Int _attack_direction = _attackable_directions.CalculateSplitedDirectionInt(_angle);	// 攻撃方向
						Vector2Int _target_idx = _base_idx + (activity ? 1 : -1) * new Vector2Int(
							_shift.x * _attack_direction.y + _shift.y * _attack_direction.x,
							_shift.y * _attack_direction.y - _shift.x * _attack_direction.x
							);	// 対象マスの番号	※鉛直上向きを基準に範囲を回転させている

						// 保全
						if (_target_idx.x < 0 || _target_idx.x >= DungeonScene.FloorData.MapData.Masses.GetLength(1)
							|| _target_idx.y < 0 || _target_idx.y >= DungeonScene.FloorData.MapData.Masses.GetLength(0))	// マップ外のマス
						{
							continue;	// マスとして処理できないので次の処理へ移る
						}

						// 変数宣言
						Mass _target_mass = DungeonScene.FloorData.MapData.Masses[_target_idx.y, _target_idx.x];	// 対象となるマス本体を取得

						// 保全
						if (!_target_mass)	// ヌルチェック
						{
							continue;	// マスが存在しないので次の処理へ移る
						}

						// リスト更新
						_target_data.target_masses.Add(_target_mass);	// 効果範囲に登録
					}

					// リスト更新
					_target_datas.Add(_target_data);	// 求めた対象を登録
				}

				// 終了
				break;	// 分岐処理完了
			}

			// 直線範囲
			case MassRange.RangeType.FRONT_LINE:
			{
				// 範囲登録
				foreach (var _angle in angles)	// 方向単位でのループ
				{
					// 変数宣言
					((float angle, List<GameObject> targets) result_data, List<Mass> target_masses) _target_data = ((_angle, new()), new());	// 範囲格納場所
					Vector2Int _target_idx = _base_idx;	// 対象マスの番号

					// 線上走査
					while (true)	// マス単位でのループ
					{
						// 初期化
						if (transform.parent)	// ヌルチェック
						{
							// 更新
							_target_idx += (activity ? 1 : -1) * _attackable_directions.CalculateSplitedDirectionInt(_angle);	// 移動して走査

							// 終了条件
							if (_target_idx.x < 0 || _target_idx.x >= DungeonScene.FloorData.MapData.Masses.GetLength(1)
								|| _target_idx.y < 0 || _target_idx.y >= DungeonScene.FloorData.MapData.Masses.GetLength(0))	// マップ外のマス
							{
								break;	// マップ外へ進出させない
							}

							// 変数宣言
							Mass _target_mass = DungeonScene.FloorData.MapData.Masses[_target_idx.y, _target_idx.x];	// 対象となるマス本体を取得

							// 終了条件
							if (!_target_mass)	// ヌルチェック
							{
								break;	// マスが存在しないので打ち止めにする
							}
							//TODO:壁で妨害されていて先に届かない

							// リスト更新
							_target_data.target_masses.Add(_target_mass);	// 効果範囲に登録
						}
					}

					// リスト更新
					_target_datas.Add(_target_data);	// 求めた対象を登録
				}

				// 終了
				break;	// 分岐処理完了
			}

			// 部屋全体
			case MassRange.RangeType.ROOM:
			{
				// 変数宣言
				((float angle, List<GameObject> targets) result_data, List<Mass> target_masses) _target_data = ((transform.eulerAngles.y, new()), new());	// 範囲格納場所
				Room _base_room = null;	// 基準位置のある部屋

				// 初期化
				if (_base_mass.transform.parent)	// ヌルチェック
				{
					_base_room = _base_mass.transform.parent.GetComponent<Room>();	// 基準マスから部屋を取得
				}

				// 処理分岐
				if (_base_room)	// ヌルチェック
				{
					// リスト更新
					foreach (Mass _target_mass in _base_room.GetComponentsInChildren<Mass>())	// 部屋に含まれるマス単位でのループ
					{
						_target_data.target_masses.Add(_target_mass);	// 効果範囲に登録
					}
				}
				else	// 部屋に対する行動ができない
				{
					// 周囲走査
					foreach (float _angle in angles)	// 方向単位でのループ
					{
						// 変数宣言
						Vector2Int _target_idx = _base_idx + (activity ? 1 : -1) * _attackable_directions.CalculateSplitedDirectionInt(_angle);	// 対象マスの番号

						// 保全
						if (_target_idx.x < 0 || _target_idx.x >= DungeonScene.FloorData.MapData.Masses.GetLength(1)
							|| _target_idx.y < 0 || _target_idx.y >= DungeonScene.FloorData.MapData.Masses.GetLength(0))	// マップ外のマス
						{
							break;	// マップ外へ進出させない
						}

						// 変数宣言
						Mass _target_mass = DungeonScene.FloorData.MapData.Masses[_target_idx.y, _target_idx.x];	// 対象となるマス本体を取得
					
						// リスト更新
						if (_target_mass)	// ヌルチェック
						{
							_target_data.target_masses.Add(_target_mass);	// 効果範囲に登録
						}
					}
				}

				// リスト更新
				_target_datas.Add(_target_data);	// 効果対象に登録

				// 終了
				break;	// 分岐処理完了
			}

			// マップ全体
			case MassRange.RangeType.WORLD:
			{
				// 変数宣言
				((float angle, List<GameObject> targets) result_data, List<Mass> target_masses) _target_data = ((transform.eulerAngles.y, new()), new());	// 範囲格納場所

				// 攻撃処理
				foreach (Mass _mass in DungeonScene.FloorData.MapData.Masses)	// マス単位でのループ
				{
					// 保全
					if (!_mass)	// ヌルチェック
					{
						continue;	// マスが存在しないので処理できない
					}
					
					// リスト更新
					_target_data.target_masses.Add(_mass);	// 効果範囲に登録
				}

				// リスト更新
				_target_datas.Add(_target_data);	// 効果対象に登録

				// 終了
				break;	// 分岐処理完了
			}

			// その他
			default:
				Debug.LogError("対応の定義されていない範囲が使用されています");
				break;	// 分岐処理完了
		}

		// 攻撃対象を設定
		foreach (var _target_data in _target_datas)
		{
			foreach (Mass _target_mass in _target_data.target_masses)	// 対象マス単位でのループ
			{
				// 処理分岐
				if (activity)	// 攻撃する側の演算なので攻撃対象を演算する
				{
					for (int _idx = 0; _idx < _target_mass.transform.childCount; _idx++)	// 設置物単位でのループ
					{
						// 変数宣言
						GameObject _target = _target_mass.transform.GetChild(_idx).gameObject;	// 攻撃を受けるオブジェクト

						// フレンドリーファイア
						if (!_data.FriendryFire)	// 味方討ちを防ぐ
						{
							// 変数宣言
							Camp _my_camp = GetComponent<Camp>();	// 自身の陣営

							if (_my_camp)	// 陣営所属済
							{
								// 変数宣言
								Camp _target_camp = _target.GetComponent<Camp>();	// 相手の陣営

								// 免除
								if (_target_camp && _my_camp.Type == _target_camp.Type)	// 同じ陣営に所属しているため候補から外す
								{
									continue;	// 追加させずに次の項へ
								}
							}
							else	// 無所属
							{
#if UNITY_EDITOR
								Debug.LogError("所属陣営が存在しません");
#endif	// end UNITY_EDITOR

								// 免除
								if (_target == gameObject)	// 少なくとも自身は同じ陣営として見做せるため候補から外す
								{
									continue;	// 追加させずに次の項へ
								}
						
							}
						}

						// リスト更新
						_target_data.result_data.targets.Add(_target);	// 攻撃対象として登録
					}
				}
				else	// 攻撃される側の演算なのでそのまま該当マスを演算とする
				{
					// リスト更新
					_target_data.result_data.targets.Add(_target_mass.gameObject);	// 攻撃対象として登録
				}
			}

			// リスト更新
			_result.attackables.Add(_target_data.result_data);	// 攻撃対象として登録
		}

		// 提供
		return _result;	// 演算結果
	}
}