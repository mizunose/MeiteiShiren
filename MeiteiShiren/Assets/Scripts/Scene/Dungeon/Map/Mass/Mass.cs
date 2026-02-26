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
	private Transform _targeted = null;	// 1ターン前に効果発動した相手
	//private Trap _trap = null;	// 付与されている罠

	// プロパティ定義

	/// <value>現在シーンがダンジョンならインスタンスを取得</value>
	protected Dungeon DungeonScene => SceneLoader.Instance.CurrentScene as Dungeon;


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
		// 変数宣言
		bool _is_targeted_reaved = true;	// 処理した相手が離れたか

		// プレイヤーに対する処理
		for (int _child_idx = 0; _child_idx < transform.childCount; _child_idx++)	// 子オブジェクト単位でのループ
		{
			// 変数宣言
			var _child = transform.GetChild(_child_idx);	// 扱う子オブジェクト

			// 検査
			if (_child && _child != _targeted)	// 未処理
			{
				if (_child == DungeonScene.Player.transform)	// プレイヤーが乗った
				{
					Boot();	// 搭乗時は自動で起動する
					_targeted = _child;	// 処理した相手を記録
					_is_targeted_reaved = false;	// 相手が乗った
				}
			}
			else	// 処理済み
			{
				// 初期化
				_is_targeted_reaved = false;	// 相手はまだ居る
			}
		}

		// リセット
		if (_is_targeted_reaved)	// 処理済みの相手が離れた
		{
			_targeted = null;	// 未処理とする
		}
	}


	/// <summary>
	/// <para>機能を起動</para>
	/// </summary>
	public virtual void Boot()
	{
		//TODO:アイテムが乗っているならインベントリに入れる。ただしインベントリが満タンなら選択UIを起動する
	}
}