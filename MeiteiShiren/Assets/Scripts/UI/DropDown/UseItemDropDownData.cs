/*
<UseItemDropDownData.cs>

-author
	mizunose

-about
	アイテム使用ドロップダウンのデータ
*/

// 名前空間宣言
using System.Collections.Generic;
using UnityEngine;

// クラス定義

/// <summary>
/// <para> Y/N ドロップダウンのデータ</para>
/// </summary>
public class UseItemDropDownData : DropDownData
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("使用コマンド")] private string _use_text = "使う";
	[SerializeField, Tooltip("廃棄コマンド")] private string _dispose_text = "捨てる";

	// プロパティ定義

	/// <value><see cref="_use_text"/></value>
	public string UseText => _use_text;

	/// <value><see cref="_dispose_text"/></value>
	public string DisposeText => _dispose_text;
}