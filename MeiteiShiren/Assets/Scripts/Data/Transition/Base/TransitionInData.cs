/*
<TransitionInData.cs>

-author
	mizunose

-about
・トランジションの抜け部のデータ
*/

// 名前空間宣言
using UnityEngine;
using System.Collections;

// クラス定義

/// <summary>
/// <para>トランジション抜け部データ</para>
/// </summary>
public abstract class TransitionInData : TransitionData
{
	// 変数宣言
	[SerializeField, Tooltip("かける時間")] private float _transition_time = 1.0f;


	/// <summary>
	/// <para>遷移処理の演出部</para>
	/// </summary>
	/// <param name="material">適用済マテリアル</param>
	/// <returns>遅延処理用のインターフェース体</returns>
	protected sealed override IEnumerator _Performance(Material material)
	{
		// 変数宣言
		float _time = 0.0f;	// 時間

		// 更新
		while (_time < _transition_time)	// フレーム単位でのループ
		{
			// カウント
			_time += Time.deltaTime;	// 時間更新

			// 演出
			_In(material, _time / _transition_time);	// 演出処理

			// 提供
			yield return null;	// 次フレームまで待機
		}

		// 提供
		yield break;	// 終了
	}


	/// <summary>
	/// <para>遷移処理の抜け部</para>
	/// </summary>
	/// <param name="material">適用済マテリアル</param>
	/// <param name="time_rate">演出時間の進捗率</param>
	/// <returns>遅延処理用のインターフェース体</returns>
	protected abstract void _In(Material material, float time_rate);
}