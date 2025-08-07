/*=====
<Move.cs>

-author
	mizunose

-about
	移動を定義
=====*/

// 名前空間宣言
using System;
using UnityEngine;
using System.Collections;

// クラス定義
/// <summary>
/// <para>移動</para>
/// </summary>
public abstract class Move : MonoBehaviour
{
	// イベント定義
	public event Action OnMoveStarted;	// 移動開始時のイベント


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
			if (InputManager.Instance.Ingame.Player.Move.IsPressed())	// 移動入力中
			{
				yield return MoveMotion();	// 移動モーションを実行 (モーション完了待機)
			}

			// 待機
			yield return null;	// 次フレームを待つ
		}
	}


	/// <summary>
	/// <para>移動モーション処理</para>
	/// </summary>
	/// <returns>遅延処理用のインターフェース</returns>
	protected abstract IEnumerator MoveMotion();


	/// <summary>
	/// <para>移動開始時の処理</para>
	/// </summary>
	protected void EmitOnMoveStarted()
	{
		// イベント発行
		if (OnMoveStarted != null)	// ヌルチェック
		{
			OnMoveStarted.Invoke();	// 移動開始時のイベントを発行
		}
	}
}