/*=====
<ActionMapsManager.cs>

-author
	mizunose

-about
	InputActionMapsの仲介管理
=====*/

// 名前空間宣言
using UnityEngine.InputSystem;

// クラス定義

/// <summary>
/// <para>InputActionMapsの管理</para>
/// </summary>

public abstract class ActionMapsManager<T> : MonoSingleton<T> where T : ActionMapsManager<T>
{
	// 変数宣言
	private int _count = 0;	// 効力カウント	※初期状態(無効)を0とし、それより多い状態を有効、それ以下を無効とする

	// プロパティ定義
	
	/// <value>子クラスで管理するInputMap</value>
	protected abstract IInputActionCollection2 Target { get; }


	/// <summary>
	/// <para>含有Mapを有効状態へ近づける</para>
	/// </summary>
	public abstract void TrendEnable();


	/// <summary>
	/// <para>含有Mapを無効状態へ近づける</para>
	/// </summary>
	public abstract void TrendDisable();
}