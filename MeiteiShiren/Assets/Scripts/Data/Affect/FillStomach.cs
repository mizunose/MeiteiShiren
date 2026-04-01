/*=====
<FillStomach.cs>

-author
	mizunose

-about
	飢餓解消効果を実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>飢餓解消</para>
/// </summary>
public class FillStomach : Affect
{
	// 定数定義
	private const int _MAX_POINT = 1;	// 回復可能上限

	// 変数宣言
	[Header("パラメータ")]
	[SerializeField, Tooltip("回復値")] private float _value = 0.0f;


	/// <summary>
	/// <para>空腹を回復する効果処理</para>
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
		var _hunger = opponent.GetComponent<Hunger>();	// 回復する飢え

		// 回復処理
		if (_hunger)	// 回復できる
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
			float _corrected_damage = (_value + _all_base_correction) * _all_correction_ratio;	// 補正を反映

			// ダメージ処理
			_hunger.Value -= (int)_corrected_damage;	// ダメージを与える

			// 変数宣言
			GameObject _print_object = new();	// 回復値表示用インスタンス
			WorldLabel _printer = _print_object.AddComponent<WorldLabel>();	// ダメージ表示機能

			// 初期化
			_printer.SetValue($"{_corrected_damage}", opponent.transform);	// ダメージ表示
			_printer.transform.SetParent(opponent.transform, false);	// 親子付け
		}
	}
}