/*=====
<CampData.cs>

-author
	mizunose

-about
	陣営のデータを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
/// <summary>
/// <para>攻撃データ</para>
/// </summary>
[CreateAssetMenu(menuName = Settings.STATUS_MENU_TAB_NAME + _NAME, fileName = _NAME)]
public class CampData : ScriptableObject
{
	// 列挙定義
	public enum CampType	// 陣営の種類
	{
		Comrade,	// 味方陣営(プレイヤー陣営)
		Enemy,	// 敵陣営
	}

	// 定数定義
	private const string _NAME = "Camp";	// タブ名称

	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("種類")] private CampType _type;

	// プロパティ定義

	/// <summary>
	/// <para>所属陣営</para>
	/// </summary>
	/// <value><see cref="_type"/></value>
	public CampType Type => _type;
}