/*
<UserInterfaceData.cs>

-author
	mizunose

-about
	UIのデータ
*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>UIのデータ</para>
/// </summary>
public class UserInterfaceData : CreatableData
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("キャンバス")] private PublicCanvasData _canvas;


	// プロパティ定義

	/// <value><see cref="_canvas"/></value>
	public PublicCanvasData Canvas => _canvas;
}