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
	private Map _map = null;
	private uint _floor_idx = 0;

	public Map Map { get { return _map; } }

	public DungeonTurnState TurnFlow { get; private set; }
	
	public GameObject Player{get; protected set;}

	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	protected override void Start()
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

		if(_data != null)
		{
#if UNITY_EDITOR
			// 保全
			if (_map)	// ヌルチェック
			{
				Debug.LogError("異常なマップ機能が存在しています");
			}
#endif	// end UNITY_EDITOR
			
			// 変数宣言
			GameObject _map_object = new();	// マップのインスタンス

			// マップ機能作成
			_map_object.transform.SetParent(transform, false);	// 自身の子に登録
			_map = _map_object.AddComponent<Map>();	// マップ機能付与
			_map.Data = _data.MapDatas[_floor_idx];	// マップにデータ適用
#if UNITY_EDITOR
			_map_object.name = "Map";	// デバッグ時にはわかりやすいように命名しておく
#endif	// end UNITY_EDITOR
		}
		else
		{
			Debug.Log("ダンジョンデータが不足しています");
		}


		GameObject _turn_state = new();
		TurnFlow = _turn_state.AddComponent<DungeonTurnState>();
	}
	

	/// <summary>
	/// <para>更新処理</para>
	/// </summary>
	protected override void Update()
	{
		//TODO:レンダーターゲットに描きこみ
	}
}