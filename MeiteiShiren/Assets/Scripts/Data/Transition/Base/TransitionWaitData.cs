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
public abstract class TransitionWaitData : TransitionData
{
	/// <summary>
	/// <para>遷移処理の演出部</para>
	/// </summary>
	/// <param name="material">適用済マテリアル</param>
	/// <returns>遅延処理用のインターフェース体</returns>
	protected sealed override IEnumerator _Performance(Material material)
	{
		// 更新
		while (true)	// ループ
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