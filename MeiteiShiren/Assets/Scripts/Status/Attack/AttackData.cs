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
public class AttackData : CreatableData
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("味方討ち")] private bool _fiendry_fire = false;	// trueで味方も攻撃に含める
	[SerializeField, Tooltip("攻撃範囲")] private MassRange _range;
	[SerializeField, Tooltip("攻撃効果")] private Affect[] _affects;

	// プロパティ定義

	/// <value><see cref="_fiendry_fire"/></value>
	public bool FriendryFire => _fiendry_fire;

	/// <value><see cref="_range"/></value>
	public MassRange Range => _range;

	/// <value><see cref="_affects"/></value>
	public Affect[] Affects => _affects;
}