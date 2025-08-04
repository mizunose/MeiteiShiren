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
public class HitPoint : MonoBehaviour
{
	// 定数定義
	private const int _MIN_GUARANTEE_MAX_VALUE = 1;	// 最大HPの最低保証値

	// イベント定義
	public event Action OnDamaged;	// 被ダメージ時のイベント
	public event Action OnHealed;	// 回復時のイベント
	public event Action OnDead;	// 死亡時のイベント

	// 変数宣言
	[SerializeField, Tooltip("最大体力"), Min(_MIN_GUARANTEE_MAX_VALUE)] private int _max_hp = _MIN_GUARANTEE_MAX_VALUE;
	private int _hp = _MIN_GUARANTEE_MAX_VALUE;	// 体力残量
	[SerializeField, Tooltip("防御")]private int _defence = 0;

	// プロパティ定義

	/// <summary>
	/// 最大HP
	/// </summary>
	/// <value><see cref="_max_hp"/></value>
	public int MaxHP
	{
		get
		{
			// 提供
			return _max_hp;	// 現在HP提供
		}
		set
		{
			// 退避
			var _nTemp = _max_hp;	// 更新前の最大値を退避

			// 更新
			_max_hp = value;	// HPの値を更新

			// 増加分回復
			if (_nTemp < _max_hp)	// 最大値が増加
			{
				// 更新
				_hp += _max_hp - _nTemp;	// 増加分回復

				// イベント発行
				if(OnHealed != null)	// ヌルチェック
				{
					OnHealed.Invoke();	// 回復時イベントを発行
				}
			}

			// 補正
			if (_max_hp < _MIN_GUARANTEE_MAX_VALUE)	// 最低保証を突破
			{
				_max_hp = _MIN_GUARANTEE_MAX_VALUE;	// 最低保証を機能させる
			}

			// レンジ
			if(_max_hp < _hp)	// HP残量が最大値を超過
			{
				_hp = _max_hp;	// 最大値に抑える
			}
		}
	}
	
	/// <summary>
	/// HPプロパティ
	/// </summary>
	/// <value><see cref="_hp"/></value>
	public int HP	// 構造体的に機能を切り出しているのでゲッタ・セッタが必要
	{
		get
		{
			// 提供
			return _hp;	// 現在HP提供
		}
		set
		{
			// 退避
			var _nTemp = _hp;	// 更新前の最大値を退避

			// 更新
			_hp = value;  // HPの値を更新

			// 補正
			if (_max_hp < _hp)	// 最大値を超過
			{
				_hp = _max_hp;	// 最大値に補正
			}

			// イベント判定
			if (_hp < 1)	// HPが無くなった
			{
				if (OnDead != null)	// ヌルチェック
				{
					OnDead.Invoke();	// 死亡時イベントを発行	//TODO:これだと死後蘇生など(とくに処理の途中で一時的に殺したなど)したときに不慮の呼び出しが発生するため要改善
				}
			}
			else if(_hp < _nTemp)	// ダメージを受けた
			{
				if(OnDamaged != null)	// ヌルチェック
				{
					OnDamaged.Invoke();	// 被ダメージ時イベントを発行
				}
			}
			else if(_hp > _nTemp)	// 回復した
			{
				if(OnHealed != null)	// ヌルチェック
				{
					OnHealed.Invoke();	// 回復時イベントを発行
				}
			}
		}
	}

	/// <summary>
	/// 防御プロパティ
	/// </summary>
	/// <value><see cref="_defence"/></value>
	public int Defence	// 構造体的に機能を切り出しているのでゲッタ・セッタが必要
	{
		get
		{
			// 提供
			return _defence;	// 現在HP提供
		}
		set
		{
			// 更新
			_defence = value;  // HPの値を更新
		}
	}
}