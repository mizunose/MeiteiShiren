/*=====
<Affects.cs>

-author
	mizunose

-about
	効果一覧を定義
=====*/

// 名前空間宣言
using System;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>効果一覧</para>
/// </summary>
[Serializable]
public class Affects
{
	// 変数宣言
	[Header("発動効果一覧")]
	[SerializeField, Tooltip("効果データ")] public Affect[] _affects;


	/// <summary>
	/// -効果発動関数
	/// <para>紐づいたすべての効果を発動する処理</para>
	/// </summary>
	/// <param name="_Oneself">効果の発動者</param>
	/// <param name="_Opponent">効果の受動者</param>
	public void BootAffects(GameObject _Oneself, GameObject _Opponent)
	{
		// 保全
		if(_affects == null)	// ヌルチェック
		{
			return;	// 処理中断
		}

		// 起動
		foreach (var affect in _affects )	// 紐づいているすべての効果を起動
		{
			if(affect)	// ヌルチェック
			{
				affect.Boot(_Oneself, _Opponent);	// 効果発動
			}
		}
	}
}