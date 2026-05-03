/*
<UserInterface.cs>

-author
	mizunose

-about
	UIを定義
*/

// 名前空間宣言
using System;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>UI</para>
/// </summary>
[DisallowMultipleComponent]
public abstract class UserInterface : MonoBehaviour
{
	// イベント定義
	public event Action OnEnabled;	// 有効化時イベント
	public event Action OnDisabled;	// 無効化時イベント
	public event Action OnDestroyed;	// 破棄時イベント


	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("データ")] private UserInterfaceData _ui_data;


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	protected virtual void Awake()
	{
		// 初期化
		Debug.Log(transform.parent);
		if(!transform.parent)	// 親が設定されていないUI
		{
			transform.SetParent(_ui_data.Canvas.Instance.transform, false);	// 親子付け
		}
	}

	
	/// <summary>
	/// <para>有効時処理</para>
	/// </summary>
	protected virtual void OnEnable()
	{
		Debug.Log("hoooo");
		// イベント発行
		OnEnabled?.Invoke();	// 無効化時イベント発行
	}

	
	/// <summary>
	/// <para>無効時処理</para>
	/// </summary>
	protected virtual void OnDisable()
	{
		// イベント発行
		OnDisabled?.Invoke();	// 無効化時イベント発行
	}


	/// <summary>
	/// <para>破棄時処理</para>
	/// </summary>
	protected virtual void OnDestroy()
	{
		// イベント発行
		OnDestroyed?.Invoke();	// 破棄時イベント発行
	}
}