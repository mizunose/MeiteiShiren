/*=====
<Player.cs>

-author
	mizunose

-about
	主操作キャラを実装
=====*/

// 名前空間宣言
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// クラス定義
/// <summary>
/// <para>プレイヤー</para>
/// </summary>
public class PlayableActor : MonoBehaviour
{
	public enum TurnCommand
	{
		COMMAND,	// コマンド選択中(未決定)
		MOVE,	// 移動
		ATTACK,	// 攻撃
		MAX,	// 要素数
	}
	
	public TurnCommand Command
	{
		get
		{
			if (InputManager.Instance.Ingame.Player.Move.triggered)	// 
			{
				return TurnCommand.MOVE;	// 移動コマンド
			}

			return TurnCommand.COMMAND;	// コマンド未決定
		}
	}
}
