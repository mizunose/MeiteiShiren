/*=====
<Camp.cs>

-author
	mizunose

-about
	陣営を実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
/// <summary>
/// <para>飢え</para>
/// </summary>
public class Camp : MonoBehaviour
{
	// 変数宣言
	[SerializeField, Tooltip("データ")] private CampData _data;

	// プロパティ定義

	/// <value>所属陣営</value>
	public CampData.CampType Type => _data.Type;
}