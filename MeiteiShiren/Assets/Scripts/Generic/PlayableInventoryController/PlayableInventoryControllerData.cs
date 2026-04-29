/*=====
<PlayableInventoryControllerData.cs>

-author
	mizunose

-about
	インベントリ操作GUIのデータを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>インベントリGUIデータ</para>
/// </summary>
public class PlayableInventoryControllerData : CreatableData
{
	// 変数宣言
	[Header("インベントリ表示")]
	[SerializeField, Tooltip("起因イベント")] private NoneArgumentEventData _trigger_event;
	[SerializeField, Tooltip("インベントリUI")] private InventoryDrawer _items_inventory;

	// プロパティ定義

	/// <value><see cref="_trigger_event"/></value>
	public NoneArgumentEventData TriggerEvent => _trigger_event;

	/// <value><see cref="_items_inventory"/></value>
	public InventoryDrawer ItemsInventory => _items_inventory;
}