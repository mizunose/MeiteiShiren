/*=====
<MoveData.cs>

-author
	mizunose

-about
	移動のデータを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>移動データ</para>
/// </summary>
public class MoveData : CreatableData
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("マスでの残留性	※falseで残留しない=マスに記憶されない")] private bool _persistence_on_mass = true;

	// プロパティ定義

	/// <value><see cref="_persistence_on_mass"/></value>
	public bool PersistenceOnMass => _persistence_on_mass;
}