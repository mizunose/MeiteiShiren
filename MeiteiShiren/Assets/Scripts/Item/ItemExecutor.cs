/*=====
<ItemExecutor.cs>

-author
	mizunose

-about
	アイテムの消費場所を実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>自分に使用するアイテム</para>
/// </summary>
public class ItemExecutor : MonoSingleton<ItemExecutor>
{
	// 定数定義
#if UNITY_EDITOR
	private const string _INSTANCE_NAME = "ItemExecuter";	// 自動生成された時のインスタンス名
#endif	// end UNITY_EDITOR

	// プロパティ定義

	#if UNITY_EDITOR
/// <value><see cref="_INSTANCE_NAME"/></value>
	protected override string InstanceName => _INSTANCE_NAME;
#endif	// end UNITY_EDITOR


	/// <summary>
	/// <para>アイテム消費のコルーチンをUIの代わりに負担</para>
	/// </summary>
	/// <param name="item"></param>
	/// <param name="user"></param>
	public void PlayItemMotion(Item item, GameObject user)
	{
		// アイテム消費
		StartCoroutine(item.Use(user));	// アイテムのモーション再生
	}
}