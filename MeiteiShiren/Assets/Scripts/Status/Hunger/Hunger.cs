/*=====
<Hunger.cs>

-author
	mizunose

-about
	空腹度/満腹度 を実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>飢え</para>
/// </summary>
[DisallowMultipleComponent]
public class Hunger : MonoBehaviour
{
	// 定数定義
	public const int MAX_VALUE = 100;	// 最大値

	// 変数宣言
	private int _value = 0;	// 満腹度
	private uint _turn_count = 0;	// ターンのカウント
	[SerializeField, Tooltip("データ")] private HungerData _data;

	// プロパティ定義

	/// <value>現在シーンがダンジョンならインスタンスを取得</value>
	private Dungeon DungeonScene => SceneLoader.Instance.CurrentScene as Dungeon;

	/// <value>空腹値</value>
	public int Value
	{
		get
		{
			// 提供
			return _value;	// 現在値提供
		}
		set
		{
			// 保全
			if (_value < value)	// 0を下回る
			{
				_value = 0;	// 0で止める
			}

			// 更新
			_value += value;

			// 補正
			if (_value > MAX_VALUE)	// 最大値を超過
			{
				_value = MAX_VALUE;	// 最大値に調整
			}
		}
	}


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Start()
	{
		// 初期化
		_value = 10;	//TODO:ダンジョン突入時は100、それ以外は探索データを用いて初期化

		// イベント接続
		DungeonScene.TurnFlow.OnTurnChanged += OnTurnChanged;	// ターン変更時処理を接続
	}


	/// <summary>
	/// <para>ターン変更時の処理</para>
	/// </summary>
	private void OnTurnChanged()
	{
		// おなかが空く
		if (_value > 0)	// 空腹でない
		{
			// カウント
			_turn_count++;	// 経過ターンをカウント

			// ターン周期の処理
			if (_turn_count == _data.KeepFulling)	// 満腹度に影響を与えるターン
			{
				// 更新
				_value--;	// 満腹度を減らす

				// リセット
				_turn_count = 0;	// ターン数をリセット
			}
		}
		else	// 空腹
		{
			if (_data.Affect)	// ヌルチェック
			{
				_data.Affect.Boot(gameObject, gameObject);	// 空腹時の効果を発動
			}
		}
	}
}