/*=====
<ExperienceTable.cs>

-author
	mizunose

-about
	補正値を定義

-note
・実際には何にも影響していません。
・継承によって対象に対応付けて定義してください。
	子に持たせ、効果側で取得することで値を読み出し、反映させることで、間接的かつ自動的に影響を与えます。
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
/// <summary>
/// 経験値テーブル
/// </summary>
[CreateAssetMenu(menuName = "ExperienceTable", fileName = "ExperienceTable")]
public class ExperienceTable : ScriptableObject
{
	// 変数宣言
	[SerializeField, Tooltip("経験値テーブル")]private uint[] _table;	// 経験値テーブル

	// プロパティ定義
	/// <summary>
	/// レベルごとの必要経験値
	/// </summary>
	/// <value><see cref="_table"/></value>
	public uint[] Table { get { return _table; } }
}