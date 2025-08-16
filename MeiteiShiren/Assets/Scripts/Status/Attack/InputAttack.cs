/*=====
<InputAttack.cs>

-author
	mizunose

-about
	入力攻撃を実装
=====*/

// 名前空間宣言
using System.Collections;
using UnityEngine;

// クラス定義
/// <summary>
/// <para>入力攻撃</para>
/// </summary>
public class InputAttack : Attack
{
	// 定数定義
	private const float _MOTION_SPEND = 1.0f;	// モーションの再生時間


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Start()
	{
		// 更新
		StartCoroutine(LateableUpdate());	// 更新処理の軌道
	}


	/// <summary>
	/// <para>遅延可能な更新処理</para>
	/// </summary>
	/// <returns>遅延処理用のインターフェース</returns>
	private IEnumerator LateableUpdate()
	{
		// フレーム更新
		while (true)
		{
			// 入力処理
			if (IngameInputManager.Instance.Player.Attack.IsPressed())	// 攻撃入力中
			{
				yield return AttackMotion();	// 攻撃モーションを実行 (モーション完了待機)
			}

			// 待機
			yield return null;	// 次フレームを待つ
		}
	}


	/// <summary>
	/// <para>攻撃モーション処理</para>
	/// </summary>
	/// <returns>遅延処理用のインターフェース</returns>
	protected override IEnumerator AttackMotion()
	{
		// 入力管理
		IngameInputManager.Instance.Player.TrendDisable();	// 入力系の処理なのでモーション中は干渉権を無効化する(有効化はターン側の一存に委ねる)
		
		//TODO:モーション再生
		//if (PlayableGraph _playable_graph.IsPlaying) { yield return null; }	// モーション中は待機

		// 代替処理
			// 変数宣言
			Vector3 _at = transform.forward.normalized;	// 到達地点
			Vector3 _from = Vector3.zero;	// 出発地点

			// 変数宣言
			float _time = 0.0f;	// 経過時間

			// 前隙モーションを取る
			while (true)	// フレーム単位でのループ
			{
				// 更新
				_time += Time.deltaTime;	// 経過時間を測定

				// 変数宣言
				float _timerate = _time / (_MOTION_SPEND * 0.5f);	// 経過時間の割合

				// 補正
				if(_timerate > 1.0f)	// 時間経過が過剰
				{
					_timerate = 1.0f;	// 割合を丸め込む
				}

				// 攻撃
				transform.localPosition = Vector3.Lerp(_from, _at,  - (Mathf.Cos(Mathf.PI * _timerate) - 1.0f ) /2.0f);	// イージング攻撃

				// 終了
				if (_timerate == 1.0f)	// モーション完了
				{
					break;	// 処理完了
				}
				else
				{
					// 待機
					yield return null;	// 次フレームを待つ
				}
			}

			// 内容処理
			OnAttacked(transform.eulerAngles.y);	// 攻撃の効果発生タイミング

			// 初期化
			_time = 0.0f;	// 経過時間

			// 後隙モーションを取る
			while (true)	// フレーム単位でのループ
			{
				// 更新
				_time += Time.deltaTime;	// 経過時間を測定

				// 変数宣言
				float _timerate = _time / (_MOTION_SPEND * 0.5f);	// 経過時間の割合

				// 補正
				if(_timerate > 1.0f)	// 時間経過が過剰
				{
					_timerate = 1.0f;	// 割合を丸め込む
				}

				// 攻撃
				transform.localPosition = Vector3.Lerp(_at, _from,  - (Mathf.Cos(Mathf.PI * _timerate) - 1.0f ) /2.0f);	// イージング攻撃

				// 終了
				if (_timerate == 1.0f)	// モーション完了
				{
					break;	// 処理完了
				}
				else
				{
					// 待機
					yield return null;	// 次フレームを待つ
				}
			}

		// 入力管理
		IngameInputManager.Instance.Player.TrendEnable();	// プレイヤーの干渉権を復権させる
	}
}