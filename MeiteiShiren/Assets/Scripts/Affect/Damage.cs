/*=====
<Damage.cs>

-author
	mizunose

-about
	ダメージ効果を実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
[CreateAssetMenu(menuName = _AFFECT_MENU_TAB_NAME + _AFFECT_NAME, fileName = _AFFECT_NAME)]
public class Damage : Affect
{
	// 定数定義
	private const string _AFFECT_NAME = "Damage";	// 効果名
	private const int _MIN_DAMAGE = 1;	// 最低保証ダメージ

	// 変数宣言
	[Header("パラメータ")]
	[SerializeField, Tooltip("ダメージ値")] private float m_fDamage = 0.0f;
	[Header("状態")]
	[SerializeField, Tooltip("無敵貫通")] private bool m_bIgnoreInvincible = false;
	[SerializeField, Tooltip("ダメージ発生時無敵付与")] private bool m_bGrantInvincible = true;
	[SerializeField, Tooltip("致死性")] private bool m_bKillable = true;

	// プロパティ定義

	/// <summary>
	/// 基礎ダメージプロパティ
	/// </summary>
	/// <value><see cref="m_fDamage"/></value>
	public float BaseDamage	// 無補正ダメージ値
	{
		get
		{
			// 提供
			return m_fDamage;	// 計算前の基礎ダメージを提供
		}
		set
		{
			// 更新
			m_fDamage = value;	// 計算前の基礎ダメージを更新
		}
	}

	/// <summary>
	/// 無敵貫通フラグプロパティ
	/// </summary>
	/// <value><see cref="m_bIgnoreInvincible"/></value>
	public bool IgnoreInvincible
	{
		get
		{
			// 提供
			return m_bIgnoreInvincible;	// 無敵貫通フラグを提供
		}
		set
		{
			// 更新
			m_bIgnoreInvincible = value;	// フラグ値更新
		}
	}

	/// <summary>
	/// 無敵付与フラグプロパティ
	/// </summary>
	/// <value><see cref="m_bGrantInvincible"/></value>
	public bool GrantInvincible
	{
		get
		{
			// 提供
			return m_bGrantInvincible;	// 無敵付与フラグを提供
		}
		set
		{
			// 更新
			m_bGrantInvincible = value;	// フラグ値更新
		}
	}

	/// <summary>
	/// 致死性フラグプロパティ
	/// </summary>
	/// <value><see cref="m_bKillable"/></value>
	public bool Killable
	{
		get
		{
			// 提供
			return m_bKillable;	// 致死性フラグを提供
		}
		set
		{
			// 更新
			m_bKillable = value;	// フラグ値更新
		}
	}


	/// <summary>
	/// -ダメージ効果関数
	/// <para>ダメージを与える効果を行う関数</para>
	/// </summary>
	/// <param name="_Oneself">効果の発動者</param>
	/// <param name="_Opponent">効果の受動者</param>
	public override void Boot(GameObject _Oneself, GameObject _Opponent)
	{
		// 保全
		if(_Opponent == null)	// 相手がいない
		{
#if UNITY_EDITOR
			Debug.Log("効果発動対象が見つかりません");
#endif	// !UNITY_EDITOR
			return;	// 処理中断
		}

		// 変数宣言
		var _HitPoint = _Opponent.GetComponent<HitPoint>();	// ダメージを受けるHP

		// ダメージ処理
		if (_HitPoint)	// ダメージを受けられる
		{
			// 変数宣言
			int _nTemporalHP = _HitPoint.HP;	// 現在HPの退避
			var _Corrections = _Oneself.GetComponentsInChildren<DamageCorrection>();	// 補正値一覧
			float _fAllBaseCorrection = 0.0f;	// 基礎値補正の合計
			float _fAllCorrectionRatio = 1.0f;	// 補正倍率の合計

			// 初期化
			foreach (var _Correction in _Corrections)	// 補正機能単位でのループ
			{
				_fAllBaseCorrection += _Correction.BaseCorrection;	// 基礎値を反映
				_fAllCorrectionRatio *= _Correction.CorrectionRatio;	// 倍率を反映
			}
			
			// 補正
			float _fCorrectedDamage = (BaseDamage + _fAllBaseCorrection) * _fAllCorrectionRatio;	// ダメージ補正を反映

			// ダメージを与える
			if (m_bKillable || _HitPoint.HP - CulcDamage(_fCorrectedDamage, _HitPoint.Defence) > 0)	// 致死性がある・もしくはそもそも殺せていない
			{
				_HitPoint.HP -= CulcDamage(_fCorrectedDamage, _HitPoint.Defence);	// 通常のダメージ処理
			}
			else if(_HitPoint.HP > 0)	// 本来ならこのダメージ処理で死ぬが、非致死性ダメージとして扱う
			{
				_HitPoint.HP = 1;	// 非致死性効果で1耐えさせる
			}
		}
	}
	
	/// <summary>
	/// -ダメージ計算関数
	/// <para>ダメージ処理に必要な情報をすべて揃え、演算する</para>
	/// </summary>
	/// <param name="_fDamageValue">与えるダメージ値</param>
	/// <param name="_fDefence">ダメージ抵抗値</param>
	/// <returns>最終ダメージ</returns>
	private int CulcDamage(float _fDamageValue, float _fDefence)
	{
		// 変数宣言
		int _nResult = 0;	// 演算結果格納用

		// ダメージ計算
		_nResult = (int)(_fDamageValue - _fDefence);	// 最終ダメージを求める

		// 補正
		if(_nResult < _MIN_DAMAGE)	// 最低保証が成立していない
		{
			_nResult = _MIN_DAMAGE;	// ダメージを保証
		}

		// 提供
		return _nResult;	// 最終ダメージ確定
	}
}