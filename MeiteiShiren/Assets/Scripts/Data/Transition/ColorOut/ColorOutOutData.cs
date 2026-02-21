/*
<ColorOutOutData.cs>

-author
	mizunose

-about
	固定色で画面全体を塗りつぶすトランジションの入りデータを実装
*/

// 名前空間宣言
using UnityEngine;

// クラス定義
[CreateAssetMenu(menuName = _MENU_TAB_NAME + _NAME, fileName = _NAME)]
public class ColorOutOutData : TransitionOutData
{
	// 定数定義
	private const string _NAME = "ColorOutOut";	// アセット名

	// 変数宣言
	[SerializeField, Tooltip("プロパティデータ")] protected ColorOutPropertiesData _properties_data;

	// プロパティ定義

	/// <value><see cref="_properties_data"/></value>
	protected override TransitionPropertiesData _PropertiesData => _properties_data;


	/// <summary>
	/// <para>遷移処理の入り部</para>
	/// </summary>
	/// <param name="material">適用済マテリアル</param>
	/// <param name="time_rate">演出時間の進捗率</param>
	/// <returns>遅延処理用のインターフェース体</returns>
	protected override void _Out(Material material, float time_rate)
	{
		// プロパティ値更新
		material.SetColor(ColorOutPropertiesData._CURTAIN_COLOR_ID, _properties_data.CurtainColor);	// 幕色更新
		material.SetFloat(ColorOutPropertiesData._DRAW_ALPHA_ID, Mathf.Lerp(0.0f, 1.0f, time_rate));	// 不透明度更新
	}
}