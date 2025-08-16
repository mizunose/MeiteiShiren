/*=====
<InputMove.cs>

-author
	mizunose

-about
	入力移動を実装
=====*/

// 名前空間宣言
using System.Collections;
using UnityEngine;

// クラス定義
/// <summary>
/// <para>入力移動</para>
/// </summary>
public class InputMove : Move
{
	// 定数定義
	private const float _ROUND_DEGREE = 360.0f;	// 円の角度


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
			if (IngameInputManager.Instance.Player.Move.IsPressed())	// 攻撃入力中
			{
				yield return MoveMotion();	// 攻撃モーションを実行 (モーション完了待機)
			}

			// 待機
			yield return null;	// 次フレームを待つ
		}
	}


	/// <summary>
	/// <para>移動モーション処理</para>
	/// </summary>
	/// <returns>遅延処理用のインターフェース</returns>
	protected override IEnumerator MoveMotion()
	{
		// 変数宣言
		Mass _current_mass;	// 現在のマス

		// 初期化
		if (transform.parent)	// ヌルチェック
		{
			_current_mass = transform.parent.GetComponent<Mass>();	// 現在マスの取得

			// 保全
			if(!_current_mass)	// ヌルチェック
			{
#if UNITY_EDITOR
				Debug.LogError("親がマスではありません");
#endif	// end UNITY_EDITOR

				// 終了
				yield break;	// 移動先を決められないため中断
			}
		}
		else
		{
#if UNITY_EDITOR
			Debug.LogError("所属マスが特定できませんでした");
#endif	// end UNITY_EDITOR

			// 終了
			yield break;	// 移動先を決められないため中断
		}

		// 変数宣言
		Vector2Int _currect_mass_idx = Map.PositionToMass(_current_mass.transform.position);	// 現在マスの番号
		Vector2 _input = IngameInputManager.Instance.Player.Move.ReadValue<Vector2>();	// 入力値
		Vector3 _world_moved = _current_mass.transform.position;	// 移動先のワールド座標

		// 初期化
		_world_moved.x += _input.x;	// 入力値のx射影に沿って移動
		_world_moved.z += _input.y;	// 入力値のy射影に沿って移動

		// 変数宣言
		Vector2Int _next_mass_idx = Map.PositionToMass(_world_moved);	// 移動先のワールド座標から該当マスの番号を取得

		// 保全
		if (_next_mass_idx.x < 0 || _next_mass_idx.x >= Dungeon.Instance.Map.Masses.GetLength(1))	// x軸方向に見てマップ外のマス
		{
			_next_mass_idx.x = _currect_mass_idx.x;	// 移動先として選択させない
		}
		if (_next_mass_idx.y < 0 ||_next_mass_idx.y >= Dungeon.Instance.Map.Masses.GetLength(0))	// y軸方向に見てマップ外のマス
		{
			_next_mass_idx.y = _currect_mass_idx.y;	// 移動先として選択させない
		}

		// 変数宣言
		Mass _next_mass = Dungeon.Instance.Map.Masses[_next_mass_idx.y, _next_mass_idx.x];	// 移動先のマス番号からマス本体を取得

		// 移動可否検査
		if(!_next_mass || _next_mass.type == Mass.Type.WALL)	// 移動不可能	//TODO:object->GetComponent<Wall>() ? null
		{
			if (_input.x != 0.0f && _input.y != 0.0f)	// 斜め移動で演算していた
			{
				// 更新
				_next_mass = Dungeon.Instance.Map.Masses[_currect_mass_idx.y, _next_mass_idx.x];	// x成分に沿った移動で再度試す

				// 検査
				if(!_next_mass || _next_mass.type == Mass.Type.WALL)	// x方向にも移動できない
				{
					// 更新
					_next_mass = Dungeon.Instance.Map.Masses[_next_mass_idx.y, _currect_mass_idx.x];	// y成分に沿った移動で再度試す
					
					// 検査
					if(!_next_mass || _next_mass.type == Mass.Type.WALL)	// y方向にも移動できない
					{
						_next_mass = _current_mass;	// 移動出来ないので移動先を元のマスに戻しておく
					}
				}
			}
			else	// 軸に沿って演算していた
			{
				// 更新
				_next_mass = _current_mass;	// 移動出来ないので移動先を元のマスに戻しておく
			}
		}

		// 親の変更
		transform.parent = _next_mass.transform;	// マスを移るため親を取り替える

		// 変数宣言
		Vector3 _at = Vector3.zero;	// 到達地点
		Vector3 _from = _at + _current_mass.transform.position - _next_mass.transform.position;	// 出発地点
		float _front = transform.rotation.eulerAngles.y;	// 出発時点での向き
		float _direction = Vector3.Angle(Vector3.forward, _at - _from);	// 終了時点での向き

		// 補正
		if ((_at - _from).x < 0.0f)	// 取得できる角度は 0to180 なので反対方向は補正してあげる必要がある
		{
			_direction = _ROUND_DEGREE - _direction;	// 180to360の部分の角度として補正する
		}

		// 処理分岐
		if (_from == _at)	// 移動しないとき
		{
			// 補正
			_direction = Vector2.Angle(Vector2.up, _input);	// 方向を入力に合わせて調整する
			if (_input.x < 0.0f)	// 取得できる角度は 0to180 なので反対方向は補正してあげる必要がある
			{
				_direction = _ROUND_DEGREE - _direction;	// 180to360の部分の角度として補正する
			}

			// 終了
			if (_front == _direction)	// 移動も回転もしない
			{
				yield break;	// モーションを取る必要がないので処理を打ち切る
			}
		}
		else	// 移動するとき
		{
			EmitOnMoveStarted();	// これから移動が開始される
		}
		
		// 補正
		if (Mathf.Abs(_direction - _front) > _ROUND_DEGREE / 2.0f)	// 回転が優角になる
		{
			// 補間用に角度を調整
				// ※eulerAnglesは 0to360 の範囲へと自動で補正されているのでこの範囲外のことは考えなくても良い。
				// ※ただし360を超える代入は禁止されている(ref: https://docs.unity3d.com/ja/2017.4/ScriptReference/Transform-eulerAngles.html#:~:text=%E3%81%A6%E3%81%8F%E3%81%A0%E3%81%95%E3%81%84%E3%80%82-,%E8%A7%92%E5%BA%A6%E3%81%8C360%E5%BA%A6%E3%82%92%E8%B6%85%E3%81%88%E3%82%8B%E3%81%A8%E5%A4%B1%E6%95%97%E3%81%99%E3%82%8B%E3%81%AE%E3%81%A7,-%E3%80%81%E3%82%A4%E3%83%B3%E3%82%AF%E3%83%AA%E3%83%A1%E3%83%B3%E3%83%88%E3%81%97%E3%81%AA%E3%81%84%E3%81%A7)
			if (_front > _direction)	// _frontを縮めれば劣角に収まる
			{
				_front = _front - _ROUND_DEGREE;	// 劣角回転するように値を調整
			}
			else	// _directionを縮めれば劣角に収まる
			{
				_direction = _direction - _ROUND_DEGREE;	// 劣角回転するように値を調整
			}
		}

		// 入力管理
		IngameInputManager.Instance.Player.TrendDisable();	// 入力系の処理なのでモーション中は干渉権を無効化する

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
			_rotation.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, Mathf.Lerp(_front, _direction, 1.0f - Mathf.Pow(1.0f - _timerate, 4.0f)), transform.rotation.eulerAngles.z);	// イージング回転
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

		// 入力管理
		IngameInputManager.Instance.Player.TrendEnable();	// プレイヤーの干渉権を復権させる
	}
}