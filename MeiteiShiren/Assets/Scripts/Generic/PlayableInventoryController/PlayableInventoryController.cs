/*=====
<PlayableInventoryController.cs>

-author
	mizunose

-about
	インベントリ操作GUIを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>インベントリ操作GUI</para>
/// </summary>
public class PlayableInventoryController : Mass
{
	// 変数宣言
	private YesNoDropDown _confirm_drop_down = null;	// 確認用に生成するドロップダウン
	private SystemSpeechBubble _confirm_message_box = null;	// 確認用に生成するメッセージボックス


	/// <summary>
	/// <para>有効時処理</para>
	/// </summary>
	protected override sealed void OnEnable()
	{
		// イベント接続
		if (_confirm_drop_down)	// ヌルチェック
		{
			_confirm_drop_down.YesEvent.signal += ConfirmedBootYes;	// 「はい」選択時の処理を接続
			_confirm_drop_down.NoEvent.signal += ConfirmedBootNo;	// 「いいえ」選択時の処理を接続
		}
	}


	/// <summary>
	/// <para>無効時処理</para>
	/// </summary>
	protected override sealed void OnDisable()
	{
		// イベント接続解除
		if (_confirm_drop_down)	// ヌルチェック
		{
			_confirm_drop_down.YesEvent.signal -= ConfirmedBootYes;	// 「はい」選択時の処理を解除
			_confirm_drop_down.NoEvent.signal -= ConfirmedBootNo;	// 「いいえ」選択時の処理を解除
		}
	}


	/// <summary>
	/// <para>機能を起動</para>
	/// </summary>
	/// <param name="user">起動者</param>
	public override void Boot(Transform user)
	{
		// プレイヤ操作
		if (user == DungeonScene.Player.transform)	// プレイヤーが乗った
		{
			// 生成
			_confirm_drop_down = Instantiate(DungeonScene.FloorData.MapData.StairData.ConfirmDropDown);	// 選択UIのインスタンス生成
			_confirm_message_box = Instantiate(DungeonScene.FloorData.MapData.StairData.ConfirmMessageBox);	// 選択UIのインスタンス生成

			// 初期化
			_confirm_message_box.SetValue(DungeonScene.FloorData.MapData.StairData.ConfirmText);	// 表示テキスト設定

			// イベント接続
			_confirm_drop_down.YesEvent.signal += ConfirmedBootYes;	// 「はい」選択時の処理
			_confirm_drop_down.NoEvent.signal += ConfirmedBootNo;	// 「いいえ」選択時の処理
		}
	}


	/// <summary>
	/// <para>起動確認で何かを選択</para>
	/// </summary>
	private void ConfirmedBoot()
	{
		// 確認UIを削除
		Destroy(_confirm_message_box.gameObject);	// 吹き出しを削除
		Destroy(_confirm_drop_down.gameObject);	// 選択肢を削除
	}


	/// <summary>
	/// <para>起動確認で「はい」を選択</para>
	/// </summary>
	private void ConfirmedBootYes()
	{
		ConfirmedBoot();	// 選択時処理
		DungeonScene.BootSwitchFloor();	// 階層移動を実行
	}


	/// <summary>
	/// <para>起動確認で「いいえ」を選択</para>
	/// </summary>
	private void ConfirmedBootNo()
	{
		ConfirmedBoot();	// 選択時処理
	}
}