/*=====
<DropDown.cs>

-author
	mizunose

-about
	ドロップダウン式の自由形式選択UIを実装
=====*/

// 名前空間宣言
using UnityEngine;
using UnityEngine.UI;

// クラス定義

/// <summary>
/// <para>自由形式ドロップダウン</para>
/// </summary>
[RequireComponent(typeof(VerticalLayoutGroup))]
public class FreeStyleDropDown : DropDown
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("データ")] private FreeStyleDropDownData _data;

	// プロパティ定義

	/// <value><see cref="_data"/></value>
	protected override DropDownData _Data => _data;

	/// <value><see cref="_choices"/></value>
	protected override SelectableInformation[] _Choices => _data.Choices;
}