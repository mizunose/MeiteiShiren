/*=====
<InputOpenMenu.cs>

-author
	mizunose

-about
	メニュー画面を開く入力を実装
=====*/

// 名前空間宣言
using System.Collections;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>メニュー展開入力</para>
/// </summary>
public class InputOpenMenu : MonoBehaviour
{
	// 変数宣言
	[SerializeField, Tooltip("データ")] private InputOpenMenuData _data;


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Start()
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
			if (IngameInputManager.Instance.Player.OpenMenu.BaseOne.triggered)	// メニュー画面展開入力中
			{
				// 入力管理
				IngameInputManager.Instance.TrendDisable();	// 入力系の処理なので干渉権を無効化する

				// 生成
				UIPage.Instance.OpenUI(_data.MenuTabDropDown);
				//Instantiate(_data.MenuTabDropDown);	// 選択UIのインスタンス生成

				// 入力管理
				IngameInputManager.Instance.TrendEnable();	// 干渉権を復権させる
			}

			// 待機
			yield return null;	// 次フレームを待つ
		}
	}
}