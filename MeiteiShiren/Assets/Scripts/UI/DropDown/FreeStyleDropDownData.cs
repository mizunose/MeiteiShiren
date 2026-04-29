/*
<FreeStyleDropDownData.cs>

-author
	mizunose

-about
	自由形式ドロップダウンのデータ
*/

// 名前空間宣言
using System.Collections.Generic;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>自由形式ドロップダウンのデータ</para>
/// </summary>
public class FreeStyleDropDownData : DropDownData
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("選択肢")] private List<DropDown.SelectableInformation> _choices;

	// プロパティ定義

	/// <value><see cref="_choices"/></value>
	public List<DropDown.SelectableInformation> Choices => _choices;
}