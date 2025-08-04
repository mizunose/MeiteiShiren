/*=====
<DungeonData.cs>

-author
	mizunose

-about
	ダンジョンのデータを実装
=====*/

// 名前空間宣言
using UnityEngine;
using System;

// クラス定義
/// <summary>
/// <para>ダンジョンデータ</para>
/// </summary>
[CreateAssetMenu(menuName = "Dungeon", fileName = "Dungeon")]
public class DungeonData : ScriptableObject
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("階層")]private MapData[] _map_datas;

	// プロパティ定義

	/// <summary>
	/// <para>マップ全体のサイズ</para>
	/// </summary>
	/// <value>周囲の壁も含めたマップ全体のサイズ</value>
	public MapData[] MapDatas{ get { return _map_datas; } }
}