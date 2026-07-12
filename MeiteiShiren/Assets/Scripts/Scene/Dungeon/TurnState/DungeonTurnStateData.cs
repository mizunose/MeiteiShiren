/*
<DungeonTurnStateData.cs>

-author
	mizunose

-about
	ターン管理のデータ
*/

// 名前空間宣言
using NUnit.Framework;
using System;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>ターン管理のデータ</para>
/// </summary>
public class DungeonTurnStateData : CreatableData
{
	// 構造体定義

	/// <summary>
	/// <para>ターンデータ</para>
	/// </summary>
	[Serializable]
	public struct LimitData
	{
		[Tooltip("ターン閾値")] public int turn_threshold;
		[Tooltip("表示文")] public string log_text;
	};

	// 変数宣言
	[Header("ターン制限")]
	[SerializeField, Tooltip("ターン制限と警告文(累加制で末尾到達時に終了)	※何もない場合上限なし")] private LimitData[] _turn_limits;

	// プロパティ定義

	/// <value><see cref="_turn_limits"/></value>
	public LimitData[] TurnLimits => _turn_limits;
}