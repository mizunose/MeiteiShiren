/*=====
<InventoryDropDown.cs>

-author
	mizunose

-about
	インベントリのドロップダウン部分を実装
=====*/

// 名前空間宣言
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// クラス定義

/// <summary>
/// <para>ドロップダウン式インベントリ</para>
/// </summary>
[RequireComponent(typeof(VerticalLayoutGroup))]
public class InventoryDropDown : DropDown
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("データ")] private InventoryDropDownData _data;
	private List<SelectableInformation> _choices = new();	// 選択肢一覧
	private List<Item> _choosable_items = new();	// 選択肢として作成したアイテム一覧(選択肢順)

	// プロパティ定義

	/// <value><see cref="_data"/></value>
	protected override DropDownData _Data => _data;

	/// <value><see cref="_choices"/></value>
	protected override List<SelectableInformation> _Choices => _choices;

	/// <value>現在カーソルが選択中のアイテム</value>
	public Item ItemOnCursor => _SelectedIndex < _choosable_items.Count ? _choosable_items[_SelectedIndex] : null;


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	protected override void Awake()
	{
		// 初期化
		OnChangeChoices();	// 選択肢を初期化

		// 継承
		base.Awake();	// 親関数の起動
	}


	/// <summary>
	/// <para>有効時処理</para>
	/// </summary>
	protected override void OnEnable()
	{
		// 継承
		base.OnEnable();	// 親関数の起動

		// イベント接続
		_data.ItemsData.OnListChanged += OnChangeChoices;	// インベントリ更新時の処理を接続
	}


	/// <summary>
	/// <para>無効時処理</para>
	/// </summary>
	protected override void OnDisable()
	{
		// 継承
		base.OnDisable();	// 親関数の起動

		// イベント接続解除
		_data.ItemsData.OnListChanged -= OnChangeChoices;	// インベントリ更新時の処理を解除
	}


	/// <summary>
	/// <para>選択肢変更時処理</para>
	/// </summary>
	private void OnChangeChoices()
	{
		// 初期化
		_choices.Clear();	// 初期化対象のリストクリア
		_choosable_items.Clear();	// 初期化対象のリストクリア

		// 更新
		foreach (var item in _data.ItemsData.ItemInventory)	// アイテム単位でのループ
		{
			ChoiseUIValue _print_content = new ChoiseUIValue {
				text = item.Data.Name,	// アイテムの名前
				icon = item.Data.Icon,	// アイテムのアイコン
			};
			_choices.Add(new SelectableInformation{choise_value = _print_content, event_data = null, sub_ui = _data.ItemUseUI});	// アイテム選択肢追加
			_choosable_items.Add(item);	// 選択肢に対応したアイテム登録
		}
		ResetChoiceUI();	// 表示テキスト更新
	}


	/// <summary>
	/// <para>サブ階層UI作成処理</para>
	/// </summary>
	/// <returns>作成後のUI</returns>
	protected override UserInterface CreateSubUI()
	{
		// 変数宣言
		var _sub_ui = UIPage.Instance.OpenUI(_data.ItemUseUI, _data.Durability);	// サブ階層のUI
		var _item = _choosable_items[_SelectedIndex];	// 該当するアイテム

		// イベント接続
		_sub_ui.UseEvent.signal += () =>{
			// アイテム使用
			ItemExecutor.Instance.PlayItemMotion(_item, _data.ItemsData.User);	// アイテムのモーション再生
		};	// アイテム使用処理を設定
		_sub_ui.DisposeEvent.signal += () =>{
			// アイテム廃棄
			if (!_data.ItemsData.Drop(_item))
			{
				Debug.Log("アイテムを置く場所がありません");
			}
			//TODO:もし周囲が埋まっていたら失敗表示、成功したらターンを進める
		};	// アイテム廃棄処理を設定

		// 提供
		return _sub_ui;	// 作成したUIを返す
	}
}