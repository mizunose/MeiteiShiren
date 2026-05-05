/*
<ChoiseUI.cs>

-author
	mizunose

-about
	選択肢のUIを定義
*/

// 名前空間宣言
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// クラス定義

/// <summary>
/// <para>選択肢</para>
/// </summary>
[RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
public class ChoiseUI : MonoBehaviour
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("データ")] private ChoiseUIData _data;
	[Header("関連リンク")]
	[SerializeField, Tooltip("テキスト表示領域")] protected TextMeshProUGUI _text_label;
	[SerializeField, Tooltip("アイコン")] protected Image _icon;
	private ChoiseUIData.Changeables _normal_state;	// 通常状態でのデータ退避領域


	/// <summary>
	/// <para>テキスト文設定</para>
	/// </summary>
	/// <param name="content">表示内容</param>
	public void Print(ChoiseUIValue content)
	{
		// 更新
		if (_text_label != null && content.text != null)	// ヌルチェック
		{
			_text_label.text = content.text;	// テキスト表示変更
		}
		if (_icon != null && content.icon != null)	// ヌルチェック
		{
			_icon.sprite = content.icon;	// アイコン表示変更
		}
	}


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Awake()
	{
		// 初期化
		_normal_state.text_color = _text_label.color;	// 色情報を保存
		_normal_state.font_size = _text_label.fontSize;	// フォントサイズを保存
	}


	/// <summary>
	/// <para>選択状態化</para>
	/// </summary>
	public void Select()
	{
		// 状態変化
		_text_label.color = _data.SelectedState.text_color;	// 対応色に変更
		_text_label.fontSize = _data.SelectedState.font_size;	// 対応サイズに変更
	}


	/// <summary>
	/// <para>非選択状態化</para>
	/// </summary>
	public void Unselect()
	{
		// 状態変化
		_text_label.color = _normal_state.text_color;	// 対応色に変更
		_text_label.fontSize = _normal_state.font_size;	// 対応サイズに変更
	}


	/// <summary>
	/// <para>決定状態化</para>
	/// </summary>
	public void Decide()
	{
		// 状態変化
		_text_label.color = _data.DecidedState.text_color;	// 対応色に変更
		_text_label.fontSize = _data.DecidedState.font_size;	// 対応サイズに変更
	}
}