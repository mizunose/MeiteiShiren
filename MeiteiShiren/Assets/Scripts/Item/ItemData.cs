/*
<ItemData.cs>

-author
	mizunose

-about
	アイテムのデータ
*/

// 名前空間宣言
using System;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>アイテムのデータ</para>
/// </summary>
public class ItemData : CreatableData
{
	// 変数宣言
	[SerializeField, Tooltip("名称")] private string _name;
	[SerializeField, Tooltip("与える効果")] private Affects _affects;
	[SerializeField, Tooltip("再利用性	※falseで使い捨て")] private bool _reusability = false;
	[SerializeField, Tooltip("説明文")] private string _description;


	// プロパティ定義

	/// <value><see cref="_affects"/></value>
	public Affects Affects => _affects;

	/// <value><see cref="_name"/></value>
	public string Name => _name;

	/// <value><see cref="_reusability"/></value>
	public bool Reusability => _reusability;

	/// <value><see cref="_description"/></value>
	public string Description => _description;
}