/*=====
<InputWait.cs>

-author
	mizunose

-about
	入力待機を実装
=====*/

// 名前空間宣言
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// クラス定義

/// <summary>
/// <para>入力待機</para>
/// </summary>
public class InputWait : Wait
{
	// 変数宣言
	private InputAction _temporal_input = new();	// 一時的に入力保護するための領域

	// プロパティ定義

	/// <value>現在シーンがダンジョンならインスタンスを取得</value>
	private Dungeon _DungeonScene => SceneLoader.Instance.CurrentScene as Dungeon;


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Start()
	{
		// 初期化
		foreach (var binding in IngameInputManager.Instance.Player.Wait.BaseOne.bindings)	// 受付入力単位でのループ
		{
			_temporal_input.AddBinding(new InputBinding{path = binding.path, interactions = binding.interactions, processors = binding.processors, groups = binding.groups});	// 入力受付を模倣
		}

		// 更新
		StartCoroutine(LateableUpdate());	// 更新処理の起動
	}


	/// <summary>
	/// <para>無効時処理</para>
	/// </summary>
	private void OnDisable()
	{
		EndWaitLoop();	// 片付け
	}


	/// <summary>
	/// <para>待機中のループ処理</para>
	/// </summary>
	private void AutoWaitLoop()
	{
		// 入力検査
		if (_temporal_input.IsPressed())	// ループ継続
		{
			WaitTurn();	// 待機ターンを自動で開始
		}
		else	// ループ中断
		{
			EndWaitLoop();	// 片付け
		}
	}


	/// <summary>
	/// <para>待機終了後の片付け</para>
	/// </summary>
	private void EndWaitLoop()
	{
		if (SceneLoader.NullCheck && _DungeonScene)	// ヌルチェック
		{
			_DungeonScene.TurnFlow.OnTurnChanged -= AutoWaitLoop;	// ループ呼び出し解除
		}
		_temporal_input.Disable();	// 待機を終えるため無効化制御の抵抗を止める
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
			if (!_temporal_input.enabled && IngameInputManager.Instance.Player.Wait.BaseOne.IsPressed())
			{
				_DungeonScene.TurnFlow.OnTurnChanged += AutoWaitLoop;
				_temporal_input.Enable();	// 離した瞬間を取るため、ターン側の無効化制御に抵抗する
				WaitTurn();	// 待機ループを起動
			}

			// 待機
			yield return null;	// 次フレームを待つ
		}
	}
}