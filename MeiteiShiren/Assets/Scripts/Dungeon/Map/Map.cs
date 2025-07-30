/*=====
<Map.cs>

-author
	mizunose

-about
	マップのデータを定義
=====*/

// 名前空間宣言
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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

	public Image img;


	/// <summary>
	/// 初期化処理
	/// </summary>
	private void Start()
	{
		if(_data != null)
		{
			_data.Generate();

			img.material.SetTexture("_MainTex", _data.MapTexture);
		}
		else
		{
			Debug.Log("マップデータ不足");
		}
	}
	

	/// <summary>
	/// 更新処理
	/// </summary>
	private void Update()
	{
		//TODO:レンダーターゲットに描きこみ
	}
}