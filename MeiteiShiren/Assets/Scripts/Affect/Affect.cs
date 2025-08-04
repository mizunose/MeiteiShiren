/*=====
<Affect.cs>

-author
	mizunose

-about
	効果を定義
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
//[CreateAssetMenu(menuName = AFFECT_MENU_TAB_NAME + "AffectName", fileName = "AffectName")]	と子クラスは記述
public abstract class Affect : ScriptableObject
{
	// 定数定義
	protected const string _AFFECT_MENU_TAB_NAME = "Affect/";	// 共通メニュータブ名


	/// <summary>
	/// <para>各効果発動の呼び出し共通化のための抽象関数</para>
	/// </summary>
	/// <param name="_Oneself">効果の発動者</param>
	/// <param name="_Opponent">効果の受動者</param>
	public abstract void Boot(GameObject _Oneself, GameObject _Opponent);
}