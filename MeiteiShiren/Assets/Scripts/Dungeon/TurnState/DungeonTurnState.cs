/*=====
<DungeonTurnState.cs>

-author
	mizunose

-about
	ターン管理
=====*/

// クラス定義
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// <para>ダンジョンにおけるターン管理</para>
/// </summary>
public class DungeonTurnState : MonoBehaviour
{
	// 列挙定義
	private enum TurnCommandType	// ターンを起動したコマンド種
	{
		MOVE,	// 移動
		ATTACK,	// 攻撃
		MAX,	// 要素数
	}

	// クラス定義
	/// <summary>
	/// <para>行動権</para>
	/// </summary>
	private class Actionable : MonoBehaviour
	{
		// 変数宣言
		private bool _chance = false;	// ターン内での行動可否(trueで可能, falseで不可能)

		// プロパティ定義
		/// <summary>
		/// <para>行動権</para>
		/// </summary>
		/// <value><see cref="_chance"/></value>
		public bool Chance => _chance;	// このターン内での行動権


		/// <summary>
		/// <para>初期化処理</para>
		/// </summary>
		private void Start()
		{
			// イベント接続
			Dungeon.Instance.TurnFlow.OnTurnChanged += OnTurnChanged;	// ターン変更時処理を接続
		}

		/// <summary>
		/// <para>ターン変更時処理</para>
		/// </summary>
		private void OnTurnChanged()
		{
			// フラグ操作
			_chance = true;	// 行動可能状態に戻る
		}
	}

	// イベント定義
	public event Action OnTurnChanged;	// ターン変更時のイベント


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Start()
	{
		// 入力管理
		IngameInputManager.Instance.Player.TrendEnable();	// プレイヤーの干渉権を有効化

		// イベント接続
		Dungeon.Instance.Player.GetComponent<Move>().OnMoveStarted += OnMoveStarted;	// プレイヤー移動時処理を接続
	}


	/// <summary>
	/// <para>プレイヤー移動時処理</para>
	/// </summary>
	private void OnMoveStarted()
	{
		// ターンの実行
		StartCoroutine(TurnFlow(TurnCommandType.MOVE));	// 移動によってターンを起動する
	}


	/// <summary>
	/// <para>ターン処理</para>
	/// </summary>
	/// <param name="command">ターンを起動したコマンド</param>
	/// <returns>遅延処理用のインターフェース</returns>
	private IEnumerator TurnFlow(TurnCommandType command)
	{
		// 入力管理
		IngameInputManager.Instance.Player.TrendDisable();	// ターンの処理が始まったのでプレイヤーの干渉権は一時的に失われる

		// 先行処理
		switch (command)	// プレイヤーのコマンド状態によって分岐
		{
			// 移動する
			case TurnCommandType.MOVE:
				yield return MoveAll(true);	// プレイヤーに合わせ、移動を先行
				break;	// 分岐処理終了

			// その他
			default:
				break;	// 先行不要
		}

		// 通常フロー
		yield return AttackAll();	// 攻撃処理
		yield return MoveAll(false);	// 移動処理

		// ターン終了
		IngameInputManager.Instance.Player.TrendEnable();	// プレイヤーの干渉権を復権させる
		if(OnTurnChanged != null)	// ヌルチェック
		{
			OnTurnChanged.Invoke();	// ターン変更時イベント発行
		}
		yield return null;	// フレーム処理終了！次フレームを待つ
	}


	/// <summary>
	/// <para>一斉移動処理</para>
	/// </summary>
	/// <param name="is_wait_player">プレイヤーも移動しているならtrue, そうでなければfalse</param>
	/// <returns>遅延処理用のインターフェース</returns>
	private IEnumerator MoveAll(bool is_wait_player)
	{
		//enemies->move
		
		// 待機
		if(is_wait_player || false) // プレイヤーが動いた、もしくはenemyが1体でも動いた
		{
			yield return new WaitForSeconds(Settings.Instance.Move.Spend);	// 移動モーション時間を待機する
		}

		// 終了
		yield break;	// 処理完了
	}


	/// <summary>
	/// <para>攻撃処理</para>
	/// </summary>
	/// <returns>遅延処理用のインターフェース</returns>
	private IEnumerator AttackAll()
	{
		//enemies->Attack
		
		// 終了
		yield break;	// 処理完了
	}
}