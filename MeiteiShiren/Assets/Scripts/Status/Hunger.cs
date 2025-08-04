/*=====
<Hunger.cs>

-author
	mizunose

-about
	空腹度/満足度 を実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
/// <summary>
/// <para>飢え</para>
/// </summary>
public class Hunger : MonoBehaviour
{
	// 変数宣言
	[SerializeField, Tooltip("腹が減るターン数"), Min(1)]private uint _keep_fulling = 1;
	private uint _turn_count = 0;	// ターンのカウント
	[SerializeField, Tooltip("空腹時の効果")]private int _affect;


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Start()
	{
		// 

	}


	/// <summary>
	/// <para>ターン変更時の処理</para>
	/// </summary>
	private void OnTurnChanged()
	{
		// カウント
		_turn_count++;	// 経過ターンをカウント

		// 
		if (_turn_count == _keep_fulling)	// 
		{
			// 
			//_affect.Affect(gameObject, gameObject);

			// リセット
			_turn_count = 0;	// ターン数をリセット
		}
	}
}