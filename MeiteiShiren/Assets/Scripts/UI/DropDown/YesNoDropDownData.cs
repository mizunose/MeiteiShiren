/*
<YesNoDropDownData.cs>

-author
	mizunose

-about
	ドロップダウン式の Y/N ボタンのデータ
*/

// 名前空間宣言
using System.Collections.Generic;
using UnityEngine;

// クラス定義

/// <summary>
/// <para> Y/N ドロップダウンのデータ</para>
/// </summary>
public class YesNoDropDownData : DropDownData
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("肯定文")] private string _yes_text = "はい";
	[SerializeField, Tooltip("否定文")] private string _no_text = "いいえ";

	// プロパティ定義

	/// <value><see cref="_yes_text"/></value>
	public string YesText => _yes_text;

	/// <value><see cref="_no_text"/></value>
	public string NoText => _no_text;
}