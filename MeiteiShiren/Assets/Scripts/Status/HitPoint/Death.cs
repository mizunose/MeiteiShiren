/*=====
<Death.cs>

-author
	mizunose

-about
	死亡時の処理を実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>死亡</para>
/// </summary>
[RequireComponent(typeof(HitPoint)), DisallowMultipleComponent]
public class Death : MonoBehaviour
{
	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Start()
	{
		// 変数宣言
		HitPoint _hit_point = GetComponent<HitPoint>();	// 体力

		// イベント接続
		_hit_point.OnDead += OnDead;	// 死亡時処理を接続
	}


	/// <summary>
	/// <para>死亡時処理</para>
	/// </summary>
	private void OnDead()
	{
		// 自壊
		Destroy(gameObject);	// 自滅する
	}
}