/*=====
<StaticMap.cs>

-author
	mizunose

-about
	静的マップ(事前に定まっているマップ)のデータを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
/// <summary>
/// <para>固定生成マップデータ</para>
/// </summary>
class StaticMap : MapData
{
	// プロパティ定義

	/// <summary>
	/// <para>マップ全体のサイズ</para>
	/// </summary>
	/// <value>周囲の壁も含めたマップ全体のサイズ</value>
	public override Vector2Int MapSize
	{
		get
		{
			// 提供
			return _size;	// 仮置き
		}
	}

	
	/// <summary>
	/// <para>マップ生成処理</para>
	/// </summary>
	public override void Generate()
	{
	}
}