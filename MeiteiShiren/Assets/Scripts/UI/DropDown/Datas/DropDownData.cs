/*
<DropDownData.cs>

-author
	mizunose

-about
	ドロップダウンのデータ
*/

// 名前空間宣言
using TMPro;
using UnityEngine;

// クラス定義
public abstract class DropDownData : CreatableData
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("キャンバス")] private PublicCanvasData _canvas;
	[SerializeField, Tooltip("選択UI")] private ChoiseUI _choise_ui;


	// プロパティ定義

	/// <value><see cref="_choise_ui"/></value>
	public ChoiseUI ChoiseUI => _choise_ui;

	/// <value><see cref="_canvas"/></value>
	public PublicCanvasData Canvas => _canvas;
}