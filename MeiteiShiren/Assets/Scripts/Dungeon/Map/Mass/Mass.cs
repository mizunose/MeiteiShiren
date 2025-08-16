/*=====
<Mass.cs>

-author
	mizunose

-about
	マス目を実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
/// <summary>
/// マップを構成するマス。子に管理物(乗っているもの)を持つ想定。また、親が構成物(マップや部屋など)である想定。
/// </summary>
public class Mass : MonoBehaviour
{
	// 列挙定義
	public enum Type	// 種類
	{
		GROUND,	// 地面
		PUBLIC_ROOM,	// 通常部屋
		PRIVATE_ROOM,	// 隠し部屋
		SHOP,	// 商店
		WALL,	// 壁
		MAX	// 要素数
	}

	// プロパティ定義
	/// <summary>
	/// <para>マスの種類</para>
	/// </summary>
	/// <value>自身のマスの種類</value>
	public Type type { get; set; } = Type.GROUND;

	// 変数宣言
	private GameObject _above_object;	// 乗っているオブジェクト
	//private Trap _trap = null;	// 付与されている罠
}