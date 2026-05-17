/*=====
<FloatCorrection.cs>

-author
	mizunose

-about
	float型の補正値を定義

-note
・実際には何にも影響していません。
・継承によって対象に対応付けて定義してください。
	子に持たせ、効果側で取得することで値を読み出し、反映させることで、間接的かつ自動的に影響を与えます。
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>float型の補正値</para>
/// </summary>
public abstract class FloatCorrection : MonoBehaviour
{
	// 変数宣言
	[Header("パラメータ")]
	[SerializeField, Tooltip("基礎値補正")] private float _base_correction = 0.0f;
	[SerializeField, Tooltip("補正倍率")] private float _correction_ratio = 1.0f;
	
	// プロパティ定義

	/// <value><see cref="_base_correction"/></value>
	public float BaseCorrection => _base_correction;

	/// <value><see cref="_correction_ratio"/></value>
	public float CorrectionRatio => _correction_ratio;
}