/*=====
<InventoryDrawer.cs>

-author
	mizunose

-about
	インベントリ表示を実装
=====*/

// 名前空間宣言
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>インベントリ表示</para>
/// </summary>

public class InventoryDrawer : UserInterface
{
	// 変数宣言
	[Header("内部参照")]
	[SerializeField, Tooltip("アイテム一覧表示領域")] private InventoryDropDown _item_drop_down;


	/// <summary>
	/// <para>有効時処理</para>
	/// </summary>
	protected virtual void OnEnable()
	{
		// イベント接続
		if(_item_drop_down)	// ヌルチェック
		{
			_item_drop_down.OnDestroyed += DestroyWith;	// アイテム一覧表示破棄時の処理を接続
		}
	}


	/// <summary>
	/// <para>無効時処理</para>
	/// </summary>
	protected virtual void OnDisable()
	{
		// イベント接続解除
		if(_item_drop_down)	// ヌルチェック
		{
			_item_drop_down.OnDestroyed -= DestroyWith;	// アイテム一覧表示破棄時の処理を解除
		}
	}


	/// <summary>
	/// <para>他UI破棄イベント時連鎖破棄処理</para>
	/// </summary>
	private void DestroyWith()
	{
		// 更新
		Destroy(gameObject);	// 自身も破棄
	}
}