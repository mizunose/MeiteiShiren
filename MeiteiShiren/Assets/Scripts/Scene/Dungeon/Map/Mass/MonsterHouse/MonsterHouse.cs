/*=====
<MonsterHouse.cs>

-author
	mizunose

-about
	モンスターハウスを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>モンスターハウス</para>
/// </summary>
public class MonsterHouse : Mass
{
	// 変数宣言
	private bool _is_active = true;

	// プロパティ定義

	/// <value>特殊モンスターハウス情報</value>
	public SpecialMonsterHouseData SpecialData { private get; set;} = null;


	/// <summary>
	/// <para>機能を起動</para>
	/// </summary>
	/// <param name="user">起動者</param>
	public override void Boot(Transform user)
	{
		// 継承
		base.Boot(user);	// 親クラスの実行

		// 起動
		if (_is_active && user.GetComponent<Camp>()?.Type == CampData.CampType.Comrade)	// 有効時に味方陣営が起動した
		{
			//TODO:モンスターハウスコール→メッセージログ

			// 変数宣言
			var _room = GetComponentInParent<Room>();	// 所属する部屋を取得
			var _room_masses = _room.GetComponentsInChildren<MonsterHouse>();	// 連動するモンスターハウスを取得

			// 誘発
			foreach (var room_mass in _room_masses)	// 連動するマス単位でのループ
			{
				room_mass.MonsterSpwan();	// 召喚を実行
			}
		}
	}


	/// <summary>
	/// <para>敵の追加召喚</para>
	/// </summary>
	public void MonsterSpwan()
	{
		if (_is_active)	// 有効時
		{
			// 敵生成
			if (!AboveCharacter)	// キャラが乗っていない
			{
				// 変数宣言
				GameObject _spawn_target;	// 生成対象

				// 抽選
				if (SpecialData?.SpawnableEnemies != null && SpecialData.SpawnableEnemies.Count > 0)	// 生成対象が限定されている
				{
					// 更新
					_spawn_target = SpecialData.SpawnableEnemies[UnityEngine.Random.Range(0, SpecialData.SpawnableEnemies.Count)];	// 生成可能対象に応じてランダムに決定
				}
				else
				{
					// 変数宣言
					var _samples = _DungeonScene.FloorData.EnemySpawnData.Enemies;	// 生成対象一覧

					// 更新
					_spawn_target = _samples[UnityEngine.Random.Range(0, _samples.Length)];	// ダンジョンに応じてランダムに決定
				}

				//変数宣言
				var _new_enemy = Instantiate(_spawn_target);	// 敵インスタンス

				// 初期化
				AddCharacter( _new_enemy );	// 生成対象を自身に乗せる
			}

			// フラグ管理
			_is_active = false;	// 生成の成否にかかわらず無効化
		}
	}
}