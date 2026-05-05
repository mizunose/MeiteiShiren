/*=====
<UIPage.cs>

-author
	mizunose

-about
	UIの表示管理を実装
=====*/

// 名前空間宣言
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>UIの表示ページ</para>
/// </summary>
[RequireComponent(typeof(Canvas))]
public class UIPage : MonoSingleton<UIPage>
{
	// 定数定義
#if UNITY_EDITOR
	private const string _INSTANCE_NAME = "UIPage";	// 自動生成された時のインスタンス名
#endif	// end UNITY_EDITOR

	// 変数宣言
	private UserInterface _current_ui = null;	// 現在操作しているUI
	private Stack<UserInterface> _breadcrumb_trail = new();	// パンくずリスト

	// プロパティ定義

	#if UNITY_EDITOR
/// <value><see cref="_INSTANCE_NAME"/></value>
	protected override string InstanceName => _INSTANCE_NAME;
#endif	// end UNITY_EDITOR


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	protected override void CustomAwake()
	{
		// 入力管理
		UIInputManager.Instance.Page.TrendEnable();	// 有効化時に入力も有効になるように調整

		// 状態管理
		enabled = false;	// 自身の無効化
	}


	/// <summary>
	/// <para>有効化時処理</para>
	/// </summary>
	protected override void OnEnable()
	{
		// 継承
		base.OnEnable();	// 親関数の起動

		// 入力管理
		UIInputManager.Instance.Page.TrendEnable();	// ページ入力有効化
	}


	/// <summary>
	/// <para>無効化時処理</para>
	/// </summary>
	protected override void OnDisable()
	{
		// 継承
		base.OnDisable();	// 親関数の起動

		// 入力管理
		if (UIInputManager.NullCheck)	// ヌルチェック
		{
			UIInputManager.Instance.Page.TrendDisable();	// ページ入力無効化
		}
	}


	/// <summary>
	/// <para>UI展開</para>
	/// </summary>
	/// <param name="target">生成するUI</param>
	/// <param name="is_clear_screen">表示をリセットするか。trueでリセット、falseで維持</param>
	public UIType OpenUI<UIType>(UIType target, bool is_clear_screen = true) where UIType : UserInterface
	{
		// 状態管理
		enabled = true;	// 自身の無効化

		// 
		if (_current_ui)	// ヌルチェック
		{
			_breadcrumb_trail.Push(_current_ui);	// パンくずリストに追加

			if(is_clear_screen)	// オブジェクトごと止める
			{
				_current_ui.gameObject.SetActive(false);
			}
			else	// 機能だけ止める	//TODO:子も止まっているか確認
			{
				_current_ui.enabled = false;
			}
		}

		//生成
		_current_ui = Instantiate(target, transform);	// 選択UIのインスタンス生成

		// 提供
		return _current_ui as UIType;	// 生成したUIを提供
	}


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	protected override void Start()
	{
		// 更新
		StartCoroutine(LateableUpdate());	// 更新処理の軌道
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
			// 入力処理
			if (UIInputManager.Instance.Page.Quit.BaseOne.triggered)	// UI操作終了入力中
			{
				PageClose();	// ページを閉じる
			}
			if (UIInputManager.Instance.Page.Cancel.BaseOne.triggered)	// UI戻し入力中
			{
				// 初期化
				Destroy(_current_ui.gameObject);	// 今のUIを破棄

				if (_breadcrumb_trail.Count > 0)	// 履歴がある
				{
					_current_ui = _breadcrumb_trail.Pop();	// 履歴から抽出する
					_current_ui.gameObject.SetActive(true);	// 
				}
				else	// リストの根元
				{
					PageClose();	// ページを閉じる
				}
			}

			// 待機
			yield return null;	// 次フレームを待つ
		}
	}


	/// <summary>
	/// <para>ページを閉じる処理</para>
	/// </summary>
	public void PageClose()
	{
		// 初期化
		_breadcrumb_trail.Clear();	// 初期化
		for (int _idx = 0; _idx < transform.childCount; _idx++)	// 子オブジェクト単位でのループ	※Destroy後もしばらく残っているのでidxはズレない
		{
			Destroy(transform.GetChild(_idx).gameObject);	// 子オブジェクトを破棄
		}

		// 状態管理
		enabled = false;	// 自身の無効化
	}
}