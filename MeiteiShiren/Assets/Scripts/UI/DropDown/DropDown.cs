/*=====
<DropDown.cs>

-author
	mizunose

-about
	ドロップダウン式の選択UIを定義
=====*/

// 名前空間宣言
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// クラス定義

/// <summary>
/// <para>ドロップダウン式選択肢</para>
/// </summary>
[RequireComponent(typeof(VerticalLayoutGroup)), DisallowMultipleComponent]
public abstract class DropDown : UserInterface
{
	// 構造体定義
	/// <summary>
	/// <para>選択候補</para>
	/// </summary>
	[Serializable]
	public class SelectableInformation
	{
		// 変数宣言
		[SerializeField, Tooltip("表示")] public string text;
		[SerializeField, Tooltip("イベント")] public NoneArgumentEventData event_data;
		[SerializeField, Tooltip("サブ階層UI")] public UserInterface sub_ui;
	}

	// イベント定義
	public event Action OnCursorMoved;	// カーソル移動時イベント

	// 変数宣言
	[Header("ステータス")]
	private int _selected_index;	// 選択番号
	private List<ChoiseUI> _text_labels = new();	// テキスト

	// プロパティ定義

	/// <value>データ</value>
	protected abstract DropDownData _Data { get; }

	/// <value>選択肢</value>
	protected abstract List<SelectableInformation> _Choices { get; }

	/// <value><see cref="_selected_index"/></value>
	protected int _SelectedIndex => _selected_index;


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	protected override void Awake()
	{
		// 継承
		base.Awake();	// 親関数の起動

		// 入力管理
		IngameInputManager.Instance.TrendDisable();	// インゲーム入力無効化
		UIInputManager.Instance.DropDown.TrendEnable();	// ドロップダウン入力有効化

		// 初期化
		ResetChoiceUI();	// テキスト領域作成

		// 初期カーソル
		_text_labels[0].Select();	// カーソル位置を選択
	}


	/// <summary>
	/// <para>テキストUIリセット</para>
	/// </summary>
	protected void ResetChoiceUI()
	{
		// 過不足調整
		for(int _idx = _text_labels.Count; _idx < _Choices.Count; _idx++)	// テキスト不足分補給
		{
			// 変数宣言
			ChoiseUI _text_object = Instantiate(_Data.ChoiseUI);	// テキスト作成

			// 初期化
			_text_labels.Add(_text_object);	// リスト登録
			_text_object.transform.SetParent(transform, false);	// 親子付け
		}
		for(int _idx = _text_labels.Count; _idx > _Choices.Count; _idx--)	// テキスト加速分削除
		{
			// 変数宣言
			int _tail_idx = _idx - 1;	// 最後尾の添え字番号

			// 除去
			Destroy(_text_labels[_tail_idx].gameObject);	// インスタンス削除
			_text_labels.RemoveAt(_tail_idx);	// 管理から外す
		}

		// テキスト表示
		for (int _idx = 0; _idx < _Choices.Count; _idx++)	// 選択候補単位でのループ
		{
			_text_labels[_idx].Print(_Choices[_idx].text);	// 表示文字設定
		}
	}


	/// <summary>
	/// <para>更新処理</para>
	/// </summary>
	protected virtual void Update()
	{
		// 入力受付
		if (UIInputManager.Instance.DropDown.Up.BaseOne.triggered)	// 上移動の入力
		{
			// カーソル操作
			MoveCursor(true);	// 上移動
		}
		if (UIInputManager.Instance.DropDown.Down.BaseOne.triggered)	// 下移動の入力
		{
			// カーソル操作
			MoveCursor(false);	// 下移動
		}
		if (UIInputManager.Instance.DropDown.Decide.BaseOne.triggered)	// 決定の入力があった
		{
			#if UNITY_EDITOR
			if (_Choices[_selected_index].event_data == null && _Choices[_selected_index].sub_ui == null)	// することがない
			{
				Debug.LogWarning("UIかイベントを設定してください");
			}
			#endif	// ! UNITY_EDITOR

			// 決定
			_Choices[_selected_index].event_data?.Invoke();	// 採択を通知
			if (_Choices[_selected_index].sub_ui != null)	// 子階層で更に選択する
			{
				// 変数宣言
				var _sub_ui = CreateSubUI();	// サブ階層のUI作成

				// 状態管理
				enabled = false;	// 機能を無効化

				// イベント接続
				_sub_ui.OnDestroyed += () => {
					// 状態管理
					enabled = true;	// 機能を再有効化
				};	// サブのUIから戻ってこれるようにする
			}
			else	// 選択を確定させる
			{
				// 入力管理
				IngameInputManager.Instance.TrendEnable();	// インゲーム入力を復権させる

				// 自身を削除
				Destroy(gameObject);	// 選択肢を削除
			}
		}
	}


	/// <summary>
	/// <para>サブ階層UI作成処理</para>
	/// </summary>
	/// <returns>作成後のUI</returns>
	protected virtual UserInterface CreateSubUI()
	{
		// 提供
		return Instantiate(_Choices[_selected_index].sub_ui, transform);	// サブ階層のUI
	}


	/// <summary>
	/// <para>選択カーソルの移動処理</para>
	/// </summary>
	/// <param name="is_positive">正方向か否か。trueで正(↑)。</param>
	private void MoveCursor(bool is_positive)
	{
		// 変数宣言
		var _past_select = _text_labels[_selected_index];	// カーソル移動前の選択物

		// カーソル移動
		if (is_positive)	// 上移動の入力
		{
			if (_selected_index > 0)	// 上に選択肢がある
			{
				_selected_index--;	// 一つ上に選択を切り替え
			}
			else	// 最上を選択中
			{
				_selected_index = _Choices.Count - 1;	//最下に選択を切り替え
			}
		}
		else	// 下移動の入力
		{
			if (_selected_index < _Choices.Count - 1)	// 下に選択肢がある
			{
				_selected_index++;	// 一つ下に選択を切り替え
			}
			else	// 最下を選択中
			{
				_selected_index = 0;	//最上に選択を切り替え
			}
		}

		// 選択状態更新
		_past_select.Unselect();	// 旧選択状態を解除
		_text_labels[_selected_index].Select();	// 新選択状態を設定

		// イベント発行
		OnCursorMoved?.Invoke();	// カーソル移動時イベント発行
	}
}