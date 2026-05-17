/*
<ItemData.cs>

-author
	mizunose

-about
	突進アイテムのデータ
*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>突進アイテムのデータ</para>
/// </summary>
public class RushItemData : ItemData
{
	// 変数宣言
	[SerializeField, Tooltip("貫通性	※trueで壁以外を貫通する")] private bool _is_penetrating;


	// プロパティ定義

	/// <value><see cref="_is_penetrating"/></value>
	public bool IsPenetrating => _is_penetrating;
}