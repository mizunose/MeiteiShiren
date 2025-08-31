/*=====
<Mass.cs>

-author
	mizunose

-about
	マス目を実装
=====*/

// 名前空間宣言
using UnityEngine;
using UnityEngine.UIElements;

// クラス定義
/// <summary>
/// マップを構成するマス。子に管理物(乗っているもの)を持つ想定。また、親が構成物(マップや部屋など)である想定。
/// </summary>
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
	private static readonly Vector3[] _VERTICES = {	// マスメッシュの頂点情報
			new Vector3(-Settings.Instance.Map.MassSize * 0.5f, 0.0f, Settings.Instance.Map.MassSize * 0.5f),	// 左上
			new Vector3(Settings.Instance.Map.MassSize * 0.5f, 0.0f, Settings.Instance.Map.MassSize * 0.5f),	// 右上
			new Vector3(-Settings.Instance.Map.MassSize * 0.5f, 0.0f, -Settings.Instance.Map.MassSize * 0.5f),	// 左下
			new Vector3(Settings.Instance.Map.MassSize * 0.5f, 0.0f, -Settings.Instance.Map.MassSize * 0.5f),	// 右下
		};

	// 変数宣言
	//private Trap _trap = null;	// 付与されている罠


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	protected override sealed void Start()
	{
		// 代替処理
		CustomStart();	// 子クラスの初期化処理

		// 初期化
		Visualize();	// 視覚化
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
		gameObject.AddComponent<MeshRenderer>().material = Dungeon.Instance.FloorData.MapData.GroundTexture;	// メッシュの描画機能を追加し、その参照マテリアルをマップに合わせて変更
		_mesh.vertices = _VERTICES;	// メッシュの頂点情報を設定
		_mesh.triangles = _INDICES;	// メッシュの頂点インデックスを設定
		_mesh.RecalculateNormals();	// 法線を再計算
		_mesh.uv = _UVS;	// テクスチャ座標を設定
		_mesh_filter.sharedMesh = _mesh;	// 作成したメッシュを登録
	}
}