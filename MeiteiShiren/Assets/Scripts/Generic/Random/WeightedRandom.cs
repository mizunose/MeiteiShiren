/*=====
<WeightedRandom.cs>

-author
	mizunose

-about
	重み付き乱数を定義
=====*/

// 名前空間宣言
using System;
using System.Collections.Generic;
using UnityEngine;

// 構造体定義

/// <summary>
/// <para>重み付き乱数テーブル</para>
/// </summary>
/// <typeparam name="TargetType">抽選対象</typeparam>
[Serializable]
public class WeightedRandom<TargetType>
{
	// 構造体定義

	/// <summary>
	/// <para>テーブルデータ</para>
	/// </summary>
	[Serializable]
	public struct RandomTableCell
	{
		// 変数宣言
		[Tooltip("相対抽選率"), Min(0)] public int relative_probability;
		[Tooltip("抽選対象")] public TargetType target;
	}

	// 変数宣言
	[SerializeField, Tooltip("テーブルデータ")] private List<RandomTableCell> _random_table = new();


	/// <summary>
	/// <para>空検査</para>
	/// </summary>
	/// <returns>テーブル内が空の場合true, 何か含まれていればfalse</returns>
	public bool IsEmpty()
	{
		// 保全
		if (_random_table.Count == 0)	// 生成候補がない
		{
			// 提供
			return true;	// 生成できない
		}

		// 提供
		return false;	// 問題なし
	}


	/// <summary>
	/// <para>抽選</para>
	/// </summary>
	/// <returns>抽選結果。抽選に失敗した場合は決定できないためデフォルト値を返す</returns>
	public TargetType DrawLots()
	{
		// 変数宣言
		int _all_probability = 0;	// 総確率

		// 統計
		foreach (var random_cell in _random_table)	// パターン単位でのループ
		{
			_all_probability += random_cell.relative_probability;	// 総確率に記録
		}

		// 保全
		if (!(_all_probability > 0))	// 全て当たらない
		{
			return default;	// 抽選不可
		}

		// 変数宣言
		int _result = UnityEngine.Random.Range(0, _all_probability);	// 確率結果
		int _result_idx = 0;	// 抽選番号

		// 抽選結果選定
		foreach (var specialize_pattern in _random_table)	// パターン単位でのループ
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
		return _random_table[_result_idx].target;	// 該当データ
	}
}