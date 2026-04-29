/*=====
<Inventory.cs>

-author
	mizunose

-about
	所持資産管理を定義
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>所持資産一覧</para>
/// </summary>
[DisallowMultipleComponent]
public abstract class Inventory : MonoBehaviour
{
	// 変数宣言
	[SerializeField, Tooltip("アイテム保存データ")] private ItemInventoryData _items_data;
	private int _capacity = 0;	// 容量
	

	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Start()
	{
		// 管理物整理
		_items_data.ItemInventory.RemoveAll(item => !item);	// ヌルを管理から除去

		// 初期化
		_items_data.User = gameObject;	// 使用者として登録
	}


	/// <summary>
	/// <para>アイテム追加</para>
	/// </summary>
	/// <param name="item">登録対象</param>
	/// <returns>処理の成否(追加に失敗するとfalse)</returns>
	public bool AddItem(Item item)
	{
		// 更新
		item.transform.SetParent(CacheContainer.Instance.transform);	// キャッシュ領域に退避
		_items_data.Add(item);	// 一覧に登録
		
		// 提供
		return true;	// 処理成功
	}
}