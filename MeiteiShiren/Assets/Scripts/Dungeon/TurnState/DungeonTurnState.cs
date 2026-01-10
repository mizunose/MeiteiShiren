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
using System.Collections.Generic;
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
		// プロパティ定義

		/// <summary>
		/// <para>行動権</para>
		/// </summary>
		/// <value>ターン内での行動可否(trueで可能, falseで不可能)</value>
		public bool Chance { get; set;} = false;


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
			Chance = true;	// 行動可能状態に戻る
		}
	}

	// イベント定義
	public event Action OnMassAction;	// マスの効果イベント
	public event Action OnTurnChanged;	// ターン変更時のイベント

	// 変数宣言
	private List<Actionable> _actors = new();	// 行動するオブジェクト一覧


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Start()
	{
		// 入力管理
		IngameInputManager.Instance.Player.TrendEnable();	// プレイヤーの干渉権を有効化

		// イベント接続
		Dungeon.Instance.Player.GetComponent<InputMove>().OnMoveStarted += OnMoveStarted;	// プレイヤー移動時処理を接続
		Dungeon.Instance.Player.GetComponent<InputAttack>().OnAttacked += OnAttacked;	// プレイヤー攻撃時処理を接続
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
	/// <para>プレイヤー攻撃時処理</para>
	/// </summary>
	private void OnAttacked()
	{
		// ターンの実行
		StartCoroutine(TurnFlow(TurnCommandType.ATTACK));	// 攻撃によってターンを起動する
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

		// 管理物整理
		_actors.RemoveAll(actor => !actor);	// ヌルを管理から除去

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
		if (OnMassAction != null)	// ヌルチェック
		{
			OnMassAction.Invoke();	// マスの処理
		}

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
		// 変数宣言
		bool _is_moved = false;	// この処理内で移動を実行したか

		// 行動管理
		foreach (var _actor in _actors)	// オブジェクト単位でのループ
		{
			if (_actor)	// ヌルチェック
			{
				if (_actor.Chance)	// 行動権あり
				{
					// 変数宣言
					var _move = _actor.GetComponent<Move>();	// 移動機能

					// 移動
					if (_move)	// ヌルチェック
					{
						// 変数宣言
						var _simulate_data = _move.Simulate();	// 試算結果

						// 行動権の消費
						if (_simulate_data.is_actionable)	// 移動するとき
						{
							_actor.Chance = false;	// 行動値を消費
							_is_moved = true;	// 移動する
							StartCoroutine(_move.MoveMotion(_simulate_data.result));	// 移動開始
						}
					}
				}
			}
		}
		
		// 待機
		if(is_wait_player || _is_moved) // プレイヤーが動いた、もしくは管理対象が1体でも動いた
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
		// 行動管理
		foreach (var _actor in _actors)	// オブジェクト単位でのループ
		{
			if (_actor)	// ヌルチェック
			{
				if (_actor.Chance)	// 行動権あり
				{
					// 変数宣言
					var _attack = _actor.GetComponent<Attack>();	// 攻撃機能

					// 攻撃
					if (_attack)	// ヌルチェック
					{
						// 変数宣言
						var _simulate_data = _attack.Simulate();	// 試算結果

						// 行動権の消費
						if (_simulate_data.AreThereAttackable)	// 攻撃するとき
						{
							_actor.Chance = false;	// 行動した
							yield return _attack.AttackMotion(_simulate_data);	// 処理完了待機
						}
					}
				}
			}
		}

		// 終了
		yield break;	// 処理完了
	}


	/// <summary>
	/// <para>破棄時処理</para>
	/// </summary>
	private void OnDestroy()
	{
		// 入力管理
		//IngameInputManager.Instance.Player.TrendDisable();	// プレイヤーの干渉権を終了	// 先にこれが削除されてしまい、ここで再生成されエラーとなるので対策が必要
	}


	/// <summary>
	/// <para>行動オブジェクト登録</para>
	/// </summary>
	/// <param name="actor">ターン管理をさせたいオブジェクト</param>
	public void AddActor(GameObject actor)
	{
		// 変数宣言
		var _actionable = actor.AddComponent<Actionable>();	// ターン制約のもと行動したいのであれば行動権を持つことが義務

		// リスト更新
		_actors.Add(_actionable);	// 行動権を登録
	}
}