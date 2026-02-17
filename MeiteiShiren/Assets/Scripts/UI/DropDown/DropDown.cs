/*=====
<DropDown.cs>

-author
	mizunose

-about
	ドロップダウン式の選択UIを定義
=====*/

// 名前空間宣言
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// クラス定義
/// <summary>
/// <para>ドロップダウン選択肢</para>
/// </summary>
[RequireComponent(typeof(VerticalLayoutGroup))]
public abstract class DropDown : VirtualizeMono
{
	// 構造体定義
	/// <summary>
	/// <para>選択候補</para>
	/// </summary>
	[Serializable]
	public class SelectableInformation
	{
		// 変数宣言
		[SerializeField, Tooltip("表示")] public string text;
		[SerializeField, Tooltip("イベント")] public NoneArgumentEventData event_data;
	}

	// 変数宣言
	[Header("ステータス")]
	private int _selected_index;	// 選択番号
	//private List<TextMeshProUGUI> _text_labels = new();	// テキスト
	private List<ChoiseUI> _text_labels = new();	// テキスト

	// プロパティ定義

	/// <value>データ</value>
	protected abstract DropDownData _Data { get; }

	/// <value>選択肢</value>
	protected abstract SelectableInformation[] _Choices { get; }


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	protected override sealed void Awake()
	{
		// 継承
		CustomAwake();	// 子クラスの初期化を起動

		// 入力管理
		IngameInputManager.Instance.TrendDisable();	// インゲーム入力無効化
		UIInputManager.Instance.DropDown.TrendEnable();	// ドロップダウン入力有効化

		// 初期化
		transform.SetParent(_Data.Canvas.Instance.transform, false);	// 親子付け

		// テキスト表示
		foreach (var choice in _Choices)	// 選択候補単位でのループ
		{
//			// 変数宣言
//			GameObject _text_object = new();	// テキストのインスタンス
//			var _text_label = _text_object.AddComponent<TextMeshProUGUI>();	// テキスト表示機能

//			// 初期化
//			_text_labels.Add(_text_label);	// リスト登録
//			_text_object.transform.SetParent(transform, false);	// 親子付け
//			if (_data.Font)	// ヌルチェック
//			{
//				_text_label.font = _data.Font;	// フォント種
//			}
//#if UNITY_EDITOR
//			else
//			{
//				Debug.LogError("表示フォントを設定してください");
//			}
//#endif	// end UNITY_EDITOR
//			_text_label.fontSize = _data.FontSize;	// フォントの大きさ
//			_text_label.alignment = TextAlignmentOptions.Center;	// 中央揃え
//			_text_label.text = choice.text;	// 表示文
//#if UNITY_EDITOR
//			_text_object.name = "DropDownText";	// デバッグ時にはわかりやすいように命名しておく
//#endif	// end UNITY_EDITOR

			ChoiseUI _text_object = Instantiate(_Data.ChoiseUI);
			_text_labels.Add(_text_object);	// リスト登録
			_text_object.transform.SetParent(transform, false);	// 親子付け
			_text_object.Print(choice.text);
		}

		// 初期化
		//_text_labels[_selected_index].color = Color.red;
	}


	/// <summary>
	/// <para>更新処理</para>
	/// </summary>
	protected override sealed void Update()
	{
		// 入力受付
		if (UIInputManager.Instance.DropDown.Up.BaseOne.triggered)	// 上移動の入力
		{
			// カーソル操作
			MoveCursor(true);	// 上移動
		}
		if (UIInputManager.Instance.DropDown.Down.BaseOne.triggered)	// 下移動の入力
		{
			// カーソル操作
			MoveCursor(false);	// 下移動
		}
		if (UIInputManager.Instance.DropDown.Decide.BaseOne.triggered)	// 決定の入力があった
		{
			// 決定
			//_text_labels[_selected_index].color = Color.white;
			_Choices[_selected_index].event_data.Invoke();	// 採択を通知
		}
	}


	/// <summary>
	/// <para>選択カーソルの移動処理</para>
	/// </summary>
	/// <param name="is_positive">正方向か否か。trueで正(↑)。</param>
	private void MoveCursor(bool is_positive)
	{
		// 変数宣言
		var _past_text = _text_labels[_selected_index];
		//_past_text.color = Color.white;
		// カーソル移動
		if (is_positive)	// 上移動の入力
		{
			if (_selected_index > 0)	// 上に選択肢がある
			{
				_selected_index--;	// 一つ上に選択を切り替え
			}
			else	// 最上を選択中
			{
				_selected_index = _Choices.Length - 1;	//最下に選択を切り替え
			}
		}
		else	// 下移動の入力
		{
			if (_selected_index < _Choices.Length - 1)	// 下に選択肢がある
			{
				_selected_index++;	// 一つ下に選択を切り替え
			}
			else	// 最下を選択中
			{
				_selected_index = 0;	//最上に選択を切り替え
			}
		}
		_past_text.Unselect();
		_text_labels[_selected_index].Select();
		//_text_labels[_selected_index].color = Color.red;
		//Destroy(this.gameObject);	// 自壊
	}
}