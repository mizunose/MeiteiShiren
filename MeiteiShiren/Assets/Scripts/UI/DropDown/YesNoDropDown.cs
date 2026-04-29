/*=====
<YesNoDropDown.cs>

-author
	mizunose

-about
	ドロップダウン式の Y/N ボタンを実装
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
public class YesNoDropDown : DropDown
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("データ")] private YesNoDropDownData _data;
	private List<SelectableInformation> _choices;

	// プロパティ定義

	/// <value>はい選択時イベント</value>
	public NoneArgumentEventData YesEvent { get; private set; }
	
	/// <value>いいえ選択時イベント</value>
	public NoneArgumentEventData NoEvent { get; private set; }

	/// <value><see cref="_data"/></value>
	protected override DropDownData _Data => _data;

	/// <value><see cref="_choices"/></value>
	protected override List<SelectableInformation> _Choices => _choices;


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	protected override void Awake()
	{
		// 継承
		base.Awake();	// 親関数の起動

		// 初期化
		YesEvent = ScriptableObject.CreateInstance<NoneArgumentEventData>();
		NoEvent = ScriptableObject.CreateInstance<NoneArgumentEventData>();
		_choices = new(){
			new SelectableInformation{text = _data.YesText, event_data = YesEvent},	// はいの選択肢
			new SelectableInformation{text = _data.NoText, event_data = NoEvent},	// いいえの選択肢
			};

		// 継承
		base.Awake();	// 親関数の起動
	}
}