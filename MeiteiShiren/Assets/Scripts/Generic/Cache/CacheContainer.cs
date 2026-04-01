/*=====
<CacheContainer.cs>

-author
	mizunose

-about
	キャッシュするオブジェクトの退避場所を実装
=====*/

// 名前空間宣言
using System;

// クラス定義

/// <summary>
/// <para>キャッシュ倉庫</para>
/// </summary>
public class CacheContainer : MonoSingleton<CacheContainer>
{
	// 定数定義
#if UNITY_EDITOR
	private const string _INSTANCE_NAME = "CacheContainer";	// 自動生成された時のインスタンス名
#endif	// end UNITY_EDITOR

	// プロパティ定義

	#if UNITY_EDITOR
/// <value><see cref="_INSTANCE_NAME"/></value>
	protected override string InstanceName => _INSTANCE_NAME;
#endif	// end UNITY_EDITOR


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	protected override void CustomAwake()
	{
		// 初期化
		gameObject.SetActive(false);	// キャッシュしているものを動作させない
	}
}