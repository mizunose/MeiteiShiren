/*=====
<InputAttack.cs>

-author
	mizunose

-about
	入力攻撃を実装
=====*/

// 名前空間宣言
using System;
using System.Collections;

// クラス定義

/// <summary>
/// <para>入力攻撃</para>
/// </summary>
public class InputAttack : Attack
{
	// イベント定義
	public event Action OnAttacked;	// 攻撃時のイベント


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Start()
	{
		// 更新
		StartCoroutine(LateableUpdate());	// 更新処理の軌道
	}


	/// <summary>
	/// <para>遅延可能な更新処理</para>
	/// </summary>
	/// <returns>遅延処理用のインターフェース</returns>
	private IEnumerator LateableUpdate()
	{
		// フレーム更新
		while (true)
		{
			// 入力処理
			if (IngameInputManager.Instance.Player.Attack.BaseOne.triggered)	// 攻撃入力中
			{
				// 入力管理
				IngameInputManager.Instance.Player.TrendDisable();	// 入力系の処理なのでモーション中は干渉権を無効化する

				// 攻撃処理
				yield return AttackMotion(Simulate());	// 攻撃モーションを実行 (モーション完了待機)

				// イベント発行
				if (OnAttacked != null)	// ヌルチェック
				{
					OnAttacked.Invoke();	// 攻撃時イベント発行
				}

				// 入力管理
				IngameInputManager.Instance.Player.TrendEnable();	// プレイヤーの干渉権を復権させる
			}

			// 待機
			yield return null;	// 次フレームを待つ
		}
	}

	
	/// <summary>
	/// <para>試算処理</para>
	/// </summary>
	/// <returns>試算結果</returns>
	public override SimulatedData Simulate()
	{
		// 提供
		return CalculateAttackableMasses(true, transform, new[]{transform.eulerAngles.y});	// 攻撃可能マスの演算結果
	}
}