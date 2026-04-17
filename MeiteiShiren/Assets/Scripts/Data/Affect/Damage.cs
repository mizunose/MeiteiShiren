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

/// <summary>
/// <para>ダメージ</para>
/// </summary>
public class Damage : Affect
{
	// 定数定義
	private const int _MIN_DAMAGE = 1;	// 最低保証ダメージ

	// 変数宣言
	[Header("パラメータ")]
	[SerializeField, Tooltip("ダメージ値")] private float _value = 0.0f;
	[Header("状態")]
	[SerializeField, Tooltip("防御貫通")] private bool _ignore_defence = false;
	[SerializeField, Tooltip("無敵貫通")] private bool _ignore_invincible = false;
	[SerializeField, Tooltip("致死性")] private bool _killable = true;

	// プロパティ定義

	/// <value><see cref="_value"/></value>
	public float BaseDamage => _value;

	/// <value><see cref="_ignore_invincible"/></value>
	public bool IgnoreInvincible => _ignore_invincible;

	/// <value><see cref="_killable"/></value>
	public bool Killable => _killable;


	/// <summary>
	/// <para>ダメージを与える効果処理</para>
	/// </summary>
	/// <param name="oneself">効果の発動者</param>
	/// <param name="opponent">効果の受動者</param>
	public override void Boot(GameObject oneself, GameObject opponent)
	{
		// 保全
		if(opponent == null)	// 相手がいない
		{
#if UNITY_EDITOR
			Debug.Log("効果発動対象が見つかりません");
#endif	// end UNITY_EDITOR
			return;	// 処理中断
		}

		// 変数宣言
		var _hit_point = opponent.GetComponent<HitPoint>();	// ダメージを受けるHP

		// ダメージ処理
		if (_hit_point)	// ダメージを受けられる
		{
			// 変数宣言
			var _corrections = oneself.GetComponentsInChildren<DamageCorrection>();	// 補正値一覧
			float _all_base_correction = 0.0f;	// 基礎値補正の合計
			float _all_correction_ratio = 1.0f;	// 補正倍率の合計

			// 初期化
			foreach (var _correction in _corrections)	// 補正機能単位でのループ
			{
				_all_base_correction += _correction.BaseCorrection;	// 基礎値を反映
				_all_correction_ratio *= _correction.CorrectionRatio;	// 倍率を反映
			}
			
			// 補正
			float _corrected_damage = (BaseDamage + _all_base_correction) * _all_correction_ratio;	// ダメージ補正を反映

			// ダメージ計算
			int _final_damage = CalculateDamage(_corrected_damage, _hit_point.Data.Defence);	// ダメージ値を算出

			// ダメージ補正
			if (!_killable && _hit_point.HP - _final_damage <= 0)	// 本来ならこのダメージ処理で死ぬが、非致死性ダメージとして扱う
			{
				_final_damage = _hit_point.HP - 1;	// 1残すダメージに補正する
			}

			// ダメージ処理
			_hit_point.HP -= _final_damage;	// ダメージを与える

			// 変数宣言
			GameObject _print_object = new();	// ダメージ表示用インスタンス
			WorldLabel _printer = _print_object.AddComponent<WorldLabel>();	// ダメージ表示機能

			// 初期化
			_printer.SetValue($"{_final_damage}", opponent.transform);	// ダメージ表示
			_printer.transform.SetParent(opponent.transform, false);	// 親子付け
		}
	}


	/// <summary>
	/// <para>ダメージ処理に必要な情報をすべて揃え、演算</para>
	/// </summary>
	/// <param name="damage_value">与えるダメージ値</param>
	/// <param name="defence">ダメージ抵抗値</param>
	/// <returns>最終ダメージ</returns>
	private int CalculateDamage(float damage_value, float defence)
	{
		// 変数宣言
		int _result = 0;	// 演算結果格納用

		// ダメージ計算
		if (_ignore_defence)	// 防御無視
		{
			_result = (int)(damage_value);	// 最終ダメージを求める
		}
		else
		{
			_result = (int)(damage_value - defence);	// 最終ダメージを求める
		}

		// 補正
		if(_result < _MIN_DAMAGE)	// 最低保証が成立していない
		{
			_result = _MIN_DAMAGE;	// ダメージを保証
		}

		// 提供
		return _result;	// 最終ダメージ確定
	}
}