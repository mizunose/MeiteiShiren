/*
<ChoiseUIValue.cs>

-author
	mizunose

-about
	選択肢のデータ
*/

// 名前空間宣言
using System;
using UnityEngine;
using UnityEngine.UI;

// クラス定義

/// <summary>
/// <para>選択肢のデータ</para>
/// </summary>
[Serializable]
public struct ChoiseUIValue
{

	// 変数宣言
	[Header("主文")]
	[SerializeField, Tooltip("主文")] public string text;
	[SerializeField, Tooltip("アイコン")] public Sprite icon;
}