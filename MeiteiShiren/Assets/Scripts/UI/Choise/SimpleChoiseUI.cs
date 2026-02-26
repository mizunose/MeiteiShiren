/*
<SimpleChoiseUI.cs>

-author
	mizunose

-about
	簡素な選択肢UIを実装
*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>選択肢の雛形</para>
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SimpleChoiseUI : ChoiseUI
{
	// 変数宣言
	[Header("関連リンク")]
	[SerializeField, Tooltip("データ")] private SimpleChoiseUIData _data;
	private SimpleChoiseUIData.Changeables _normal_state;	// 通常状態でのデータ退避領域


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
	public override void Select()
	{
		// 状態変化
		_text_label.color = _data.SelectedState.text_color;	// 対応色に変更
		_text_label.fontSize = _data.SelectedState.font_size;	// 対応サイズに変更
	}


	/// <summary>
	/// <para>非選択状態化</para>
	/// </summary>
	public override void Unselect()
	{
		// 状態変化
		_text_label.color = _normal_state.text_color;	// 対応色に変更
		_text_label.fontSize = _normal_state.font_size;	// 対応サイズに変更
	}


	/// <summary>
	/// <para>決定状態化</para>
	/// </summary>
	public override void Decide()
	{
		// 状態変化
		_text_label.color = _data.DecidedState.text_color;	// 対応色に変更
		_text_label.fontSize = _data.DecidedState.font_size;	// 対応サイズに変更
	}
}