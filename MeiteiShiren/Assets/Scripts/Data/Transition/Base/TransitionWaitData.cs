/*
<TransitionWaitData.cs>

-author
	mizunose

-about
	トランジションの待機部のデータを定義
*/

// 名前空間宣言
using System.Collections;
using UnityEngine;

// クラス定義
/// <summary>
/// <para>トランジション待機部データ</para>
/// </summary>
//[CreateAssetMenu(menuName = _MENU_TAB_NAME + _NAME, fileName = _NAME)]
public abstract class TransitionWaitData : TransitionData
{
	// 定数定義
	protected const string _MENU_TAB_NAME = TransitionData.TRANSITION_MENU_TAB_NAME + "Wait/";	// アセットメニュー名


	/// <summary>
	/// <para>遷移処理の演出部</para>
	/// </summary>
	/// <param name="material">適用済マテリアル</param>
	/// <returns>遅延処理用のインターフェース体</returns>
	protected sealed override IEnumerator _Performance(Material material)
	{
		// 更新
		while (true)
		{
			// 演出
			_Wait(material);	// 演出処理

			// 提供
			yield return null;	// 次フレームまで待機
		}
	}


	/// <summary>
	/// <para>遷移処理の待機部</para>
	/// </summary>
	/// <param name="material">適用済マテリアル</param>
	/// <returns>遅延処理用のインターフェース体</returns>
	protected abstract void _Wait(Material material);
}