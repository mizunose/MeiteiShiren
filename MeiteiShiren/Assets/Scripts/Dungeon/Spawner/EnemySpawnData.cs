/*=====
<EnemySpawnData.cs>

-author
	mizunose

-about
	敵生成データを実装
=====*/

// 名前空間宣言
using System.Collections.Generic;
using UnityEngine;

// クラス定義
/// <summary>
/// <para>敵生成データ</para>
/// </summary>
public class EnemySpawnData : CreatableData
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("生成対象")] private GameObject[] _enemies;
	private List<GameObject> _spawned = new();	// 生成物一覧


	/// <summary>
	/// <para>敵生成処理</para>
	/// </summary>
	public void Spwan()
	{
		// 保全
		if (_enemies.Length == 0)	// 生成候補がない
		{
			return;	// 生成できないので終了
		}
		if (_spawned == null)	// 生成物の格納場所がない
		{
			_spawned = new();	// 領域確保
		}

		// 生成数管理
		_spawned.RemoveAll(_object => _object == null);	// 消えたものを監視から除外
		if (_spawned.Count >= Settings.Instance.EnemySpawner.MaxSpawn)	// 生成上限をオーバーする
		{
			return;	// 生成上限を守り終了
		}

		// 変数宣言
		var _masses = Dungeon.Instance.FloorData.MapData.MainContact.GetComponentsInChildren<Mass>();	// 主連続領域のマス
		List<Transform> _masses_transform = new();	// 生成位置の候補一覧

		// 初期化
		for (int _mass_idx = 0; _mass_idx < _masses.Length; _mass_idx++)	// マス単位でのループ
		{
			if (_masses[_mass_idx].transform.childCount == 0)	// 生成物配置可能
			{
				_masses_transform.Add(_masses[_mass_idx].transform);	// 生成位置候補として登録
			}
		}

		// 保全
		if (_masses_transform.Count == 0)	// 生成位置の候補がない
		{
			// 終了
			return;	// 生成できないので終了
		}
		
		// 変数宣言
		var _enemy = Instantiate(_enemies[UnityEngine.Random.Range(0, _enemies.Length)]);	// 生成インスタンス

		// 場所を決定
		_enemy.transform.SetParent(_masses_transform[UnityEngine.Random.Range(0, _masses_transform.Count)], false);	// 生成位置を選択

		// リスト更新
		_spawned.Add(_enemy);	// 生成物を監視

		// ターン制に紐づけ
		Dungeon.Instance.TurnFlow.AddActor(_enemy);	// 行動をターン管理に任せる
	}
}