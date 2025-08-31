/*=====
<MapData.cs>

-author
	mizunose

-about
	マップのデータを定義
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
/// <summary>
/// <para>マップデータの抽象クラス</para>
/// </summary>
//[CreateAssetMenu(menuName = _MENU_TAB_NAME + "MapName", fileName = "MapName")]	と子クラスは記述
public abstract class MapData : ScriptableObject
{
	// 定数定義
	protected const string _MENU_TAB_NAME = "MapData/";	// 共通メニュータブ名

	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("サイズ"), Min(1)] protected Vector2Int _size = new Vector2Int(1, 1);
	[Header("生成物の情報")]
	[SerializeField, Tooltip("地面のテクスチャ")] protected Material _ground_texture;
	[SerializeField, Tooltip("壁のモデル")] protected GameObject _wall_model;
	[Header("ミニマップ")]
	[SerializeField, Tooltip("アンカー(左上)に対する位置")] protected Vector2 _anchor_position = new Vector2(220, -220);
	[SerializeField, Tooltip("ミニマップ表示サイズ")] protected Vector2 _minimap_size = new Vector2(400, 400);

	// プロパティ定義

	/// <summary>
	/// <para>外部には読み込み専用なマップ情報</para>
	/// </summary>
	/// <value>マップ情報を格納したテクスチャ</value>
	public Texture2D MiniMapTexture { get; protected set; }

	/// <summary>
	/// <para>マップ全体のサイズ</para>
	/// </summary>
	/// <value>周囲の壁も含めたマップ全体のサイズ</value>
	public abstract Vector2Int MapSize{ get; }

	/// <summary>
	/// <para>マップのマス</para>
	/// </summary>
	/// <value>周囲の壁も含めたマップ全体のマス</value>
	public Mass[,] Masses { get; protected set; }

	/// <summary>
	/// <para>メイン領域</para>
	/// </summary>
	/// <value>主部分の連続領域</value>
	public GameObject MainContact { get; protected set; }

	/// <summary>
	/// <para>マスが適用するマテリアル</para>
	/// </summary>
	/// <value><see cref="_ground_texture"/></value>
	public Material GroundTexture => _ground_texture;

	/// <summary>
	/// <para>アンカーに対する相対位置</para>
	/// </summary>
	/// <value><see cref="_anchor_position"/></value>
	public Vector2 AnchorPosition => _anchor_position;
	
	/// <summary>
	/// <para>ミニマップの描画サイズ</para>
	/// </summary>
	/// <value><see cref="_anchor_position"/></value>
	public Vector2 MiniMapSize => _minimap_size;


	/// <summary>
	/// <para>マップ生成処理</para>
	/// </summary>
	public abstract void Generate();
}