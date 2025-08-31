/*=====
<Dungeon.cs>

-author
	mizunose

-about
	ダンジョンを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
/// <summary>
/// <para>ダンジョン</para>
/// </summary>
public class Dungeon : MonoSingleton<Dungeon>
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("データ")] private DungeonData _data;
	private uint _floor_idx = 0;	// 

	// プロパティ定義

	/// <summary>
	/// <para>マップ本体</para>
	/// </summary>
	/// <value>このダンジョンのマップ</value>
	public Map Map { get; private set; }

	/// <summary>
	/// <para>ターン管理本体</para>
	/// </summary>
	/// <value>ダンジョン内のターンシステム</value>
	public DungeonTurnState TurnFlow { get; private set; }
	
	/// <summary>
	/// <para>プレイヤー本体</para>
	/// </summary>
	/// <value>プレイアブルキャラ</value>
	public GameObject Player{get; protected set;}

	/// <summary>
	/// <para>階層データ</para>
	/// </summary>
	/// <value>現階層のデータ</value>
	public DungeonData.FloorData FloorData => _data.FloorDatas[_floor_idx];


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	protected override void Start()
	{
		if(_data)	// ヌルチェック
		{
			
			if (_data.Player)	// ヌルチェック
			{
				Player = Instantiate(_data.Player);	// プレイヤー生成
			}
#if UNITY_EDITOR
			else
			{
				Debug.LogError("生成プレイヤーの情報が設定されていません");
			}
#endif	// end UNITY_EDITOR
		}
#if UNITY_EDITOR
		else
		{
			Debug.LogError("ダンジョンデータが不足しています");
		}
#endif	// end UNITY_EDITOR

#if UNITY_EDITOR
		// 保全
		if (Map)	// ヌルチェック
		{
			Debug.LogError("異常なマップ機能が存在しています");
		}
#endif	// end UNITY_EDITOR

		// 変数宣言
		GameObject _map_object = new();	// マップのインスタンス

		// マップ機能作成
		_map_object.transform.SetParent(transform, false);	// 自身の子に登録
		Map = _map_object.AddComponent<Map>();	// マップ機能付与
#if UNITY_EDITOR
		_map_object.name = "Map";	// デバッグ時にはわかりやすいように命名しておく
#endif	// end UNITY_EDITOR

		// 変数宣言
		GameObject _enemy_spawner_object = new();	// 敵生成のインスタンス

		// 敵生成機能作成
		_enemy_spawner_object.transform.SetParent(transform, false);	// 自身の子に登録
		_enemy_spawner_object.AddComponent<EnemySpawner>();	// 敵生成機能付与
#if UNITY_EDITOR
		_enemy_spawner_object.name = "EnemySpawner";	// デバッグ時にはわかりやすいように命名しておく
#endif	// end UNITY_EDITOR

		// 変数宣言
		GameObject _turn_state = new();	// ターン管理のインスタンス

		// ターン管理機能作成
		TurnFlow = _turn_state.AddComponent<DungeonTurnState>();	// ターン管理機能付与
#if UNITY_EDITOR
			_turn_state.name = "turnState";	// デバッグ時にはわかりやすいように命名しておく
#endif	// end UNITY_EDITOR
	}
}