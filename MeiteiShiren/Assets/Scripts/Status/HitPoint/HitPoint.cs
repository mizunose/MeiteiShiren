/*=====
<HitPoint.cs>

-author
	mizunose

-about
	生死の状態を実装
=====*/

// 名前空間宣言
using System;
using UnityEngine;

// クラス定義
/// <summary>
/// <para>体力</para>
/// </summary>
public class HitPoint : MonoBehaviour
{
	// イベント定義
	public event Action OnDamaged;	// 被ダメージ時のイベント
	public event Action OnHealed;	// 回復時のイベント
	public event Action OnDead; // 死亡時のイベント

	// 定数定義
	public const int MIN_GUARANTEE_MAX_VALUE = 1;	// 最大体力の最低保証値

	// 変数宣言
	[SerializeField, Tooltip("データ")] private HitPointData _data;
	private int _correct_max_hp = 0;	// 最大HPの補正(一時的なもの)
	private int _lost_hp = 0;	// 消費体力

	// プロパティ定義

	/// <summary>
	/// <para>最大HP</para>
	/// </summary>
	/// <value><see cref="_max_hp"/></value>
	public int MaxHP
	{
		get
		{
			// 変数宣言
			var _level = GetComponent<Level>();	// レベル機能取得
			
			// 提供
			if (_level)	// レベルがある
			{
				return _data.MaxHP + _correct_max_hp + (int)_level.Value;	// レベルを加味した最大HP
			}
			else	// レベルがない
			{
				return _data.MaxHP + _correct_max_hp;	// 最大HP
			}
		}
		set
		{
			// 変数宣言
			int _after_value = value;	// 更新先の値

			// 補正
			if (_after_value < MIN_GUARANTEE_MAX_VALUE)	// 最低保証を突破
			{
				_after_value = MIN_GUARANTEE_MAX_VALUE;	// 最低保証を適用
			}

			// 更新
			_correct_max_hp += _after_value - MaxHP;	//TODO:Correctionの活用

			// イベント判定
			if (HP < 1)	// 最大HPの変動によりHPが無くなった
			{
				if (OnDead != null)	// ヌルチェック
				{
					OnDead.Invoke();	// 死亡時イベントを発行	//TODO:これだと死後蘇生など(とくに処理の途中で一時的に殺したなど)したときに不慮の呼び出しが発生するため要改善
				}
			}
		}
	}
	
	/// <summary>
	/// <para>残りHP</para>
	/// </summary>
	/// <value>最大HPから減少値を除いたもの</value>
	public int HP
	{
		get
		{
			// 提供
			return MaxHP - _lost_hp;	// 現在HP提供
		}
		set
		{
			// 退避
			int _temporal_value = HP;	// 現在値を記録

			// 更新
			_lost_hp = MaxHP - value;	// 回復なら消費をなくし、ダメージなら更に消費する

			// 補正
			if (_lost_hp < 0)	// HPが最大値を超過
			{
				_lost_hp = 0;	// HPが最大値になるよう補正
			}

			// イベント発行
			if (HP < 1)	// HPが無くなった
			{
				if (OnDead != null)	// ヌルチェック
				{
					OnDead.Invoke();	// 死亡時イベントを発行
				}
			}
			else if(HP < _temporal_value)	// ダメージを受けた
			{
				if(OnDamaged != null)	// ヌルチェック
				{
					OnDamaged.Invoke();	// 被ダメージ時イベントを発行
				}
			}
			else if(HP > _temporal_value)	// 回復した
			{
				if(OnHealed != null)	// ヌルチェック
				{
					OnHealed.Invoke();	// 回復時イベントを発行
				}
			}
		}
	}

	/// <summary>
	/// <para>体力データ</para>
	/// </summary>
	/// <value><see cref="_data"/></value>
	public HitPointData Data
	{
		get
		{
			// 提供
			return _data;	// データ
		}
	}
}