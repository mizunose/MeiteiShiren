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
public abstract class Attack : MonoBehaviour
{
	// 定数定義
	private const float _ROUND_DEGREE = 360.0f;	// 円の角度
	private const int _SPLIT_DIRECTION = 8;	// 攻撃の方向候補数

	// 変数宣言
	[SerializeField, Tooltip("データ")] private AttackData _data;


	//TODO:敵は複数方向、プレイヤーは単一方向で演算
	/// <summary>
	/// <para>試算処理</para>
	/// </summary>
	/// <returns>試算結果</returns>
	public List<GameObject> Simulate()
	{
		// 変数宣言
		List<GameObject> _result = new();	// 演算結果格納用
		Mass _current_mass;	// 現在のマス

		// 初期化
		if (transform.parent)	// ヌルチェック
		{
			_current_mass = transform.parent.GetComponent<Mass>();	// 現在マスの取得

			// 保全
			if(!_current_mass)	// ヌルチェック
			{
#if UNITY_EDITOR
				Debug.LogError("親がマスではありません");
#endif	// end UNITY_EDITOR

				// 終了
				return _result;	// マスを使用した演算が成立しないため中断
			}
		}
		else
		{
#if UNITY_EDITOR
			Debug.LogError("所属マスが特定できませんでした");
#endif	// end UNITY_EDITOR

			// 終了
			return _result;	// マスを使用した演算が成立しないため中断
		}

		// 変数宣言
		Vector2Int _mass_idx = Map.PositionToMass(_current_mass.transform.position);	// 現在マスの番号

		// 保全
		if (!_data)	// ヌルチェック
		{
#if UNITY_EDITOR
			Debug.LogError("攻撃のデータが設定されていません");
#endif	// end UNITY_EDITOR
			return _result;	// 処理不可能なため終了
		}

		// 変数宣言
		List<Mass> _target_masses = new();	// 効果対象のマス一覧

		// 初期化
		switch (_data.Range.Type)	// 範囲の定義によって分岐
		{
			// 限定範囲
			case MassRange.RangeType.RANGED:
				
				// 範囲登録
				foreach (Vector2Int _shift in _data.Range.Range)	// マス単位でのループ
				{
					// 変数宣言
					Vector2Int _target_idx = _mass_idx + new Vector2Int(
						(int)(_shift.x * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y)) + (int)(_shift.y * Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y)),
						(int)(_shift.x * Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y)) + (int)(_shift.y * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y))
						);	// 対象マスの番号	※鉛直上向きを基準に範囲を回転させている

					// 保全
					if (_target_idx.x < 0 || _target_idx.x >= Dungeon.Instance.FloorData.MapData.Masses.GetLength(1)
						|| _target_idx.y < 0 || _target_idx.y >= Dungeon.Instance.FloorData.MapData.Masses.GetLength(0))	// マップ外のマス
					{
						continue;	// マスとして処理できないので次の処理へ移る
					}

					// 変数宣言
					Mass _target_mass = Dungeon.Instance.FloorData.MapData.Masses[_target_idx.y, _target_idx.x];	// 対象となるマス本体を取得

					// 保全
					if (!_target_mass)	// ヌルチェック
					{
						continue;	// マスが存在しないので次の処理へ移る
					}

					// リスト更新
					_target_masses.Add(_target_mass);	// 効果範囲に登録
				}

				// 終了
				break;	// 分岐処理完了

			// 直線範囲
			case MassRange.RangeType.FRONT_LINE:

				// 攻撃処理
				while (true)	// マス単位でのループ
				{
					// 初期化
					if (transform.parent)	// ヌルチェック
					{
						// 更新
						_mass_idx += CalculateAttackDirection(transform.eulerAngles.y);	// 移動して走査

						// 終了条件
						if (_mass_idx.x < 0 || _mass_idx.x >= Dungeon.Instance.FloorData.MapData.Masses.GetLength(1)
							|| _mass_idx.y < 0 || _mass_idx.y >= Dungeon.Instance.FloorData.MapData.Masses.GetLength(0))	// マップ外のマス
						{
							break;	// マップ外へ進出させない
						}

						// 変数宣言
						Mass _target_mass = Dungeon.Instance.FloorData.MapData.Masses[_mass_idx.y, _mass_idx.x];	// 対象となるマス本体を取得

						// 終了条件
						if (!_target_mass)	// ヌルチェック
						{
							break;	// マスが存在しないので打ち止めにする
						}
						//TODO:壁で妨害されていて先に届かない

						// リスト更新
						_target_masses.Add(_target_mass);	// 効果範囲に登録
					}
				}

				// 終了
				break;	// 分岐処理完了

			// 部屋全体
			case MassRange.RangeType.ROOM:

				// 変数宣言
				Room _current_room = null;	// 現在いる部屋

				// 初期化
				if (_current_mass.transform.parent)	// ヌルチェック
				{
					_current_room = _current_mass.transform.parent.GetComponent<Room>();	// 現在マスから部屋を取得
				}

				// 処理分岐
				if (_current_room)	// ヌルチェック
				{
					// リスト更新
					foreach (Mass _target_mass in _current_room.GetComponentsInChildren<Mass>())	// 部屋に含まれるマス単位でのループ
					{
						_target_masses.Add(_target_mass);	// 効果範囲に登録
					}
				}
				else	// 部屋に対する行動ができない
				{
					// 更新
					_mass_idx += CalculateAttackDirection(transform.eulerAngles.y);	// 移動して走査

					// 保全
					if (_mass_idx.x < 0 || _mass_idx.x >= Dungeon.Instance.FloorData.MapData.Masses.GetLength(1)
						|| _mass_idx.y < 0 || _mass_idx.y >= Dungeon.Instance.FloorData.MapData.Masses.GetLength(0))	// マップ外のマス
					{
						break;	// マップ外へ進出させない
					}

					// 変数宣言
					Mass _target_mass = Dungeon.Instance.FloorData.MapData.Masses[_mass_idx.y, _mass_idx.x];	// 対象となるマス本体を取得
					
					// リスト更新
					if (_target_mass)	// ヌルチェック
					{
						_target_masses.Add(_target_mass);	// 効果範囲に登録
					}
				}

				// 終了
				break;	// 分岐処理完了
				
			// マップ全体
			case MassRange.RangeType.WORLD:

				// 攻撃処理
				foreach (Mass _mass in Dungeon.Instance.FloorData.MapData.Masses)	// マス単位でのループ
				{
					// 保全
					if (!_mass)	// ヌルチェック
					{
						continue;	// マスが存在しないので処理できない
					}
					
					// リスト更新
					_target_masses.Add(_mass);	// 効果範囲に登録
				}

				// 終了
				break;	// 分岐処理完了
				
			// その他
			default:
				Debug.LogError("対応の定義されていない範囲が使用されています");
				break;	// 分岐処理完了
		}

		// 攻撃対象を設定
		foreach (Mass _target_mass in _target_masses)	// 対象マス単位でのループ
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
				_result.Add(_target);	// 攻撃対象として登録
			}
		}

		// 提供
		return _result;	// 演算結果
	}


	/// <summary>
	/// <para>攻撃モーション処理</para>
	/// </summary>
	/// <param name="targets">攻撃対象</param>
	/// <returns>遅延処理用のインターフェース</returns>
	public IEnumerator AttackMotion(List<GameObject> targets)
	{
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
			DoAttack(targets);	// 攻撃の効果発生タイミング

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
		foreach (GameObject _target in targets)	// 発動対象ごとのループ
		{
			foreach (var _affect in _data.Affects)	// 効果単位でのループ
			{
				if (_affect)	// ヌルチェック
				{
					_affect.Boot(gameObject, _target);	// 設置物に効果発動
				}
			}
		}
	}


	/// <summary>
	/// <para>角度を攻撃方向に変換する</para>
	/// </summary>
	/// <param name="angle">角度で表された向き</param>
	/// <returns>攻撃方向を示すベクトル</returns>
	private Vector2Int CalculateAttackDirection(float angle)
	{
		// 初期化
		switch ((int)((angle + _ROUND_DEGREE / _SPLIT_DIRECTION / 2) / (_ROUND_DEGREE / _SPLIT_DIRECTION)))	// 攻撃方向によって分岐
		{
			// 0 / 8
			case 0:
				return Vector2Int.up;	// 上

			// 1 / 8
			case 1:
				return Vector2Int.one;	// 右上

			// 2 / 8
			case 2:
				return Vector2Int.right;	// 右

			// 3 / 8
			case 3:
				return new Vector2Int(1, -1);	// 右下

			// 4 / 8
			case 4:
				return Vector2Int.down;	// 下

			// 5 / 8
			case 5:
				return -Vector2Int.one;	// 左下

			// 6 / 8
			case 6:
				return Vector2Int.left;	// 左

			// 7 / 8
			case 7:
				return new Vector2Int(-1, 1);	// 左上

			// その他
			default:
#if UNITY_EDITOR
				Debug.LogError("攻撃方向に対応が定義されていません");
#endif	// end UNITY_EDITOR
				return Vector2Int.up;	// 仮データ
		}
	}
}