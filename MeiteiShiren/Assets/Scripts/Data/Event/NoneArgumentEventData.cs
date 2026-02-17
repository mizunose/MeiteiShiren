/*
<NoneArgumentEventData.cs>

-author
	mizunose

-about
	引数無しイベントのデータ
*/

// 名前空間宣言
using System;
using UnityEngine;

// クラス定義
[CreateAssetMenu(menuName = _MENU_TAB_NAME + _NAME, fileName = _NAME + "Data")]
public class NoneArgumentEventData : EventData
{
	// 定数定義
	private const string _NAME = "NoneArgumentEvent";	// アセット名

	// イベント定義
	public event Action signal;	// 扱うイベント


	/// <summary>
	/// <para>イベント実行</para>
	/// </summary>
	public void Invoke()
	{
		// イベント発行
		signal?.Invoke();	// イベントシグナルを発行
	}
}