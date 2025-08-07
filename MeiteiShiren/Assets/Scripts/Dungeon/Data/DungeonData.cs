/*=====
<DungeonData.cs>

-author
	mizunose

-about
	ダンジョンのデータを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
/// <summary>
/// <para>ダンジョンデータ</para>
/// </summary>
[CreateAssetMenu(menuName = _NAME, fileName = _NAME)]
public class DungeonData : ScriptableObject
{
	// 定数定義
	private const string _NAME = "Dungeon";	// タブ名称

	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("階層")] private MapData[] _map_datas;
	[SerializeField, Tooltip("操作キャラ")] private ObjectMakeInfo _player;	//TODO:チーム配置

	// プロパティ定義

	/// <summary>
	/// <para>マップのデータ</para>
	/// </summary>
	/// <value>階層に対応したマップのデータ</value>
	public MapData[] MapDatas
	{
		get
		{
			// 提供
			return _map_datas;	// データ配列
		}
	}

	/// <summary>
	/// <para>操作キャラ</para>
	/// </summary>
	/// <value>操作キャラのプレハブ</value>
	public ObjectMakeInfo Player
	{
		get
		{
			// 提供
			return _player;	// データ配列
		}
	}
}