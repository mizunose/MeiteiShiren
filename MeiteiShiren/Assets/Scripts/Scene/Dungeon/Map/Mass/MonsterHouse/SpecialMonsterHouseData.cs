/*=====
<SpecialMonsterHouseData.cs>

-author
	mizunose

-about
	特殊モンスターハウスのデータを実装
=====*/

// 名前空間宣言
using System.Collections.Generic;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>特殊モンスターハウスデータ</para>
/// </summary>
public class SpecialMonsterHouseData : CreatableData
{
	// 変数宣言
	[SerializeField, Tooltip("特殊生成対象")] private WeightedRandom<GameObject> _spawnable_enemies = new();


	// プロパティ定義

	/// <value><see cref="_spawnable_enemies"/></value>
	public WeightedRandom<GameObject> SpawnableEnemies => _spawnable_enemies;
}
