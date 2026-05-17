/*=====
<TurnAttack.cs>

-author
	mizunose

-about
	ターン呼び出し待ちの攻撃を実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>ターン制攻撃</para>
/// </summary>
public class TurnAttack : Attack
{	
	/// <summary>
	/// <para>試算処理</para>
	/// </summary>
	/// <returns>試算結果</returns>
	public override SimulatedData Simulate()
	{
		// 提供
		return CalculateAttackableMasses(true, transform, AttackableAngles);	// 攻撃可能マスの演算結果
	}
}