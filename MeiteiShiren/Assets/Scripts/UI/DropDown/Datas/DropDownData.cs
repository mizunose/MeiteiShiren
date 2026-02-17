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
//[CreateAssetMenu(menuName = _MENU_TAB_NAME + _NAME, fileName = _NAME + "Data")]
public abstract class DropDownData : ScriptableObject
{
	// 定数定義
	protected const string _MENU_TAB_NAME = "DropDown/";	// 共通メニュータブ名

	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("キャンバス")] private PublicCanvasData _canvas;
	[SerializeField, Tooltip("選択UI")] private ChoiseUI _choise_ui;

	[Header("テキスト")]
	[SerializeField, Tooltip("フォント")] private TMP_FontAsset _font = null;
	[SerializeField, Tooltip("フォントサイズ"), Min(0.0f)] private float _font_size = 38.0f;


	// プロパティ定義

	/// <value><see cref="_font"/></value>
	public TMP_FontAsset Font => _font;

	/// <value><see cref="_font_size"/></value>
	public float FontSize => _font_size;

	/// <value><see cref="_choise_ui"/></value>
	public ChoiseUI ChoiseUI => _choise_ui;

	/// <value><see cref="_canvas"/></value>
	public PublicCanvasData Canvas => _canvas;
}