/*=====
<StairData.cs>

-author
	mizunose

-about
	店のデータを実装
=====*/

// 名前空間宣言
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>店データ</para>
/// </summary>
public class ShopData : CreatableData
{
	// 変数宣言
	[SerializeField, Tooltip("商品")] private WeightedRandom<Item> _goods = new();
	[SerializeField, Tooltip("商品配置数最低値"), Min(0)] private int _min_set_goods;
	[SerializeField, Tooltip("商品配置数猶予(最低値に加えていくつまで配置して良いか)"), Min(0)] private int _margin_set_goods;
	[SerializeField, Tooltip("商品を配置しない外周幅"), Min(0)] private int _unsell_margin;

	// プロパティ定義

	/// <value><see cref="_goods"/></value>
	public WeightedRandom<Item> Goods => _goods;

	/// <value><see cref="_min_set_goods"/></value>
	public int MinSetGoods => _min_set_goods;

	/// <value><see cref="_margin_set_goods"/></value>
	public int MarginSetGoods => _margin_set_goods;

	/// <value><see cref="_unsell_margin"/></value>
	public int UnsellMargin => _unsell_margin;
}