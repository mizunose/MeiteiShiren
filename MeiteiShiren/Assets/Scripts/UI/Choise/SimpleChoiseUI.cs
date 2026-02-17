using System;
using TMPro;
using UnityEngine;

// クラス定義
/// <summary>
/// <para>選択肢の雛形</para>
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SimpleChoiseUI : ChoiseUI
{
	// 構造体定義
	/// <summary>
	/// <para>選択状態によって変わりうるデータ</para>
	/// </summary>
	[Serializable]
	private struct SimpleChoiseData
	{
		// 変数宣言
		public Color text_color;	// テキスト表示色
		public float font_size;	// フォントサイズ
	}

	// 変数宣言
	private SimpleChoiseData _normal_data;
	[SerializeField, Tooltip("選択時のデータ")] private SimpleChoiseData _selected_data;
	[SerializeField, Tooltip("決定時のデータ")] private SimpleChoiseData _decided_data;


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Awake()
	{
		// 初期化
		_normal_data.text_color = _text_label.color;
		_normal_data.font_size = _text_label.fontSize;
	}


	/// <summary>
	/// <para>選択状態化</para>
	/// </summary>
	public override void Select()
	{
		_text_label.color = _selected_data.text_color;
		_text_label.fontSize = _selected_data.font_size;
	}


	/// <summary>
	/// <para>非選択状態化</para>
	/// </summary>
	public override void Unselect()
	{
		_text_label.color = _normal_data.text_color;
		_text_label.fontSize = _normal_data.font_size;
	}


	/// <summary>
	/// <para>決定状態化</para>
	/// </summary>
	public override void Decide()
	{
		
		_text_label.color = _decided_data.text_color;
		_text_label.fontSize = _decided_data.font_size;
	}
}