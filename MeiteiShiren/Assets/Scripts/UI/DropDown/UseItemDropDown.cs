/*=====
<UseItemDropDown.cs>

-author
	mizunose

-about
	アイテム使用ドロップダウンのデータ
=====*/

// 名前空間宣言
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// クラス定義

/// <summary>
/// <para>ドロップダウン式 Y/N ボタン</para>
/// </summary>
[RequireComponent(typeof(VerticalLayoutGroup))]
public class UseItemDropDown : DropDown
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("データ")] private UseItemDropDownData _data;
	private List<SelectableInformation> _choices;

	// プロパティ定義

	/// <value>はい選択時イベント</value>
	public NoneArgumentEventData UseEvent { get; private set; }
	
	/// <value>いいえ選択時イベント</value>
	public NoneArgumentEventData DisposeEvent { get; private set; }

	/// <value><see cref="_data"/></value>
	protected override DropDownData _Data => _data;

	/// <value><see cref="_choices"/></value>
	protected override List<SelectableInformation> _Choices => _choices;


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	protected override void Awake()
	{
		// 初期化
		UseEvent = ScriptableObject.CreateInstance<NoneArgumentEventData>();
		DisposeEvent = ScriptableObject.CreateInstance<NoneArgumentEventData>();
		_choices = new(){
			new SelectableInformation{choise_value = _data.UseText, event_data = UseEvent},	// 使用コマンド
			new SelectableInformation{choise_value = _data.DisposeText, event_data = DisposeEvent},	// 廃棄コマンド
			};

		// 継承
		base.Awake();	// 親関数の起動
	}
}