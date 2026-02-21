/*
<FreeStyleDropDownData.cs>

-author
	mizunose

-about
	自由形式ドロップダウンのデータ
*/

// 名前空間宣言
using UnityEngine;

// クラス定義
[CreateAssetMenu(menuName = _MENU_TAB_NAME + _NAME, fileName = _NAME + "Data")]
public class FreeStyleDropDownData : DropDownData
{
	// 定数定義
	private const string _NAME = "FreeStyleDropDown";	// アセット名

	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("選択肢")] private DropDown.SelectableInformation[] _choices;

	// プロパティ定義

	/// <value><see cref="_choices"/></value>
	public DropDown.SelectableInformation[] Choices => _choices;
}