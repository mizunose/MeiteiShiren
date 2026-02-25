/*=====
<InputMapManager.cs>

-author
	mizunose

-about
	InputActionMapの管理を実装

-note
・継承先では各InputActionのゲッターと対象Mapのnew(),Dispose()を用意する必要があります
=====*/

// クラス定義
using UnityEngine.InputSystem;

/// <summary>
/// <para>InputActionMapの状態管理</para>
/// </summary>
public abstract class InputMapManager
{
	// 変数宣言
	private int _count = 0;	// 効力カウント	※初期状態(無効)を0とし、それより多い状態を有効、それ以下を無効とする

	// プロパティ定義
	
	/// <value>子クラスで管理するInputMap</value>
	protected abstract InputActionMap Target { get; }


	/// <summary>
	/// <para>有効状態へ近づける。無効状態より優勢になったとき、有効状態へ遷移する</para>
	/// </summary>
	public void TrendEnable()
	{
		// カウント更新
		_count++;	// 有効状態に近づける

		// 状態遷移
		if (!Target.enabled && _count > 0)	// 有効状態に遷移するべき
		{
			Target.Enable();	// 入力を有効化
		}
	}

	
	/// <summary>
	/// <para>無効状態へ近づける。有効状態より優勢になったとき、無効状態へ遷移する</para>
	/// </summary>
	public void TrendDisable()
	{
		// カウント更新
		_count--;	// 無効状態に近づける

		// 状態遷移
		if (Target.enabled && _count <= 0)	// 無効状態に遷移するべき
		{
			Target.Disable();	// 入力を無効化
		}
	}
}