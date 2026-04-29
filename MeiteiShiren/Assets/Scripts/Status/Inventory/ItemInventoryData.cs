/*
<InventoryData.cs>

-author
	mizunose

-about
	インベントリのデータ
*/

// 名前空間宣言
using System;
using System.Collections.Generic;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>インベントリのデータ</para>
/// </summary>
public class ItemInventoryData : CreatableData
{
	// イベント定義
	public event Action OnListChanged;	// リスト操作時のイベント

	// 変数宣言
	private List<Item> _items = new();	// 管理領域


	// プロパティ定義

	/// <value>使用者</value>
	public GameObject User { get; set; }

	/// <value><see cref="_items"/></value>
	public List<Item> ItemInventory => _items;


	/// <summary>
	/// <para>アイテム保存処理</para>
	/// </summary>
	public void Add(Item item)
	{
		// 更新
		_items.Add(item);	// リスト登録

		// イベント接続
		item.OnDestroyed += ()=>{
			// 更新
			_items.Remove(item);	// 消えるアイテムを管理対象から外す

			// イベント発行
			OnListChanged?.Invoke();	// リスト操作時イベント発行
		};	// アイテムを失ったときリストを更新する

		// イベント発行
		OnListChanged?.Invoke();	// リスト操作時イベント発行
	}


	/// <summary>
	/// <para>リスト初期化</para>
	/// </summary>
	public void Clear()
	{
		// 初期化
		_items.Clear();	// リストをクリアする
		User = null;	// 所有者をリセットする
	}
}