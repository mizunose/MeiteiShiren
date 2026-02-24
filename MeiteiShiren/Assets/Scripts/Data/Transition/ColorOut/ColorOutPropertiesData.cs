/*
<ColorOutPropertiesData.cs>

-author
	mizunose

-about
	固定色で画面全体を塗りつぶすトランジションのプロパティデータを実装
*/

// 名前空間宣言
using UnityEngine;

// クラス定義
public class ColorOutPropertiesData : TransitionPropertiesData
{
	// 定数定義
	public static readonly int _CURTAIN_COLOR_ID = Shader.PropertyToID("curtain_color");	// 塗りつぶし色パラメータID
	public static readonly int _DRAW_ALPHA_ID = Shader.PropertyToID("draw_alpha");	// 描画不透明度パラメータID

	// 変数宣言
	[SerializeField, Tooltip("色")] private Color _curtain_color = Color.black;

	// プロパティ定義

	/// <value><see cref="_curtain_color"/></value>
	public Color CurtainColor => _curtain_color;
}