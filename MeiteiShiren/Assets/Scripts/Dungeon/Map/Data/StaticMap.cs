/*=====
<StaticMap.cs>

-author
	mizunose

-about
	静的マップ(事前に定まっているマップ)のデータを実装
=====*/

// 名前空間宣言
using UnityEngine;

[CreateAssetMenu(menuName = _MENU_TAB_NAME + _NAME, fileName = _NAME)]
class StaticMap : MapData
{
	// 定数定義
	private const string _NAME = "StaticMap";	// タブ名称
	
	// プロパティ定義
	/// <summary>
	/// マップ全体のサイズ
	/// </summary>
	/// <value>周囲の壁も含めたマップ全体のサイズ</value>
	public override Vector2Int MapSize
	{
		get
		{
			// 提供
			return _size;	// 仮置き
		}
	}

	
	/// <summary>
	/// マップ生成処理
	/// </summary>
	/// <param name="_Oneself">効果の発動者</param>
	/// <param name="_Opponent">効果の受動者</param>
	public override void Generate()
	{
	}
}