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
	[SerializeField, Tooltip("使用コマンド")] private ChoiseUIValue _use_text = new ChoiseUIValue{ text = "使う" };
	[SerializeField, Tooltip("廃棄コマンド")] private ChoiseUIValue _dispose_text = new ChoiseUIValue{ text = "捨てる" };

	// プロパティ定義

	/// <value><see cref="_use_text"/></value>
	public ChoiseUIValue UseText => _use_text;

	/// <value><see cref="_dispose_text"/></value>
	public ChoiseUIValue DisposeText => _dispose_text;
}