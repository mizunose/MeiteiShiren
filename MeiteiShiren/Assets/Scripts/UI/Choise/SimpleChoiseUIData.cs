/*
<SimpleChoiseUIData.cs>

-author
	mizunose

-about
	簡素な選択肢UIのデータを実装
*/

// 名前空間宣言
using System;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>簡素な選択肢UIのデータ</para>
/// </summary>
public class SimpleChoiseUIData : CreatableData
{
	// 構造体定義
	/// <summary>
	/// <para>選択状態によって変わりうるデータ</para>
	/// </summary>
	[Serializable]
	public struct Changeables
	{
		// 変数宣言
		public Color text_color;	// テキスト表示色
		public float font_size;	// フォントサイズ
	}

	// 変数宣言
	[SerializeField, Tooltip("選択時のデータ")] private Changeables _selected_state;
	[SerializeField, Tooltip("決定時のデータ")] private Changeables _decided_state;

	// プロパティ定義

	/// <value><see cref="_selected_state"/></value>
	public Changeables SelectedState => _selected_state;

	/// <value><see cref="_decided_state"/></value>
	public Changeables DecidedState => _decided_state;
}