/*=====
<ObjectMakeInfo.cs>

-author
	mizunose

-about
	オブジェクト作成用の型を実装
=====*/

// 名前空間宣言
using System;
using UnityEngine;

// 構造体定義
/// <summary>
/// <para>オブジェクト作成用の情報</para>
/// </summary>
[Serializable]
public struct ObjectMakeInfo
{
	// 変数宣言
	[Tooltip("プレハブ")] public GameObject model;
	[Tooltip("モデルの中心")] public Vector3 center;
}