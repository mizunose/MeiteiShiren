/*=====
<ChaseData.cs>

-author
	mizunose

-about
	追跡のデータを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
/// <summary>
/// <para>攻撃データ</para>
/// </summary>
[CreateAssetMenu(menuName = Settings.STATUS_MENU_TAB_NAME + _NAME, fileName = _NAME)]
public class ChaseData : ScriptableObject
{
	// 定数定義
	private const string _NAME = "Chase";	// タブ名称

	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("視界の範囲"), Min(0)] private int _view_range = 1;

	// プロパティ定義

	/// <summary>
	/// <para>所属陣営</para>
	/// </summary>
	/// <value><see cref="_view_range"/></value>
	public int ViewRange => _view_range;
}