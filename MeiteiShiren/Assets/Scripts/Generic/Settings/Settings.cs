/*=====
<Settings.cs>

-author
	mizunose

-about
	ゲーム全体で共有不変の各種プロパティ値を実装

-note
・autoloadで読み込まれるプレハブの値を使う想定です。無かった場合は参照の際に自動生成されるため各種初期値となります。
・新規データ設計の際には データクラス定義(別ファイル)、変数宣言、プロパティ定義 をすべてする必要があります。
	競合の規模を縮小するため、必ず変数は機能単位のScriptableObjectとしてください。
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>プロパティ値</para>
/// </summary>
public class Settings : MonoSingleton<Settings>
{
	// 定数定義
	private const string _INSTANCE_NAME = "Settings";	// 自動生成された時のインスタンス名

	// 変数宣言
	[Header("ダンジョン")]
	[SerializeField, Tooltip("マップのプロパティ")] private MapSetting _map_setting;
	[SerializeField, Tooltip("敵生成のプロパティ")] private EnemySpawnerSetting _enemy_spawner_setting;
	[SerializeField, Tooltip("移動のプロパティ")] private MoveSetting _move_setting;
	[SerializeField, Tooltip("ワールド空間ラベルのプロパティ")] private WorldLabelSetting _world_label_setting;

	// プロパティ定義

#if UNITY_EDITOR
	/// <value><see cref="_INSTANCE_NAME"/></value>
	protected override string InstanceName => _INSTANCE_NAME;
#endif	// !UNITY_EDITOR

	/// <summary>
	/// <para>マップのプロパティ</para>
	/// </summary>
	/// <value><see cref="_map_setting"/></value>
	public MapSetting Map
	{
		get
		{
			// インスタンス確保
			if (!_map_setting)	// ヌルチェック
			{
				_map_setting = ScriptableObject.CreateInstance<MapSetting>();	// 規定値で作成
			}

			// 提供
			return _map_setting;	// マップのプロパティ一覧
		}
	}

	/// <summary>
	/// <para>敵生成のプロパティ</para>
	/// </summary>
	/// <value><see cref="_enemy_spawner_setting"/></value>
	public EnemySpawnerSetting EnemySpawner
	{
		get
		{
			// インスタンス確保
			if (!_enemy_spawner_setting)	// ヌルチェック
			{
				_enemy_spawner_setting = ScriptableObject.CreateInstance<EnemySpawnerSetting>();	// 規定値で作成
			}

			// 提供
			return _enemy_spawner_setting;	// 敵生成のプロパティ一覧
		}
	}

	/// <summary>
	/// <para>移動のプロパティ</para>
	/// </summary>
	/// <value><see cref="_move_setting"/></value>
	public MoveSetting Move
	{
		get
		{
			// インスタンス確保
			if (!_move_setting)	// ヌルチェック
			{
				_move_setting = ScriptableObject.CreateInstance<MoveSetting>();	// 規定値で作成
			}

			// 提供
			return _move_setting;	// 移動のプロパティ一覧
		}
	}

	/// <summary>
	/// <para>ワールド空間ラベルのプロパティ</para>
	/// </summary>
	/// <value><see cref="_world_label_setting"/></value>
	public WorldLabelSetting WorldLabel
	{
		get
		{
			// インスタンス確保
			if (!_world_label_setting)	// ヌルチェック
			{
				_world_label_setting = ScriptableObject.CreateInstance<WorldLabelSetting>();	// 規定値で作成
			}

			// 提供
			return _world_label_setting;	// ワールド空間ラベルのプロパティ一覧
		}
	}
}