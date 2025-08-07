/*=====
<HungerData.cs>

-author
	mizunose

-about
	空腹/満腹 のデータを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
/// <summary>
/// <para>空腹/満腹 データ</para>
/// </summary>
[CreateAssetMenu(menuName = Settings.STATUS_MENU_TAB_NAME + _NAME, fileName = _NAME)]
public class HungerData : ScriptableObject
{
	// 定数定義
	private const string _NAME = "Hunger";	// タブ名称

	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("腹が減るターン数"), Min(1)] private uint _keep_fulling = 1;
	[SerializeField, Tooltip("空腹時の効果")] private Affect _affect;

	// プロパティ定義

	/// <summary>
	/// <para>腹が減るターン数</para>
	/// </summary>
	/// <value><see cref="_keep_fulling"/></value>
	public uint KeepFulling
	{
		get
		{
			// 提供
			return _keep_fulling;	// 腹が減るターン数
		}
	}

	/// <summary>
	/// <para>空腹時効果</para>
	/// </summary>
	/// <value><see cref="_affect"/></value>
	public Affect Affect
	{
		get
		{
			// 提供
			return _affect;	// 空腹時に発動する効果
		}
	}
}