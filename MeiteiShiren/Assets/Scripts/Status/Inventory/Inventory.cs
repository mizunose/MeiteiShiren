/*=====
<Inventory.cs>

-author
	mizunose

-about
	所持資産管理を定義
=====*/

// 名前空間宣言
using System.Collections.Generic;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>所持資産一覧</para>
/// </summary>
[DisallowMultipleComponent]
public abstract class Inventory : MonoBehaviour
{
	// 変数宣言
	private List<Item> _items = new();	// 管理領域
	private int _capacity = 0;	// 容量


	/// <summary>
	/// <para>アイテム追加</para>
	/// </summary>
	/// <param name="item">登録対象</param>
	/// <returns>処理の成否(追加に失敗するとfalse)</returns>
	public bool AddItem(Item item)
	{


		// 更新
		item.transform.SetParent(CacheContainer.Instance.transform);	// キャッシュ領域に退避
		_items.Add(item);	// 一覧に登録

		// 提供
		return true;	// 処理成功
	}
}