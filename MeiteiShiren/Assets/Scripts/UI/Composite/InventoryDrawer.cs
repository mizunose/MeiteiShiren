/*=====
<InventoryDrawer.cs>

-author
	mizunose

-about
	インベントリ表示を実装
=====*/

// 名前空間宣言
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
	protected override void OnEnable()
	{
		// 継承
		base.OnDisable();	// 親関数の起動

		// イベント接続
		if(_item_drop_down)	// ヌルチェック
		{
			_item_drop_down.OnEnabled += EnableWith;	// アイテム一覧表示有効化時の処理を接続
			_item_drop_down.OnDisabled += DisableWith;	// アイテム一覧表示無効化時の処理を接続
			_item_drop_down.OnDestroyed += DestroyWith;	// アイテム一覧表示破棄時の処理を接続
		}
	}


	/// <summary>
	/// <para>無効時処理</para>
	/// </summary>
	protected override void OnDisable()
	{
		// 継承
		base.OnDisable();	// 親関数の起動

		// イベント接続解除
		if(_item_drop_down)	// ヌルチェック
		{
			_item_drop_down.OnEnabled -= EnableWith;	// アイテム一覧表示有効化時の処理を解除
			_item_drop_down.OnDisabled -= DisableWith;	// アイテム一覧表示無効化時の処理を解除
			_item_drop_down.OnDestroyed -= DestroyWith;	// アイテム一覧表示破棄時の処理を解除
		}
	}


	/// <summary>
	/// <para>他UI有効化時連鎖有効化処理</para>
	/// </summary>
	private void EnableWith()
	{
		Debug.Log("？");
		// 更新
		enabled = true;	// 自身も無効化
	}


	/// <summary>
	/// <para>他UI無効化時連鎖無効化処理</para>
	/// </summary>
	private void DisableWith()
	{
		// 更新
		enabled = false;	// 自身も無効化
	}


	/// <summary>
	/// <para>他UI破棄時連鎖破棄処理</para>
	/// </summary>
	private void DestroyWith()
	{
		// 更新
		Destroy(gameObject);	// 自身も破棄
	}
}