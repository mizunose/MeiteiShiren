/*=====
<EnemySpawnData.cs>

-author
	mizunose

-about
	敵生成データを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>敵生成データ</para>
/// </summary>
public class EnemySpawnData : CreatableData
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("生成対象")] private WeightedRandom<GameObject> _enemies = new();

	// プロパティ定義

	/// <value><see cref="_enemies"/></value>
	public WeightedRandom<GameObject> Enemies => _enemies;
}