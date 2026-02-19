/*=====
<StairData.cs>

-author
	mizunose

-about
	階段マスのデータを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
[CreateAssetMenu(menuName = _NAME, fileName = _NAME)]
public class StairData : ScriptableObject
{
	// 定数定義
	private const string _NAME = "Stair";   // タブ名称

	// 変数宣言
	[Header("処理前確認")]
	[SerializeField, Tooltip("動作確認のドロップダウン")] private YesNoDropDown _confirm_drop_down;

	// プロパティ定義

	/// <value><see cref="_confirm_drop_down"/></value>
	public YesNoDropDown ConfirmDropDown => _confirm_drop_down;
}