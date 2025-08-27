/*=====
<DungeonData.cs>

-author
	mizunose

-about
	ダンジョンのデータを実装
=====*/

// 名前空間宣言
using System;
using UnityEngine;

// クラス定義
/// <summary>
/// <para>ダンジョンデータ</para>
/// </summary>
[CreateAssetMenu(menuName = _NAME, fileName = _NAME)]
public class DungeonData : ScriptableObject
{
	// 構造体定義
	/// <summary>
	/// <para>階層データ</para>
	/// </summary>
	[Serializable]
	public class FloorData
	{
		// 変数宣言
		[Header("データ")]
		[SerializeField, Tooltip("マップ")] private MapData _map_data;
		[SerializeField, Tooltip("敵生成")] private EnemySpawnData _enemy_spawn_data;

		// プロパティ定義

		/// <summary>
		/// <para>マップデータ</para>
		/// </summary>
		/// <value><see cref="_map_data"/></value>
		public MapData MapData =>_map_data;

		/// <summary>
		/// <para>敵生成データ</para>
		/// </summary>
		/// <value><see cref="_enemy_spawn_data"/></value>
		public EnemySpawnData EnemySpawnData => _enemy_spawn_data;
	}

	// 定数定義
	private const string _NAME = "Dungeon";	// タブ名称

	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("階層")] private FloorData[] _floor_datas;
	[SerializeField, Tooltip("操作キャラ")] private GameObject _player;	//TODO:チーム配置

	// プロパティ定義

	/// <summary>
	/// <para>階層データ</para>
	/// </summary>
	/// <value><see cref="_floor_datas"/></value>
	public FloorData[] FloorDatas => _floor_datas;

	/// <summary>
	/// <para>操作キャラ</para>
	/// </summary>
	/// <value><see cref="_player"/></value>
	public GameObject Player => _player;
}