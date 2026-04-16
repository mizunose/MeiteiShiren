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
	[SerializeField, Tooltip("与える効果")] protected Affects _affects;

	// プロパティ定義

	/// <value><see cref="_affects"/></value>
	public Affects Affects => _affects;
}