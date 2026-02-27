/*=====
<Mass.cs>

-author
	mizunose

-about
	マス目を実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>マップを構成するマス。子に管理物(乗っているもの)を持つ想定。また、親が構成物(マップや部屋など)である想定。</para>
/// </summary>
[DisallowMultipleComponent]
public class Mass : VirtualizeMono
{
	// クラス定義
	private class AboveObject<T> where T : Object
	{
		// 変数宣言
		private T _instance = null;	// インスタンス

		// プロパティ定義
		public T Instance => _instance;

		/// <value>処理のターゲットにしたか。処理済みならtrue, そうでなければfalse</value>
		public bool IsTargeted { get; set; } = false;


		/// <summary>
		/// <para>コンストラクタ</para>
		/// </summary>
		/// <param name="instance"></param>
		public AboveObject(T instance)
		{
			// 初期化
			_instance = instance;	// インスタンス設定
		}
	}

	// 定数定義
	private static readonly int[] _INDICES = {	// マスメッシュの頂点インデックス
			0, 1, 2,	// 左側三角形
			1, 3, 2,	// 右側三角形
		};
	private static readonly Vector2[] _UVS = {	// マスメッシュのテクスチャ座標
			new Vector2(0.0f, 0.0f),	// 左上
			new Vector2(1.0f, 0.0f),	// 右上
			new Vector2(0.0f, 1.0f),	// 左下
			new Vector2(1.0f, 1.0f),	// 右下
		};

	// 変数宣言
	private AboveObject<Item> _above_item = null;	// 所持しているアイテム
	private AboveObject<GameObject> _above_character = null;	// 乗せたキャラクター
	private Inventory _character_inventory = null;	// キャラクターが持っているインベントリ

	// プロパティ定義

	/// <value>現在シーンがダンジョンならインスタンスを取得</value>
	protected Dungeon DungeonScene => SceneLoader.Instance.CurrentScene as Dungeon;

	/// <value>キャラクタのインスタンス</value>
	protected GameObject AboveCharacter => _above_character.Instance;


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	protected override sealed void Start()
	{
		// 代替処理
		CustomStart();	// 子クラスの初期化処理

		// 初期化
		Visualize();	// 視覚化

		// イベント接続
		DungeonScene.TurnFlow.OnMassAction += TurnedAction;	// 行動指示時処理を接続
	}


	/// <summary>
	/// <para>視覚化処理</para>
	/// </summary>
	protected virtual void Visualize()
	{
		// 変数宣言
		Mesh _mesh = new Mesh();	//メッシュ本体
		var _mesh_filter = gameObject.AddComponent<MeshFilter>();	// メッシュ管理機能

		// メッシュ作成
		gameObject.AddComponent<MeshRenderer>().material = DungeonScene.FloorData.MapData.GroundTexture;	// メッシュの描画機能を追加し、その参照マテリアルをマップに合わせて変更
		_mesh.vertices = Settings.Instance.Map.MassVertices;	// メッシュの頂点情報を設定
		_mesh.triangles = _INDICES;	// メッシュの頂点インデックスを設定
		_mesh.RecalculateNormals();	// 法線を再計算
		_mesh.uv = _UVS;	// テクスチャ座標を設定
		_mesh_filter.sharedMesh = _mesh;	// 作成したメッシュを登録
	}


	/// <summary>
	/// <para>ターン制行動</para>
	/// </summary>
	private void TurnedAction()
	{
		// アイテム回収
		if (_character_inventory)	// インベントリを持っている
		{
			if (_character_inventory.AddItem(_above_item.Instance))	// インベントリに登録
			{
				_above_item = null;	// アイテムを託したため管理から外す
			}
		}

		// プレイヤーに対する処理
		for (int _child_idx = 0; _child_idx < transform.childCount; _child_idx++)	// 子オブジェクト単位でのループ
		{
			// 変数宣言
			var _child = transform.GetChild(_child_idx);	// 扱う子オブジェクト

			// 検査
			if (_above_character?.Instance && !_above_character.IsTargeted)	// 未処理
			{
				Boot();	// 搭乗時は自動で起動する
				_above_character.IsTargeted = true;	// 処理した
			}
		}
	}


	/// <summary>
	/// <para>アイテム追加</para>
	/// </summary>
	/// <returns>処理の成否(追加に失敗するとfalse)</returns>
	public bool AddItem(Item item)
	{
		// すでにアイテムがある場合受け入れない
		if (_above_item?.Instance)	// ヌルチェック
		{
			// 提供
			return false;	// 処理失敗
		}

		// アイテム受理
		_above_item = new (item);	// アイテム登録
		item.transform.SetParent(transform, false);	// 自身の子に登録

		// 提供
		return true;	// 処理成功
	}


	/// <summary>
	/// <para>キャラクタ追加</para>
	/// </summary>
	/// <returns>処理の成否(追加に失敗するとfalse)</returns>
	public bool AddCharacter(GameObject character)
	{
		// すでにキャラクタがいる場合受け入れない
		if (_above_character?.Instance)	// ヌルチェック
		{
			// 提供
			return false;	// 処理失敗
		}

		// 更新
		_above_character = new (character);	// キャラクタ登録
		character.transform.SetParent(transform, false);	// 自身の子に登録
		_character_inventory = character.GetComponent<Inventory>();	// インベントリ部分をキャッシュ

		// 提供
		return true;	// 処理成功
	}


	/// <summary>
	/// <para>キャラクタ移動</para>
	/// </summary>
	/// <returns>処理の成否(移動に失敗するとfalse)</returns>
	public bool MoveCharacter(Mass next_move)
	{
		// 保全
		if (!_above_character?.Instance)	// 移すキャラクタがいない
		{
			// 提供
			return false;	// 処理失敗
		}
		
		// 更新
		if (next_move.AddCharacter(_above_character.Instance))
		{
			_above_character = null;	// キャラクタを移したため管理から外す
			_character_inventory = null;	// インベントリ部分もクリア
		}
		else
		{
			// 提供
			return false;	// 処理失敗
		}

		// 提供
		return true;	// 処理成功
	}


	/// <summary>
	/// <para>機能を起動</para>
	/// </summary>
	public virtual void Boot()
	{
	}
}