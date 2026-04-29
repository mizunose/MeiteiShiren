/*=====
<SelfAffectItem.cs>

-author
	mizunose

-about
	自分に使用するアイテムを実装
=====*/

// 名前空間宣言
using System.Collections;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>自分に使用するアイテム</para>
/// </summary>
public class SelfAffectItem : Item
{
	/// <summary>
	/// <para>使用モーション処理</para>
	/// </summary>
	/// <param name="user">使用者</param>
	/// <returns>遅延処理用のインターフェース体</returns>
	protected　override IEnumerator _UseMotion(GameObject user)
	{
		//TODO:モーション再生

		//TODO:味方を選ぶ

		// 効果発動
		_data.Affects.BootAffects(gameObject, user);	// 発動者に効果発動
		
		// 提供
		yield break;	// 終了
	}
}