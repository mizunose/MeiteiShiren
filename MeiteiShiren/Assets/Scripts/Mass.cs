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
/// マップを構成するマス。子に管理物(乗っているもの)を持つ想定。また、親がMapである想定。
/// </summary>
public class Mass : MonoBehaviour
{
	// 列挙定義
	public enum TYPE	// 種類
	{
		GROUND,	// 地面
		ROOM,	// 地面
		WALL,	// 壁
		MAX	// 要素数
	}

	// プロパティ定義
	public TYPE type { get; set; } = TYPE.GROUND;	// 
	


	// Start is called once before the first execution of Update after the MonoBehaviour is created

	GameObject g;


	void Start()
	{
		GameObject.Instantiate(g, Vector3.zero, Quaternion.identity);
	}


	// Update is called once per frame
	void Update()
	{
		
	}
}



	//// クラス定義
	//[CreateAssetMenu(menuName = AFFECT_MENU_TAB_NAME + "AffectName", fileName = "AffectName")]
	//public class CAffect : ScriptableObject
	//{
	//	// 定数定義
	//	public const string AFFECT_MENU_TAB_NAME = "Affect/";	// 共通メニュータブ名	※MenuItemの定義場所的にpublicでないと厳しい

	//	public string menuName;

	//	/// <summary>
	//	/// -効果関数
	//	/// <para>各効果の呼び出し共通化のための抽象関数</para>
	//	/// </summary>
	//	/// <param name="_Oneself">効果の発動者</param>
	//	/// <param name="_Opponent">効果の受動者</param>
	//	public void Affect(GameObject _Oneself, GameObject _Opponent){}
	//}