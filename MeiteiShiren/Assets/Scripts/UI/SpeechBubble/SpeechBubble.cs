/*=====
<SpeechBubble.cs>

-author
	mizunose

-about
	吹き出しを定義
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
	[Header("関連リンク")]
	[SerializeField, Tooltip("テキスト表示領域")] private TextMeshProUGUI _text_label;
	[SerializeField, Tooltip("データ")] private SpeechBubbleData _data;


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Awake()
	{
		// 変数宣言
		var _rect_transform = GetComponent<RectTransform>();	// 矩形情報

		// 初期化
		transform.SetParent(_data.CanvasData.Instance.transform , false);	// 親子付け
			// アンカー設定(ストレッチ)
			_rect_transform.anchorMin = Vector2.zero;	// アンカー最小端
			_rect_transform.anchorMax = Vector2.one;	// アンカー最大端
			// 領域設定
			_rect_transform.offsetMin = new Vector2(_data.CanvasData.VirtualSize.x * _data.LeftMarginRate, _data.CanvasData.VirtualSize.y * _data.BottomMarginRate);	// 最小端設定
			_rect_transform.offsetMax = new Vector2(-_data.CanvasData.VirtualSize.x * _data.RightMarginRate, -_data.CanvasData.VirtualSize.y * _data.TopMarginRate);	// 最大端設定
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