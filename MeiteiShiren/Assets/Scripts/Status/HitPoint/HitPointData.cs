/*=====
<HitPointData.cs>

-author
	mizunose

-about
	体力のデータを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
/// <summary>
/// <para>体力データ</para>
/// </summary>
[CreateAssetMenu(menuName = Settings.STATUS_MENU_TAB_NAME + _NAME, fileName = _NAME)]
public class HitPointData : ScriptableObject
{
	// 定数定義
	private const string _NAME = "HitPoint";	// タブ名称

	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("最大体力"), Min(HitPoint.MIN_GUARANTEE_MAX_VALUE)] private int _max_hp = HitPoint.MIN_GUARANTEE_MAX_VALUE;
	[SerializeField, Tooltip("防御")] private float _defence = 0;


	/// <summary>
	/// <para>最大体力</para>
	/// </summary>
	/// <value><see cref="_defence"/></value>
	public int MaxHP
	{
		get
		{
			// 提供
			return _max_hp;	// 最大HP
		}
	}

	/// <summary>
	/// <para>防御力</para>
	/// </summary>
	/// <value><see cref="_defence"/></value>
	public float Defence
	{
		get
		{
			// 提供
			return _defence;	// 防御力
		}
	}
}