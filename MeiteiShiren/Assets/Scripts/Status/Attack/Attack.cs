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
using UnityEngine.UIElements;
using static MassRange;

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


	/// <summary>
	/// <para>攻撃モーション処理</para>
	/// </summary>
	/// <returns>遅延処理用のインターフェース</returns>
	protected abstract IEnumerator AttackMotion();


	/// <summary>
	/// <para>攻撃モーションを取り終えた時の攻撃処理部分</para>
	/// </summary>
	/// <param name="direction">攻撃モーション前(入力時点)での向き</param>
	protected void OnAttacked(float angle)
	{
		// 補正
		angle = Mathf.Repeat(angle, _ROUND_DEGREE);	// 向きを 0 to 360 に補正


		// 変数宣言
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
				return;	// マスを使用した演算が成立しないため中断
			}
		}
		else
		{
#if UNITY_EDITOR
			Debug.LogError("所属マスが特定できませんでした");
#endif	// end UNITY_EDITOR

			// 終了
			return;	// マスを使用した演算が成立しないため中断
		}

		// 変数宣言
		Vector2Int _mass_idx = Map.PositionToMass(_current_mass.transform.position);	// 現在マスの番号

		// 保全
		if (!_data)	// ヌルチェック
		{
#if UNITY_EDITOR
			Debug.LogError("攻撃のデータが設定されていません");
#endif	// end UNITY_EDITOR
			return;	// 処理不可能なため終了
		}

		// 変数宣言
		List<Mass> _target_masses = new();	// 効果対象のマス一覧

		// 初期化
		switch (_data.Range.Type)	// 範囲の定義によって分岐
		{
			// 限定範囲
			case RangeType.RANGED:
				
				// 範囲登録
				foreach (Vector2Int _shift in _data.Range.Range)	// マス単位でのループ
				{
					// 変数宣言
					Vector2Int _target_idx = _mass_idx + new Vector2Int(
						(int)(_shift.x * Mathf.Cos(Mathf.Deg2Rad * angle)) + (int)(_shift.y * Mathf.Sin(Mathf.Deg2Rad * angle)),
						(int)(_shift.x * Mathf.Sin(Mathf.Deg2Rad * angle)) + (int)(_shift.y * Mathf.Cos(Mathf.Deg2Rad * angle))
						);	// 対象マスの番号	※鉛直上向きを基準に範囲を回転させている

					// 保全
					if (_target_idx.x >= Dungeon.Instance.Map.Masses.GetLength(1) || _target_idx.y >= Dungeon.Instance.Map.Masses.GetLength(0))	// マップ外のマス
					{
						continue;	// マスとして処理できないので次の処理へ移る
					}

					// 変数宣言
					Mass _target_mass = Dungeon.Instance.Map.Masses[_target_idx.y, _target_idx.x];	// 対象となるマス本体を取得

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
			case RangeType.FRONT_LINE:

				// 変数宣言
				Vector2Int _direction;	// 攻撃方向

				// 初期化
				switch ((int)((angle + _ROUND_DEGREE / _SPLIT_DIRECTION / 2) / (_ROUND_DEGREE / _SPLIT_DIRECTION)))	// 攻撃方向によって分岐
				{
					// 0 / 8
					case 0:
						_direction = Vector2Int.up;	// 上
						break;	// 分岐処理完了
						
					// 1 / 8
					case 1:
						_direction = Vector2Int.one;	// 右上
						break;	// 分岐処理完了
						
					// 2 / 8
					case 2:
						_direction = Vector2Int.right;	// 右
						break;	// 分岐処理完了
						
					// 3 / 8
					case 3:
						_direction = new Vector2Int(1, -1);	// 右下
						break;	// 分岐処理完了

					// 4 / 8
					case 4:
						_direction = Vector2Int.down;	// 下
						break;	// 分岐処理完了

					// 5 / 8
					case 5:
						_direction = -Vector2Int.one;	// 左下
						break;	// 分岐処理完了

					// 6 / 8

					case 6:
						_direction = Vector2Int.left;	// 左
						break;	// 分岐処理完了

					// 7 / 8
					case 7:
						_direction = new Vector2Int(-1, 1);	// 左上
						break;	// 分岐処理完了

					// その他
					default:
#if UNITY_EDITOR
						Debug.LogError("攻撃方向に対応が定義されていません");
#endif	// end UNITY_EDITOR
						_direction = Vector2Int.up;	// 仮データ
						break;	// 分岐処理完了
				}

				// 攻撃処理
				while (true)	// マス単位でのループ
				{
					// 初期化
					if (transform.parent)	// ヌルチェック
					{
						// 更新
						_mass_idx += _direction;	// 移動して走査

						// 終了条件
						if (_mass_idx.x >= Dungeon.Instance.Map.Masses.GetLength(1) || _mass_idx.y >= Dungeon.Instance.Map.Masses.GetLength(0))	// マップ外のマス
						{
							break;	// マップ外へ進出させない
						}

						// 変数宣言
						Mass _target_mass = Dungeon.Instance.Map.Masses[_mass_idx.y, _mass_idx.x];	// 対象となるマス本体を取得

						// 終了条件
						if (!_target_mass)	// ヌルチェック
						{
							break;	// マスが存在しないので打ち止めにする
						}
						if (_target_mass.type == Mass.Type.WALL)	// 壁
						{
							break;	// 壁で妨害されていて先に届かない
						}

						// リスト更新
						_target_masses.Add(_target_mass);	// 効果範囲に登録
					}
				}

				// 終了
				break;	// 分岐処理完了

			// 部屋全体
			case RangeType.ROOM:

				//TODO:マスの親をroomに

				// 終了
				break;	// 分岐処理完了
				
			// マップ全体
			case RangeType.WORLD:

				// 攻撃処理
				foreach (Mass _mass in Dungeon.Instance.Map.Masses)	// マス単位でのループ
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

		// 効果処理
		foreach (Mass _target_mass in _target_masses)	// 対象マス単位でのループ
		{
			for (int _idx = 0; _idx < _target_mass.transform.childCount; _idx++)	// 設置物単位でのループ
			{
				foreach (var _affect in _data.Affects)	// 効果単位でのループ
				{
					if (_affect)	// ヌルチェック
					{
						_affect.Boot(gameObject, _target_mass.transform.GetChild(_idx).gameObject);	// 設置物に効果発動
						}
					}
			}
			Destroy(_target_mass.gameObject);
		}
	}
}