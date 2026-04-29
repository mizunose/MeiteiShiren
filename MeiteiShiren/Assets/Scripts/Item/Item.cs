/*=====
<Item.cs>

-author
	mizunose

-about
	アイテムを実装
=====*/

// 名前空間宣言
using UnityEngine;
using System;
using System.Collections;

// クラス定義

/// <summary>
/// <para>アイテム</para>
/// </summary>
[DisallowMultipleComponent]
public abstract class Item : MonoBehaviour
{
	// イベント定義
	public event Action OnDestroyed;	// 破棄時イベント

	// 変数宣言
	[SerializeField, Tooltip("データ")] protected ItemData _data;

	// プロパティ定義

	/// <value><see cref="_data"/></value>
	public ItemData Data => _data;


	/// <summary>
	/// <para>使用</para>
	/// </summary>
	/// <param name="user">使用者</param>
	/// <returns>遅延処理用のインターフェース</returns>
	public IEnumerator Use(GameObject user)
	{
		// 更新
		yield return _UseMotion(user);	// 使用モーション再生

		// 終了
		if (!_data.Reusability)	// 使い切り
		{
			Destroy(gameObject);	// 自身を削除
		}
		yield break;
	}


	/// <summary>
	/// <para>使用モーション処理</para>
	/// </summary>
	/// <param name="user">使用者</param>
	/// <returns>遅延処理用のインターフェース</returns>
	protected abstract IEnumerator _UseMotion(GameObject user);


	/// <summary>
	/// <para>破棄時処理</para>
	/// </summary>
	private void OnDestroy()
	{
		// イベント発行
		OnDestroyed?.Invoke();	// 破棄時イベント発行
	}
}