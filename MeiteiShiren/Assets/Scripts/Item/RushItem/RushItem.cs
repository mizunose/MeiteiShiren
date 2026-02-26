/*=====
<RushItem.cs>

-author
	mizunose

-about
	突進して効果を与えるアイテムを実装
=====*/

// 名前空間宣言
using System.Collections;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>突進するアイテム</para>
/// </summary>
public abstract class RushItem : Item
{
	// 定数定義
	private static readonly SplitedDirections _movable_directions = new EightDirections();	// 移動可能な方向


	/// <summary>
	/// <para>使用</para>
	/// </summary>
	/// <param name="user">使用者</param>
	/// <returns>遅延処理用のインターフェース体</returns>
	public override IEnumerator Use(GameObject user)
	{
		// 変数宣言
		var _rush_direction = _movable_directions.CalculateSplitedDirectionInt(user.gameObject.transform.rotation.y);	// 突撃方向
		GameObject _hitten = null;	// 当たった相手

		//TODO:モーション再生
		
		// 効果発動
		if (_hitten)//TODO:衝突判定
		{
			_affects.BootAffects(gameObject, user);	// 衝突相手に効果発動
		}

		// 提供
		yield break;	// 終了
	}
}