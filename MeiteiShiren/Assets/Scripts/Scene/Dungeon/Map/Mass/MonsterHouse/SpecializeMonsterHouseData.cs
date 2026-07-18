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
	private const int _RATIO_RAND_RANGE_MAX = 100;  // 乱数幅

	// 構造体定義

	/// <summary>
	/// <para>生成パターン</para>
	/// </summary>
	[Serializable]
	private struct SpecializePattern
	{
		// 変数宣言
		[Tooltip("相対生成率"), Min(0)] public int relative_probability;
		[Tooltip("生成対象")] public SpecialMonsterHouseData spetialize_data;
	}

	// 変数宣言
	[SerializeField, Tooltip("特殊化率"), Range(0, _RATIO_RAND_RANGE_MAX)] private int _to_specialize_threshold = 0;
	[SerializeField, Tooltip("特殊生成対象")] private List<SpecializePattern> _specialize_patterns = new();



	/// <summary>
	/// <para>特殊化抽選</para>
	/// </summary>
	/// <returns>特殊化に成功したときは対応する抽選結果、失敗した場合は使用しないためヌルを返す</returns>
	public SpecialMonsterHouseData DrawLots()
	{
		// 抽選
		if (UnityEngine.Random.Range(0, _RATIO_RAND_RANGE_MAX) < _to_specialize_threshold)	// 特殊化成功
		{
			// 変数宣言
			int _all_probability = 0;	// 総確率

			// 統計
			foreach (var specialize_pattern in _specialize_patterns)	// パターン単位でのループ
			{
				_all_probability += specialize_pattern.relative_probability;	// 総確率に記録
			}

			// 保全
			if (!(_all_probability > 0))	// 全て当たらない
			{
				return null;	// 抽選不可
			}

			// 変数宣言
			int _result = UnityEngine.Random.Range(0, _all_probability);	// 確率結果
			int _result_idx = 0;	// 抽選番号

			// 抽選結果選定
			foreach (var specialize_pattern in _specialize_patterns)	// パターン単位でのループ
			{
				_result -= specialize_pattern.relative_probability;	// 絞り込み
				if (_result < 0)	// 対象深度に到達
				{
					// 終了
					break;	// 結果確定
				}

				// 更新
				_result_idx++;	// 階層算出
			}
			
			// 提供
			return _specialize_patterns[_result_idx].spetialize_data;	// 該当データ
		}
		else	// 失敗
		{
			// 提供
			return null;	// 特殊化しない
		}
	}
}
