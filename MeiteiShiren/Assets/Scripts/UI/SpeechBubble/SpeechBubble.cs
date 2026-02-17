/*=====
<SpeechBubble.cs>

-author
	mizunose

-about
	吹き出しを実装
=====*/

// 名前空間宣言
using TMPro;
using UnityEngine;

// クラス定義
/// <summary>
/// <para>吹き出しの雛形</para>
/// </summary>
[RequireComponent(typeof(RectTransform))]
public abstract class SpeechBubble : MonoBehaviour
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("キャンバス情報")] private PublicCanvasData _canvas;
	[SerializeField, Tooltip("テキスト表示領域")] private TextMeshProUGUI _text_label;



	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Awake()
	{
		// 初期化
		transform.SetParent(_canvas.Instance.transform , false);	// 親子付け
	}


	/// <summary>
	/// <para>更新処理</para>
	/// </summary>
	private void Update()
	{
		// 更新
		//TODO:カーソル明滅処理
	}


	/// <summary>
	/// <para>テキスト文設定</para>
	/// </summary>
	/// <param name="text">表示内容</param>
	/// <param name="target">表示位置基準</param>
	public void SetValue(string text)
	{
		// 初期化
		_text_label.text = text;	// テキスト表示変更
	}
}