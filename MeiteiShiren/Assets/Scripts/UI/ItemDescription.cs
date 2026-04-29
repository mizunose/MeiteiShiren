/*=====
<InventoryItemDescription.cs>

-author
	mizunose

-about
	インベントリのドロップダウン部分を実装
=====*/

// 名前空間宣言
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// クラス定義

/// <summary>
/// <para>ドロップダウン式インベントリ</para>
/// </summary>
[RequireComponent(typeof( TextMeshProUGUI))]
public class InventoryItemDescription : MonoBehaviour
{
	// 変数宣言
	[Header("内部参照")]
	[SerializeField, Tooltip("アイテム一覧表示領域")] private InventoryDropDown _item_drop_down;
	private TextMeshProUGUI _description_text;	// 説明文表示領域

	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	protected virtual void Awake()
	{
		// 初期化
		_description_text = GetComponent<TextMeshProUGUI>();	// 説明文表示領域取得
		OnCursorMoved();	// 説明文初期化
	}


	/// <summary>
	/// <para>有効時処理</para>
	/// </summary>
	protected virtual void OnEnable()
	{
		// イベント接続
		if (_item_drop_down)	// ヌルチェック
		{
			_item_drop_down.OnCursorMoved += OnCursorMoved;	// カーソル移動時処理を接続
		}
	}


	/// <summary>
	/// <para>無効時処理</para>
	/// </summary>
	protected virtual void OnDisable()
	{
		// イベント接続解除
		if (_item_drop_down)	// ヌルチェック
		{
			_item_drop_down.OnCursorMoved -= OnCursorMoved;	// カーソル移動時処理を解除
		}
	}


	/// <summary>
	/// <para>インベントリカーソル移動時処理</para>
	/// </summary>
	private void OnCursorMoved()
	{
		// 更新
		_description_text.SetText(_item_drop_down.ItemOnCursor ? _item_drop_down.ItemOnCursor.Data.Description : "");	// テキストを選択したアイテムのものに更新
	}
}