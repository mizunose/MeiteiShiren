/*
<InventoryDropDownData.cs>

-author
	mizunose

-about
	インベントリのドロップダウン部分のデータ
*/

// 名前空間宣言
using System.Collections.Generic;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>ドロップダウン式インベントリのデータ</para>
/// </summary>
public class InventoryDropDownData : DropDownData
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("インベントリデータ")] private ItemInventoryData _items_data;
	[SerializeField, Tooltip("キャンセル文")] private string _cancel_text = "閉じる";
	[SerializeField, Tooltip("アイテムに対するUI")] private UseItemDropDown _item_use_ui;

	// プロパティ定義
	
	/// <value><see cref="_items_data"/></value>
	public ItemInventoryData ItemsData => _items_data;

	/// <value><see cref="_cancel_text"/></value>
	public string CancelText => _cancel_text;

	/// <value><see cref="_item_use_ui"/></value>
	public UseItemDropDown ItemUseUI => _item_use_ui;
}