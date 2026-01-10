/*
<TransitionDatas.cs>

-author
	mizunose

-about
	トランジション用データの羅列群を定義
*/

// 名前空間宣言
using System;
using UnityEngine;

// クラス定義
/// <summary>
/// <para>トランジション用データ群</para>
/// </summary>
[Serializable]
public class TransitionDatas
{
	// 変数宣言
	[SerializeField, Tooltip("入り部分")] private TransitionOutData _out_data = null;
	[SerializeField, Tooltip("待機部分")] private TransitionWaitData _wait_data = null;
	[SerializeField, Tooltip("抜け部分")] private TransitionInData _in_data = null;

	// プロパティ定義

	/// <value><see cref="_out_data"/></value>
	public TransitionOutData OutData => _out_data;

	/// <value><see cref="_wait_data"/></value>
	public TransitionWaitData WaitData => _wait_data;

	/// <value><see cref="_in_data"/></value>
	public TransitionInData InData => _in_data;
}