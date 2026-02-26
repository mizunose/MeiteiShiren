/*=====
<SplitedDirections.cs>

-author
	mizunose

-about
	次元を限定した方向を定義
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>分割された方向</para>
/// </summary>
public abstract class SplitedDirections
{
	// 定数定義
	protected const float _ROUND_DEGREE = 360.0f;	// 円の角度

	// プロパティ定義

	/// <value>定義内の角度一覧</value>
	public abstract float[] SplitedAngles { get; }


	/// <summary>
	/// <para>角度を限定された方向に変換する</para>
	/// </summary>
	/// <param name="angle">角度で表された向き</param>
	/// <returns>変換後のベクトル</returns>
	public abstract Vector2Int CalculateSplitedDirectionInt(float angle);
}