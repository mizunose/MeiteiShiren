/*
<NoneArgumentEventData.cs>

-author
	mizunose

-about
	引数無しイベントのデータ
*/

// 名前空間宣言
using System;

// クラス定義
public class NoneArgumentEventData : EventData
{
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