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
public abstract class Affect : CreatableData
{
	/// <summary>
	/// <para>各効果発動の呼び出し共通化のための抽象関数</para>
	/// </summary>
	/// <param name="oneself">効果の発動者</param>
	/// <param name="opponent">効果の受動者</param>
	public abstract void Boot(GameObject oneself, GameObject opponent);
}