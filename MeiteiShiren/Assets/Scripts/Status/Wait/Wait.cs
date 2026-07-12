/*=====
<Wait.cs>

-author
	mizunose

-about
	待機を定義
=====*/

// 名前空間宣言
using System;
using System.Collections;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>待機</para>
/// </summary>
[DisallowMultipleComponent]
public abstract class Wait : MonoBehaviour
{
	// イベント定義
	public event Action OnWaitStarted;	// 待機開始時のイベント


	/// <summary>
	/// <para></para>
	/// </summary>
	protected void WaitTurn()
	{
		// イベント発行
		if (OnWaitStarted != null)	// ヌルチェック
		{
			OnWaitStarted.Invoke();	// 待機開始時のイベントを発行
		}
	}


	/// <summary>
	/// <para>待機モーション処理</para>
	/// </summary>
	/// <param name="data">試算データ</param>
	/// <returns>遅延処理用のインターフェース</returns>
	public IEnumerator WaitMotion()
	{
		// イベント発行
		WaitTurn();	// 待機イベント

		// 終了
		yield break;	// モーション完了
	}
}