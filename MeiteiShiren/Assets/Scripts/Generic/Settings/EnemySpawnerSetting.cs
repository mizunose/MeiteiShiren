/*=====
<EnemySpawnerSetting.cs>

-author
	mizunose

-about
	敵生成のプロパティ値を実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
/// <summary>
/// <para>マップのプロパティ値</para>
/// </summary>
public class EnemySpawnerSetting : CreatableData
{
	// 変数宣言
	[SerializeField, Tooltip("生成ターン周期")] private uint _cycle_interval = 1;
	[SerializeField, Tooltip("最大生成数"), Min(0)] private uint _max_spawn = 0;

	// プロパティ定義

	/// <summary>
	/// <para>生成処理の間隔ターン数</para>
	/// </summary>
	/// <value><see cref="_cycle_interval"/></value>
	public float CycleInterval => _cycle_interval;

	/// <summary>
	/// <para>最大生成数</para>
	/// </summary>
	/// <value><see cref="_max_spawn"/></value>
	public uint MaxSpawn => _max_spawn;
}	