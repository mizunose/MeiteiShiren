/*=====
<InputOpenMenuData.cs>

-author
	mizunose

-about
	メニュー展開入力のデータを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>メニュー展開入力データ</para>
/// </summary>
public class InputOpenMenuData : CreatableData
{
	// 変数宣言
	[Header("メニュー表示")]
	[SerializeField, Tooltip("メニュータブのドロップダウン")] private FreeStyleDropDown _menu_tab_drop_down;

	// プロパティ定義

	/// <value><see cref="_menu_tab_drop_down"/></value>
	public FreeStyleDropDown MenuTabDropDown => _menu_tab_drop_down;
}