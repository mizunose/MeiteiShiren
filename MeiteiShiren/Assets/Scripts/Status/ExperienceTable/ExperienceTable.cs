/*=====
<ExperienceTable.cs>

-author
	mizunose

-about
	経験値テーブルを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>経験値テーブル</para>
/// </summary>
public class ExperienceTable : CreatableData
{
	// 変数宣言
	[SerializeField, Tooltip("次レベルまでの経験値")] private uint[] _table;

	// プロパティ定義

	/// <value><see cref="_table"/></value>
	public uint[] Table => _table;
}