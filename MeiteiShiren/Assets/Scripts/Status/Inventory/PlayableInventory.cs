/*=====
<PlayableInventory.cs>

-author
	mizunose

-about
	操作可能なインベントリを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>操作可能なインベントリ</para>
/// </summary>
public class PlayableInventory : Inventory
{
	// 変数宣言
	[SerializeField, Tooltip("データ")] private PlayableInventoryControllerData _data;


	/// <summary>
	/// <para>有効時処理</para>
	/// </summary>
	private void OnEnable()
	{
		// イベント接続
		_data.TriggerEvent.signal += OnDrawInventoryGUI;	// 起動時の処理を接続
	}


	/// <summary>
	/// <para>無効時処理</para>
	/// </summary>
	private void OnDisable()
	{
		// イベント接続解除
		_data.TriggerEvent.signal -= OnDrawInventoryGUI;	// 起動時の処理を解除
	}


	/// <summary>
	/// <para>インベントリ内表示処理</para>
	/// </summary>
	private void OnDrawInventoryGUI()
	{
		// 生成
		Instantiate(_data.ItemsInventory);	// 選択UIのインスタンス生成
	}
}