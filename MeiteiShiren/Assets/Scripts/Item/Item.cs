/*=====
<Item.cs>

-author
	mizunose

-about
	アイテムを実装
=====*/

// 名前空間宣言
using UnityEngine;
using System.Collections;

// クラス定義

/// <summary>
/// <para>アイテム</para>
/// </summary>
[DisallowMultipleComponent]
public abstract class Item : MonoBehaviour
{
	// 変数宣言
	[SerializeField, Tooltip("データ")] protected ItemData _data;

	/// <summary>
	/// <para>使用</para>
	/// </summary>
	/// <param name="user">使用者</param>
	/// <returns>遅延処理用のインターフェース体</returns>
	public abstract IEnumerator Use(GameObject user);
}