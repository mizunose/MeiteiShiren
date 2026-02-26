/*=====
<EnemySpawner.cs>

-author
	mizunose

-about
	敵生成
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>敵生成管理</para>
/// </summary>
[DisallowMultipleComponent]
public class EnemySpawner : MonoBehaviour
{
	// 変数宣言
	uint _turn_count = 0;	// 生成待機カウント


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Start()
	{
		// イベント接続
		Dungeon.Instance.TurnFlow.OnTurnChanged += OnTurnChanged;	// ターン終了時処理を接続
	}


	/// <summary>
	/// <para>ターン終了時処理</para>
	/// </summary>
	private void OnTurnChanged()
	{
		// カウント
		_turn_count++;	// ターン経過を記録

		// 生成
		if (_turn_count > Settings.Instance.EnemySpawner.CycleInterval)	// 必要な経過ターンが経った
		{
			_turn_count = 0;	// カウント初期化
			Dungeon.Instance.FloorData.EnemySpawnData.Spwan();	// 生成処理
		}
	}
}