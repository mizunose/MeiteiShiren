/*
<EventData.cs>

-author
	mizunose

-about
	イベントのデータ
*/

// 名前空間宣言
using UnityEngine;

// クラス定義
//[CreateAssetMenu(menuName = _MENU_TAB_NAME + _NAME, fileName = _NAME + "Data")]
public abstract class EventData : ScriptableObject
{
	// 定数定義
	protected const string _MENU_TAB_NAME = "Event/";	// アセットメニュー名
}