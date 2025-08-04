/*=====
<Map.cs>

-author
	mizunose

-about
	マップのデータを定義
=====*/

// 名前空間宣言
using System;
using UnityEngine;

// クラス定義
/// <summary>
/// <para>マップデータの抽象クラス</para>
/// </summary>
//[CreateAssetMenu(menuName = _MENU_TAB_NAME + "MapName", fileName = "MapName")]	と子クラスは記述
public abstract class MapData : ScriptableObject
{
	// 構造体定義
	[Serializable]
	protected struct ObjectMakeInfo
	{
		[Tooltip("プレハブ")] public GameObject model;
		[Tooltip("モデルの中心")] public Vector3 center;
	}

	// 定数定義
	protected const string _MENU_TAB_NAME = "MapData/";	// 共通メニュータブ名
	public const float MASS_SIZE = 1.0f;	// 1マスあたりの大きさ

	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("サイズ"), Min(1)] protected Vector2Int _size = new Vector2Int(1, 1);
	[Header("生成物の情報")]
	[SerializeField, Tooltip("地面のテクスチャ")] protected Material _ground_texture;
	[SerializeField, Tooltip("壁のモデル")] protected ObjectMakeInfo _wall_model;
	[SerializeField, Tooltip("操作キャラ")] protected ObjectMakeInfo _player;	//TODO:チーム配置
	[Header("ミニマップ")]
	[SerializeField, Tooltip("アンカーに対する位置")]public Vector2 _anchor_position = new Vector2(220, -220);
	[SerializeField, Tooltip("ミニマップ表示サイズ")]public Vector2 _minimap_size = new Vector2(400, 400);

	// プロパティ定義

	/// <summary>
	/// <para>外部には読み込み専用なマップ情報</para>
	/// </summary>
	/// <value>マップ情報を格納したテクスチャ</value>
	public Texture2D MapTexture {get; protected set;}
	/// <summary>
	/// <para>マップ全体のサイズ</para>
	/// </summary>
	/// <value>周囲の壁も含めたマップ全体のサイズ</value>
	public abstract Vector2Int MapSize{get;}
	/// <summary>
	/// <para>マップのマス</para>
	/// </summary>
	/// <value>周囲の壁も含めたマップ全体のマス</value>
	public Mass[,] MapMasses{get; protected set;}

	public GameObject Player{get; protected set;}



	/// <summary>
	/// <para>マップ生成処理</para>
	/// </summary>
	/// <param name="_Oneself">効果の発動者</param>
	/// <param name="_Opponent">効果の受動者</param>
	public abstract void Generate();
}