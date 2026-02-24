/*
<ColorOutInData.cs>

-author
	mizunose

-about
	固定色で画面全体を塗りつぶすトランジションの抜けデータを実装
*/

// 名前空間宣言
using UnityEngine;

// クラス定義
public class ColorOutInData : TransitionInData
{
	// 変数宣言
	[SerializeField, Tooltip("プロパティデータ")] protected ColorOutPropertiesData _properties_data;

	// プロパティ定義

	/// <value><see cref="_properties_data"/></value>
	protected override TransitionPropertiesData _PropertiesData => _properties_data;


	/// <summary>
	/// <para>遷移処理の抜け部</para>
	/// </summary>
	/// <param name="material">適用済マテリアル</param>
	/// <param name="time_rate">演出時間の進捗率</param>
	/// <returns>遅延処理用のインターフェース体</returns>
	protected override void _In(Material material, float time_rate)
	{
		// プロパティ値更新
		material.SetColor(ColorOutPropertiesData._CURTAIN_COLOR_ID, _properties_data.CurtainColor);	// 幕色更新
		material.SetFloat(ColorOutPropertiesData._DRAW_ALPHA_ID, Mathf.Lerp(1.0f, 0.0f, time_rate));	// 不透明度更新
	}
}