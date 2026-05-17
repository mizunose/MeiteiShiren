/*
<DropDownData.cs>

-author
	mizunose

-about
	ドロップダウンのデータ
*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>ドロップダウンのデータ</para>
/// </summary>
public abstract class DropDownData : CreatableData
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("選択UI")] private ChoiseUI _choise_ui;
	[SerializeField, Tooltip("複数回選択できるか。trueで選択後も持続し、falseで選択とともに消滅")] private bool _durability = false;


	// プロパティ定義

	/// <value><see cref="_choise_ui"/></value>
	public ChoiseUI ChoiseUI => _choise_ui;

	/// <value><see cref="_durability"/></value>
	public bool Durability => _durability;
}