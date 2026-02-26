/*=====
<Dungeon.cs>

-author
	mizunose

-about
	ダンジョンを実装
=====*/

// 名前空間宣言
using UnityEngine;
using System.Collections;

// クラス定義

/// <summary>
/// <para>ダンジョン</para>
/// </summary>
[DisallowMultipleComponent]
public class Dungeon : MonoSingleton<Dungeon>
{
	// 定数定義
	private const string _INSTANCE_NAME = "Dungeon";	// 自動生成された時のインスタンス名

	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("データ")] private DungeonData _data;
	private uint _floor_idx = 0;	// 階層番号
	private EnemySpawner _enemy_spawner;	//敵生成機

	// プロパティ定義

#if UNITY_EDITOR
	/// <value><see cref="_INSTANCE_NAME"/></value>
	protected override string InstanceName => _INSTANCE_NAME;
#endif	// end UNITY_EDITOR

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
		// 初期化
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

		// 階層生成
		CreateFloor();	// 階層情報の初期化
	}

	
	/// <summary>
	/// <para>階層移動の起動</para>
	/// </summary>
	public void BootSwitchFloor()
	{
		StartCoroutine(SwitchFloor());	// 階層切り替え
	}


	/// <summary>
	/// <para>階層移動処理</para>
	/// </summary>
	/// <param name="transitions">遷移演出データ</param>
	/// <returns>遅延処理用のインターフェース体</returns>
	private IEnumerator SwitchFloor()
	{
		// 階層移動
		if(_floor_idx < _data.FloorDatas.Length - 1)	// 次階層が存在
		{
			_floor_idx++;	// 次階層へ
		}
		else	// 最上階を超えた
		{
			// クリア処理
			SceneLoader.Instance.BootChangeScene(_data.NextScene, _data.ClearedTransitions);	// シーン切り替え
			yield break;	// 処理終了
		}

		// 遷移開始
		if (_data.FloorTransitions?.OutData)	// ヌルチェック
		{
			yield return _data.FloorTransitions.OutData.Act();	// 画面転換アニメーション
		}

		// 変数宣言
		Coroutine _coroutine = null;	// 待機画面の描画スレッド
		
		// 待機開始
		if (_data.FloorTransitions?.WaitData) // ヌルチェック
		{
			_coroutine = StartCoroutine(_data.FloorTransitions.WaitData.Act());	// 待機画面表示
		}

		// 階層クリア
		Destroy(TurnFlow.gameObject);
		Destroy(Map.gameObject);	// 現階層のマップを破棄
		Destroy(_enemy_spawner.gameObject);	// 現階層の敵生成を破棄

		// 階層生成
		CreateFloor();	// 階層情報の初期化

		// 待機終了
		if (_coroutine != null)	// ヌルチェック
		{
			StopCoroutine(_coroutine);	// コルーチンを止める
		}

		// 遷移再開
		if (_data.FloorTransitions?.InData)	// ヌルチェック
		{
			yield return _data.FloorTransitions?.InData.Act();	// 画面転換アニメーション
		}

		// 遷移完了
		yield break;	// 処理終了
	}


	/// <summary>
	/// <para>階層情報生成処理</para>
	/// </summary>
	private void CreateFloor()
	{
		// 変数宣言
		GameObject _turn_state = new();	// ターン管理のインスタンス

		// ターン管理機能作成
		_turn_state.transform.SetParent(transform, false);	// 自身の子に登録
		TurnFlow = _turn_state.AddComponent<DungeonTurnState>();	// ターン管理機能付与
#if UNITY_EDITOR
		_turn_state.name = "turnState";	// デバッグ時にはわかりやすいように命名しておく
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
		_enemy_spawner = _enemy_spawner_object.AddComponent<EnemySpawner>();	// 敵生成機能付与
#if UNITY_EDITOR
		_enemy_spawner_object.name = "EnemySpawner";	// デバッグ時にはわかりやすいように命名しておく
#endif	// end UNITY_EDITOR
	}
}