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

	/// <summary>
	/// <para>マスの大きさ</para>
	/// </summary>
	/// <value><see cref="_mass_size"/></value>
	public float MassSize
	{
		get
		{
			// 提供
			return _mass_size;	// マスの大きさ
		}
	}

	/// <summary>
	/// <para>ミニマップ透明度</para>
	/// </summary>
	/// <value><see cref="_alpha"/></value>
	public float Alpha
	{
		get
		{
			// 提供
			return _alpha;	// ミニマップの透明度
		}
	}
}	