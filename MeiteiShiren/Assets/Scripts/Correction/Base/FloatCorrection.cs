/*=====
<FloatCorrection.cs>

-author
	mizunose

-about
	補正値を定義

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
	[SerializeField, Tooltip("基礎値補正")] private float m_fBaseCorrection = 0.0f;
	[SerializeField, Tooltip("補正倍率")] private float m_fCorrectionRatio = 1.0f;
	
	// プロパティ定義

	/// <summary>
	/// 基礎補正プロパティ
	/// </summary>
	/// <value><see cref="m_fBaseCorrection"/></value>
	public float BaseCorrection
	{
		get
		{
			// 提供
			return m_fBaseCorrection;	// 基礎値補正を提供
		}
		set
		{
			// 更新
			m_fBaseCorrection = value;	// 基礎値補正を更新
		}
	}

	/// <summary>
	/// 補正倍率プロパティ
	/// </summary>
	/// <value><see cref="m_fCorrectionRatio"/></value>
	public float CorrectionRatio
	{
		get
		{
			// 提供
			return m_fCorrectionRatio;	// 補正倍率を提供
		}
		set
		{
			// 更新
			m_fCorrectionRatio = value;	// 補正倍率を更新
		}
	}
}