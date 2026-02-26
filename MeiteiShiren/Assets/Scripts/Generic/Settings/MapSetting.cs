/*=====
<MapSetting.cs>

-author
	mizunose

-about
	マップのプロパティ値を実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>マップのプロパティ値</para>
/// </summary>
public class MapSetting : CreatableData
{
	// 変数宣言
	[SerializeField, Tooltip("1マスあたりの大きさ")] private float _mass_size = 1.0f;
	[SerializeField, Tooltip("ミニマップ表示透明度"), Range(0.0f, 1.0f)] private float _alpha = 0.21f;

	// プロパティ定義

	/// <value><see cref="_mass_size"/></value>
	public float MassSize => _mass_size;

	/// <value><see cref="_alpha"/></value>
	public float Alpha => _alpha;
}