/*=====
<SpecializeMonsterHouseData.cs>

-author
	mizunose

-about
	特殊モンスターハウス生成のデータを実装
=====*/

// 名前空間宣言
using System;
using System.Collections.Generic;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>特殊モンスターハウス化データ</para>
/// </summary>
public class SpecializeMonsterHouseData : CreatableData
{
	// 定数定義
	private const int _RATIO_RAND_RANGE_MAX = 100;	// 乱数幅

	// 変数宣言
	[SerializeField, Tooltip("特殊化率"), Range(0, _RATIO_RAND_RANGE_MAX)] private int _to_specialize_threshold = 0;
	[SerializeField, Tooltip("特殊生成対象")] private WeightedRandom<SpecialMonsterHouseData> _specialize_patterns = new();


	/// <summary>
	/// <para>特殊化抽選</para>
	/// </summary>
	/// <returns>特殊化に成功したときは対応する抽選結果、失敗した場合は使用しないためヌルを返す</returns>
	public SpecialMonsterHouseData DrawLots()
	{
		// 抽選
		if (UnityEngine.Random.Range(0, _RATIO_RAND_RANGE_MAX) < _to_specialize_threshold)	// 特殊化成功
		{
			// 提供
			return _specialize_patterns.DrawLots();	// 抽選結果
		}
		else	// 失敗
		{
			// 提供
			return null;	// 特殊化しない
		}
	}
}