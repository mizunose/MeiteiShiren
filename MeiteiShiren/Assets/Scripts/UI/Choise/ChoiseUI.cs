using System;
using TMPro;
using UnityEngine;

// クラス定義
/// <summary>
/// <para>選択肢の雛形</para>
/// </summary>
[RequireComponent(typeof(RectTransform))]
public abstract class ChoiseUI : MonoBehaviour
{
	// 変数宣言
	[SerializeField, Tooltip("テキスト表示領域")] protected TextMeshProUGUI _text_label;

	
	/// <summary>
	/// <para>テキスト文設定</para>
	/// </summary>
	/// <param name="text">表示内容</param>
	public void Print(string text)
	{
		// 更新
		_text_label.text = text;	// テキスト表示変更
	}


	/// <summary>
	/// <para>選択状態化</para>
	/// </summary>
	public abstract void Select();


	/// <summary>
	/// <para>非選択状態化</para>
	/// </summary>
	public abstract void Unselect();


	/// <summary>
	/// <para>決定状態化</para>
	/// </summary>
	public abstract void Decide();
}