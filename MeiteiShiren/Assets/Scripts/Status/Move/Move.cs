/*=====
<Move.cs>

-author
	mizunose

-about
	移動を定義
=====*/

// 名前空間宣言
using System;
using System.Collections;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>移動</para>
/// </summary>
[DisallowMultipleComponent]
public abstract class Move : MonoBehaviour
{
	// 構造体定義
	/// <summary>
	/// <para>試算データ</para>
	/// </summary>
	public struct SimulatedData
	{
		// 変数宣言
		public Transform next_mass;	// 移動先のマス
		public float direction;	// 終了時点での向き
	}

	// 定数定義
	protected const float _ROUND_DEGREE = 360.0f;	// 円の角度

	// イベント定義
	public event Action OnMoveStarted;	// 移動開始時のイベント


	/// <summary>
	/// <para>試算処理</para>
	/// </summary>
	/// <returns>試算結果</returns>
	public abstract (bool is_actionable, SimulatedData result) Simulate();


	/// <summary>
	/// <para>移動モーション処理</para>
	/// </summary>
	/// <param name="data">試算データ</param>
	/// <returns>遅延処理用のインターフェース</returns>
	public IEnumerator MoveMotion(SimulatedData data)
	{
		// 変数宣言
		Mass _current_mass = GetCurrentMass();	// 現在マス

		// 保全
		if (!_current_mass)	// ヌルチェック
		{
#if UNITY_EDITOR
			Debug.Log("移動元のマスが存在しません");
#endif	// end UNITY_EDITOR
			yield break;	// 移動できないので終了
		}

		// 変数宣言
		Vector3 _at = Vector3.zero;	// 到達地点
		Vector3 _from = _at + (data.next_mass ? _current_mass.transform.position - data.next_mass.transform.position : Vector3.zero);	// 出発地点
		float _front = transform.rotation.eulerAngles.y;	// 出発時点での向き

		// 補正
		if (Mathf.Abs(data.direction - _front) > _ROUND_DEGREE / 2.0f)	// 回転が優角になる
		{
			// 補間用に角度を調整
				// ※eulerAnglesは 0to360 の範囲へと自動で補正されているのでこの範囲外のことは考えなくても良い。
				// ※ただし360を超える代入は禁止されている(ref: https://docs.unity3d.com/ja/2017.4/ScriptReference/Transform-eulerAngles.html#:~:text=%E3%81%A6%E3%81%8F%E3%81%A0%E3%81%95%E3%81%84%E3%80%82-,%E8%A7%92%E5%BA%A6%E3%81%8C360%E5%BA%A6%E3%82%92%E8%B6%85%E3%81%88%E3%82%8B%E3%81%A8%E5%A4%B1%E6%95%97%E3%81%99%E3%82%8B%E3%81%AE%E3%81%A7,-%E3%80%81%E3%82%A4%E3%83%B3%E3%82%AF%E3%83%AA%E3%83%A1%E3%83%B3%E3%83%88%E3%81%97%E3%81%AA%E3%81%84%E3%81%A7)
			if (_front > data.direction)	// _frontを縮めれば劣角に収まる
			{
				_front = _front - _ROUND_DEGREE;	// 劣角回転するように値を調整
			}
			else	// _directionを縮めれば劣角に収まる
			{
				data.direction = data.direction - _ROUND_DEGREE;	// 劣角回転するように値を調整
			}
		}

		// 親の変更
		transform.parent = data.next_mass;	// マスを移るため親を取り替える
		
		// イベント発行
		if (_from != _at)	// 移動するとき
		{
			if (OnMoveStarted != null)	// ヌルチェック
			{
				OnMoveStarted.Invoke();	// 移動開始時のイベントを発行
			}
		}

		// 変数宣言
		float _time = 0.0f;	// 経過時間

		// モーションを取る
		while (true)	// フレーム単位でのループ
		{
			// 更新
			_time += Time.deltaTime;	// 経過時間を測定

			// 変数宣言
			float _timerate = _time / Settings.Instance.Move.Spend;	// 経過時間の割合

			// 補正
			if(_timerate > 1.0f)	// 時間経過が過剰
			{
				_timerate = 1.0f;	// 割合を丸め込む
			}

			// 移動
			transform.localPosition = Vector3.Lerp(_from, _at,  - (Mathf.Cos(Mathf.PI * _timerate) - 1.0f ) /2.0f);	// イージング移動

			// 回転
			var _rotation = transform.rotation;	// 構造体の取り出し(CS1612エラーの回避)
			_rotation.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, Mathf.Lerp(_front, data.direction, 1.0f - Mathf.Pow(1.0f - _timerate, 4.0f)), transform.rotation.eulerAngles.z);	// イージング回転
			transform.rotation = _rotation;	// 変更を反映

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
	}


	/// <summary>
	/// <para>所属マス取得</para>
	/// </summary>
	/// <returns>所属するマスがあればそれを返し、無ければnullを返す</returns>
	protected Mass GetCurrentMass()
	{
		// 変数宣言
		Mass _result = null;	// 演算結果格納用

		// 初期化
		if (transform.parent)	// ヌルチェック
		{
			_result = transform.parent.GetComponent<Mass>();	// 現在マスの取得

#if UNITY_EDITOR
			// 保全
			if(!_result)	// ヌルチェック
			{
				Debug.LogError("親がマスではありません");
			}
#endif	// end UNITY_EDITOR
		}
#if UNITY_EDITOR
		else
		{
			Debug.LogError("管理者がいない独立したオブジェクトなため、マスに所属していません");
		}
#endif	// end UNITY_EDITOR

		// 提供
		return _result;	// 演算結果
	}


	/// <summary>
	/// <para>移動可否検査</para>
	/// </summary>
	/// <param name="target">シミュレーション先</param>
	/// <returns>移動できるときはtrue, できないときはfalse</returns>
	protected bool IsMovable(Mass target)
	{
		// 提供
		return target && target.transform.childCount == 0;	// 移動可否
	}
}