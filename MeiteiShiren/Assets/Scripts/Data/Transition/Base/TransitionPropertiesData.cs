/*
<TransitionPropertiesData.cs>

-author
	mizunose

-about
	トランジションのプロパティのデータを定義
*/

// 名前空間宣言
using UnityEngine;

// クラス定義
/// <summary>
/// <para>トランジションプロパティデータ</para>
/// </summary>
//[CreateAssetMenu(menuName = _MENU_TAB_NAME + _NAME, fileName = _NAME)]
public abstract class TransitionPropertiesData : ScriptableObject
{
	// 定数定義
	protected const string _MENU_TAB_NAME = TransitionData.TRANSITION_MENU_TAB_NAME + "Properties/";	// アセットメニュー名

	// 変数宣言
	[SerializeField, Tooltip("対応シェーダー")] private Shader _supported_shader;

	// プロパティ定義

	/// <value><see cref="_supported_shader"/></value>
	public Shader SupportedShader => _supported_shader;
}