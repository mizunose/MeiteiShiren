/*=====
<Map.cs>

-author
	mizunose

-about
	マップのデータを定義
=====*/

// 名前空間宣言
using UnityEditor;
using UnityEngine;
using Unity.Collections;

// クラス定義
/// <summary>
/// マップデータの抽象クラス
/// </summary>
//[CreateAssetMenu(menuName = _MENU_TAB_NAME + "MapName", fileName = "MapName")]	と子クラスは記述
public abstract class MapData : ScriptableObject
{
	// 定数定義
	protected const string _MENU_TAB_NAME = "MapData/"; // 共通メニュータブ名

	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("サイズ"), Min(1)] protected Vector2Int _size = new Vector2Int(1, 1);
	[Header("マスの情報")]
	[SerializeField, Tooltip("地面のテクスチャ")] protected Material _ground_texture;
	[SerializeField, Tooltip("壁のモデル")] protected GameObject _wall_model;

	// プロパティ定義
	/// <summary>
	/// 外部には読み込み専用なマップ情報
	/// </summary>
	/// <value>マップ情報を格納したテクスチャ</value>
	public Texture2D MapTexture {get; protected set;}


	/// <summary>
	/// マップ生成処理
	/// </summary>
	/// <param name="_Oneself">効果の発動者</param>
	/// <param name="_Opponent">効果の受動者</param>
	public abstract void Generate();
}