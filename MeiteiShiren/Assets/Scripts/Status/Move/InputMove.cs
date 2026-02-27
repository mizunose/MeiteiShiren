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
	// プロパティ定義

	/// <value>現在シーンがダンジョンならインスタンスを取得</value>
	private Dungeon DungeonScene => SceneLoader.Instance.CurrentScene as Dungeon;


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Start()
	{
		// 更新
		StartCoroutine(LateableUpdate());	// 更新処理の起動
	}


	/// <summary>
	/// <para>試算処理</para>
	/// </summary>
	/// <returns>試算結果</returns>
	public override (bool is_actionable, SimulatedData result) Simulate()
	{
		// 変数宣言
		SimulatedData _result = new();	// 演算結果格納用
		Mass _current_mass = GetCurrentMass();	// 現在のマス

		// 保全
		if (!_current_mass)	// ヌルチェック
		{
			// 終了
			return (false, _result);	// マスに居ない以上他のマスへも移れない
		}

		// 変数宣言
		Vector2Int _currect_mass_idx = Map.PositionToMass(_current_mass.transform.position);	// 現在マスの番号
		Vector2 _input = IngameInputManager.Instance.Player.Move.BaseOne.ReadValue<Vector2>() * Settings.Instance.Map.MassSize;	// 入力値
		Vector3 _world_moved = _current_mass.transform.position;	// 移動先のワールド座標

		// 初期化
		_world_moved.x += _input.x;	// 入力値のx射影に沿って移動
		_world_moved.z += _input.y;	// 入力値のy射影に沿って移動

		// 変数宣言
		Vector2Int _next_mass_idx = Map.PositionToMass(_world_moved);	// 移動先のワールド座標から該当マスの番号を取得

		// 保全
		if (_next_mass_idx.x < 0 || _next_mass_idx.x >= DungeonScene.FloorData.MapData.Masses.GetLength(1))	// x軸方向に見てマップ外のマス
		{
			_next_mass_idx.x = _currect_mass_idx.x;	// 移動先として選択させない
		}
		if (_next_mass_idx.y < 0 ||_next_mass_idx.y >= DungeonScene.FloorData.MapData.Masses.GetLength(0))	// y軸方向に見てマップ外のマス
		{
			_next_mass_idx.y = _currect_mass_idx.y;	// 移動先として選択させない
		}

		// 変数宣言
		Mass _next_mass = DungeonScene.FloorData.MapData.Masses[_next_mass_idx.y, _next_mass_idx.x];	// 移動先のマス番号からマス本体を取得

		// 移動可否検査
		if(!IsMovable(_next_mass))	// 移動不可能
		{
			if (_input.x != 0.0f && _input.y != 0.0f)	// 斜め移動で演算していた
			{
				// 更新
				_next_mass = DungeonScene.FloorData.MapData.Masses[_currect_mass_idx.y, _next_mass_idx.x];	// x成分に沿った移動で再度試す

				// 検査
				if(!IsMovable(_next_mass))	// x方向にも移動できない
				{
					// 更新
					_next_mass = DungeonScene.FloorData.MapData.Masses[_next_mass_idx.y, _currect_mass_idx.x];	// y成分に沿った移動で再度試す
					
					// 検査
					if(!IsMovable(_next_mass))	// y方向にも移動できない
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

		// 初期化
		_result.next_mass = _next_mass;	// 移動先を確定

		// 変数宣言
		_result.direction = Vector3.Angle(Vector3.forward, _result.next_mass.transform.position - _current_mass.transform.position);	// 終了時点での向き
		
		// 補正
		if ((_result.next_mass.transform.position - _current_mass.transform.position).x < 0.0f)	// 取得できる角度は 0to180 なので反対方向は補正してあげる必要がある
		{
			_result.direction = _ROUND_DEGREE - _result.direction;	// 180to360の部分の角度として補正する
		}

		// 処理分岐
		if (_result.next_mass.transform.position == _current_mass.transform.position)	// 移動しないとき
		{
			// 補正
			_result.direction = Vector2.Angle(Vector2.up, _input);	// 方向を入力に合わせて調整する
			if (_input.x < 0.0f)	// 取得できる角度は 0to180 なので反対方向は補正してあげる必要がある
			{
				_result.direction = _ROUND_DEGREE - _result.direction;	// 180to360の部分の角度として補正する
			}

			// 終了
			if (transform.rotation.eulerAngles.y == _result.direction)	// 移動も回転もしない
			{
				return (false, _result);	// モーションを取る必要がないので処理を打ち切る
			}
		}

		// 提供
		return (true, _result);	// 演算結果
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
			if (IngameInputManager.Instance.Player.Move.BaseOne.IsPressed())	// 移動入力中
			{
				// 試行(入力値を使用するため入力管理よりも先行)
				var _simulate_data = Simulate();	// 移動のデータ試算を行う

				// 入力管理
				IngameInputManager.Instance.Player.TrendDisable();	// 入力系の処理なのでモーション中は干渉権を無効化する

				// 移動処理
				yield return MoveMotion(_simulate_data.result);	// 移動モーションを実行 (モーション完了待機)

				// 入力管理
				IngameInputManager.Instance.Player.TrendEnable();	// プレイヤーの干渉権を復権させる
			}

			// 待機
			yield return null;	// 次フレームを待つ
		}
	}
}