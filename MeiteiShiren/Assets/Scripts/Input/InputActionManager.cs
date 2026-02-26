/*
<InputActionManager.cs>

-author
	mizunose

-about
	入力有効管理
*/

// 名前空間宣言
using System.Collections.Generic;
using UnityEngine.InputSystem;

// クラス定義

/// <summary>
/// <para>入力インスタンスの管理</para>
/// </summary>
public class InputActionManager
{
	// 変数宣言
	private List<InputAction> _disabled_list = new();	// 無効化した入力一覧
	private InputAction _enable_one;	// 現在公開されている入力インスタンス
	private InputAction _base_one;	// 最初の入力インスタンス

	// プロパティ定義

	/// <value><see cref="_base_one"/></value>
	public InputAction BaseOne => _base_one;


	/// <summary>
	/// <para>コンストラクタ</para>
	/// </summary>
	/// <param name="target"></param>
	public InputActionManager(InputAction target)
	{
		// 初期化
		_base_one = target;	// 扱う対象を初期化
		_enable_one = _base_one;	// 対象を公開
	}


	/// <summary>
	/// <para>有効化</para>
	/// </summary>
	public void Enable()
	{
		// 有効化
		_enable_one = _disabled_list?[_disabled_list.Count - 1];	// 有効化するものを扱うものとして設定
		_disabled_list.RemoveAt(_disabled_list.Count - 1);	// 有効化するため無効化リストから除去
		_enable_one?.Enable();	// 入力インスタンスの有効化
	}


	/// <summary>
	/// <para>無効化</para>
	/// </summary>
	public void Disable()
	{
		// 無効化
		_enable_one?.Disable();	// 入力インスタンスの無効化
		_disabled_list?.Add(_enable_one);	// 最新の無効化に追加
	}


	/// <summary>
	/// <para>乗っ取り</para>
	/// </summary>
	/// <returns>乗っ取り後の偽インスタンス</returns>
	public InputAction Jack()
	{
		// 変数宣言
		InputAction _fake_one = new();	// 偽入力インスタンス
		
		// コピー処理
		foreach (var binding in _base_one.bindings)	// 受付入力単位でのループ
		{
			_fake_one.AddBinding(new InputBinding{path = binding.path, interactions = binding.interactions, processors = binding.processors, groups = binding.groups});	// 入力受付を模倣
		}
		
		// 入れ替え
		Disable();	// 現在扱っているものを無効化
		_fake_one.Enable();	// 偽物を有効化
		_enable_one = _fake_one;	// 偽物を入力インスタンスに差し替える

		// 提供
		return _fake_one;	// 生成した入力インスタンスを提供
	}


	/// <summary>
	/// <para>乗っ取り解放</para>
	/// </summary>
	public void JackOff()
	{
		// 差し戻し
		if (_enable_one != _base_one)	// ジャックされている
		{
			_enable_one.Disable();	// 偽物を無効化
			_enable_one.Dispose();	// 解放処理
		}
		Enable();	// ジャック前の状況に戻す
	}
}