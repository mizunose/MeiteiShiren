/*=====
<SpeechBubbleData.cs>

-author
	mizunose

-about
	吹き出しのデータを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
public class SpeechBubbleData : CreatableData
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("キャンバス情報")] private PublicCanvasData _canvas_data;
	[Header("余白設定")]
	[SerializeField, Tooltip("左の余白率"), Range(0.0f, 1.0f)] private float _left_margin_rate = 0.0f;
	[SerializeField, Tooltip("右の余白率"), Range(0.0f, 1.0f)] private float _right_margin_rate = 0.0f;
	[SerializeField, Tooltip("上部の余白率"), Range(0.0f, 1.0f)] private float _top_margin_rate = 0.3f;
	[SerializeField, Tooltip("下部の余白率"), Range(0.0f, 1.0f)] private float _bottom_margin_rate = 0.0f;

	// プロパティ定義

	/// <value><see cref="_canvas_data"/></value>
	public PublicCanvasData CanvasData => _canvas_data;

	/// <value><see cref="_left_margin_rate"/></value>
	public float LeftMarginRate => _left_margin_rate;

	/// <value><see cref="_right_margin_rate"/></value>
	public float RightMarginRate => _right_margin_rate;

	/// <value><see cref="_top_margin_rate"/></value>
	public float TopMarginRate => _top_margin_rate;

	/// <value><see cref="_bottom_margin_rate"/></value>
	public float BottomMarginRate => _bottom_margin_rate;
}