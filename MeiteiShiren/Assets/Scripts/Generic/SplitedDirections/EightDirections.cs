/*=====
<EightDirections.cs>

-author
	mizunose

-about
	8次元の方向を実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>8方向</para>
/// </summary>
public class EightDirections : SplitedDirections
{
	// 定数定義
	public const int SPLIT_DIRECTION = 8;	// 方向候補数
	static float[] _SPLITED_ANGLES = Culc();	// 角度候補一覧

	// プロパティ定義

	/// <value><see cref="_SPLITED_ANGLES"/></value>
	public override float[] SplitedAngles => _SPLITED_ANGLES;

	/// <summary>
	/// <para>角度を限定された方向に変換する</para>
	/// </summary>
	/// <param name="angle">角度で表された向き</param>
	/// <returns>変換後のベクトル</returns>
	public override Vector2Int CalculateSplitedDirectionInt(float angle)
	{
		// 初期化
		switch ((int)((angle + _ROUND_DEGREE / SPLIT_DIRECTION / 2) / (_ROUND_DEGREE / SPLIT_DIRECTION)))	// 攻撃方向によって分岐
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
				Debug.LogError("方向に対応が定義されていません");
#endif	// end UNITY_EDITOR
				return Vector2Int.zero;	// 仮データ
		}
	}




	private static float[] Culc()
	{
			// 変数宣言
			float[] _result = new float[SPLIT_DIRECTION];	// 演算結果格納用

			// 初期化
			for (int _direction_idx = 0; _direction_idx < SPLIT_DIRECTION; _direction_idx++)	// 攻撃方向単位でのループ
			{
				_result[_direction_idx] = _ROUND_DEGREE * _direction_idx / SPLIT_DIRECTION;	// 攻撃方向から角度を算出
			}

			// 提供
			return _result;	// 演算結果
	}
}