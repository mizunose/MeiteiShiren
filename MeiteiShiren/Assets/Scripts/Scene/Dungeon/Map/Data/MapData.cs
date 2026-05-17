/*=====
<MapData.cs>

-author
	mizunose

-about
	マップのデータを定義
=====*/

// 名前空間宣言
using UnityEngine;
using System.Collections.Generic;

// クラス定義

/// <summary>
/// <para>マップデータ</para>
/// </summary>
public abstract class MapData : CreatableData
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("サイズ"), Min(1)] protected Vector2Int _size = new Vector2Int(1, 1);
	[Header("生成物の情報")]
	[SerializeField, Tooltip("地面のテクスチャ")] private Material _ground_texture;
	[SerializeField, Tooltip("壁のモデル")] private GameObject _wall_model;
	[SerializeField, Tooltip("階段のデータ")] private StairData _stair_data;
	[Header("ミニマップ")]
	[SerializeField, Tooltip("アンカー(左上)に対する位置")] private Vector2 _anchor_position = new Vector2(220, -220);
	[SerializeField, Tooltip("ミニマップ表示サイズ")] private Vector2 _minimap_size = new Vector2(400, 400);

	// プロパティ定義

	/// <value>マップ情報を格納したテクスチャ</value>
	public Texture2D MiniMapTexture { get; protected set; }

	/// <value>周囲の壁も含めたマップ全体のサイズ</value>
	public abstract Vector2Int MapSize{ get; }

	/// <value>周囲の壁も含めたマップ全体のマス</value>
	public Mass[,] Masses { get; protected set; }

	/// <value>主部分の連続領域</value>
	public GameObject MainContact { get; protected set; }

	/// <value><see cref="_ground_texture"/></value>
	public Material GroundTexture => _ground_texture;

	/// <value><see cref="_stair_data"/></value>
	public StairData StairData => _stair_data;

	/// <value><see cref="_anchor_position"/></value>
	public Vector2 AnchorPosition => _anchor_position;
	
	/// <value><see cref="_anchor_position"/></value>
	public Vector2 MiniMapSize => _minimap_size;


	/// <summary>
	/// <para>マップ生成処理</para>
	/// </summary>
	public abstract void Generate();
}