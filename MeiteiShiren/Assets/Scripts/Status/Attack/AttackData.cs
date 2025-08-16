/*=====
<AttackData.cs>

-author
	mizunose

-about
	攻撃のデータを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
/// <summary>
/// <para>攻撃データ</para>
/// </summary>
[CreateAssetMenu(menuName = Settings.STATUS_MENU_TAB_NAME + _NAME, fileName = _NAME)]
public class AttackData : ScriptableObject
{
	// 定数定義
	private const string _NAME = "Attack";	// タブ名称

	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("攻撃範囲")] private MassRange _range;
	[SerializeField, Tooltip("攻撃効果")] private Affect[] _affects;

	// プロパティ定義

	/// <summary>
	/// <para>腹が減るターン数</para>
	/// </summary>
	/// <value><see cref="_range"/></value>
	public MassRange Range
	{
		get
		{
			// 提供
			return _range;	// 腹が減るターン数
		}
	}

	/// <summary>
	/// <para>攻撃時効果一覧</para>
	/// </summary>
	/// <value><see cref="_affects"/></value>
	public Affect[] Affects
	{
		get
		{
			// 提供
			return _affects;	// 攻撃時に発動する効果
		}
	}
}