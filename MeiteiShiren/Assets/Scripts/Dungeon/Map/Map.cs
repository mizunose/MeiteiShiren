/*=====
<Map.cs>

-author
	mizunose

-about
	マップのデータを定義
=====*/

// 名前空間宣言
using UnityEditor;
using UnityEngine;

// クラス定義
/// <summary>
/// マップデータの抽象クラス
/// </summary>
//[CreateAssetMenu(menuName = _MENU_TAB_NAME + "MapName", fileName = "MapName")]	と子クラスは記述
public class Map : MonoBehaviour
{
	// 定数定義
	protected const string _MENU_TAB_NAME = "MapData/"; // 共通メニュータブ名

	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("データ")] private MapData _data;

	/// <summary>
	/// マップ生成処理
	/// </summary>
	/// <param name="_Oneself">効果の発動者</param>
	/// <param name="_Opponent">効果の受動者</param>
	private void Start()
	{
		if(_data != null)
		{
			_data.Generate();
		}
		else
		{
			Debug.Log("マップデータ不足");
		}
	}
}