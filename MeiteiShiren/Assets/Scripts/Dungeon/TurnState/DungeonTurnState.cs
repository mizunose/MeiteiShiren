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
/// ダンジョンにおけるターン管理
/// </summary>
public class DungeonTurnState : MonoBehaviour
{
	// 列挙定義
	public enum TurnState	// ターンの状態
	{
		PLAYER_INPUT,	// プレイヤーの操作ターン
	}

	// イベント定義
	public event Action OnTurnChanged;	// ターン変更時のイベント

	// プロパティ定義
	/// <summary>
	/// 現在ターンの状態
	/// </summary>
	public TurnState State { get; private set; }

	private class Actionable : MonoBehaviour
	{
		public bool _chance = false;
	}
	

	private void Start()
	{
		InputManager.Instance.Ingame.Player.Enable();


		StartCoroutine(Flow());
	}
	//protected void Update()
	//{
	//	PlayableActor.TurnCommand _player_command;
	//	// プレイヤーの入力によって処理を変更
		
	//	var _pl_act = Dungeon.Instance.Map.Player.GetComponent<PlayableActor>();
	//	if(_pl_act)
	//	{
	//		_player_command = _pl_act.Command;
	//	}
	//	else
	//	{
	//		Debug.Log("コマンド...");
	//		_player_command = PlayableActor.TurnCommand.MOVE;
	//	}

	//	// 先行処理
	//	switch (_player_command)	// プレイヤーのコマンド状態によって分岐
	//	{
	//		// まだ選択中
	//		case PlayableActor.TurnCommand.COMMAND:
	//			return;	// フローを進められない
			
	//		// 移動する
	//		case PlayableActor.TurnCommand.MOVE:
	//			StartCoroutine(MoveAll());	// 移動を先行
	//			//MoveAll();	// 移動を先行
	//			break;

	//		// その他
	//		default:
	//			break;	// 先行不要
	//	}
		
	//	InputManager.Instance.Ingame.Player.Disable();


	//	// 通常フロー


	//	// ターン終了
	//	Debug.Log("q");
	//	//InputManager.Instance.Ingame.Player.Enable();
	//	if(OnTurnChanged != null)
	//	{
	//		OnTurnChanged.Invoke();
	//	}
	//}
	
	IEnumerator Flow()
	{
		while (true)
		{
			PlayableActor.TurnCommand _player_command;
			// プレイヤーの入力によって処理を変更
		
			var _pl_act = Dungeon.Instance.Map.Player.GetComponent<PlayableActor>();
			if(_pl_act)
			{
				_player_command = _pl_act.Command;
			}
			else
			{
				_player_command = PlayableActor.TurnCommand.MOVE;
			}

			// 先行処理
			switch (_player_command)	// プレイヤーのコマンド状態によって分岐
			{
				// まだ選択中
				case PlayableActor.TurnCommand.COMMAND:
					yield return null;	// フローを進められない
					continue;	// 処理をやり直す	※switch文ではなく、while文の処理
			
				// 移動する
				case PlayableActor.TurnCommand.MOVE:
					yield return MoveAll();	// 移動を先行
					//MoveAll();	// 移動を先行
					break;

				// その他
				default:
					break;	// 先行不要
			}
		
			InputManager.Instance.Ingame.Player.Disable();


			// 通常フロー
			AttackAll();
			MoveAll();

			// ターン終了
			Debug.Log("q");
			InputManager.Instance.Ingame.Player.Enable();
			if(OnTurnChanged != null)
			{
				OnTurnChanged.Invoke();
			}
			yield return null;	// フレーム処理終了！次フレームを待つ
		}
	}

	IEnumerator MoveAll()
	{
		var _pl_mv = Dungeon.Instance.Map.Player.GetComponent<Move>();
		if(_pl_mv)
		{
			_pl_mv.Action();

			yield return new WaitForSeconds(Move.MoveStatus.Instance.Spend);
		}


		//enemies
		

		yield break;
	}

	IEnumerator AttackAll()
	{
		//player
		//yield return plAtk();

		//enemies
		

		yield break;
	}
}