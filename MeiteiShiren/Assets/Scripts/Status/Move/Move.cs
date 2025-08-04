/*=====
<Move.cs>

-author
	mizunose

-about
	移動を定義
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
/// <summary>
/// <para>プレイヤー</para>
/// </summary>
[CreateAssetMenu(menuName = "move", fileName = "ExperienceTable")]
public abstract class Move : MonoBehaviour
{
	/// <summary>
	/// 定数設定用クラス
	/// </summary>
	public class MoveStatus : MonoSingleton<MoveStatus>
	{
		// 変数宣言
		[SerializeField]private float _spend = 0.25f;
		public static float _spend_correction = 1.0f;

		// プロパティ定義
		public float Spend { get { return _spend * _spend_correction; } }
	}

	public abstract void Action();

}
