/*=====
<StraightMove.cs>

-author
	mizunose

-about
	直線移動を実装

-note
・プロパティの方向値に従って直進します。端につく、または攻撃できる(する意味がある)場合、移動しません。
=====*/

// 名前空間宣言
using System.Collections.Generic;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>入力移動</para>
/// </summary>
public class StraightMove : Move
{
	// 変数宣言
	[SerializeField, Tooltip("データ")] private MoveData _data;

	// プロパティ定義

	/// <value>現在シーンがダンジョンならインスタンスを取得</value>
	private Dungeon DungeonScene => SceneLoader.Instance.CurrentScene as Dungeon;

	/// <value><see cref="_data"/></value>
	protected override MoveData _Data => _data;

	/// <value>移動方向</value>
	public Vector2Int MoveDirection { private get; set;} = Vector2Int.zero;


	/// <summary>
	/// <para>試算処理</para>
	/// </summary>
	/// <returns>試算結果</returns>
	public override (bool is_actionable, SimulatedData result) Simulate()
	{
		// 変数宣言
		SimulatedData _result = new();	// 演算結果格納用
		Mass _current_mass = GetCurrentMass();	// 現在のマス
		Vector2Int _current_mass_idx = Map.PositionToMass(_current_mass.transform.position);	// 現在マスの番号


		// 変数宣言
		Attack _attack = GetComponent<Attack>();	// 攻撃機能

		// 状態遷移
		if (_attack && _attack.Simulate().AreThereAttackable)	// 攻撃できる場合はそれを最優先とする
		{
			// 終了
			return (false, _result);	// 攻撃できる位置から移動する必要がないため中断
		}
		else	// 直進を繰り返す
		{
			// 更新
			Vector2 _shift = _movable_directions.CalculateSplitedDirectionInt(transform.eulerAngles.y);	// 進行方向
			_result.next_mass = CalculateMovedMass(_shift);	// 進行先のマス

			// 終了
			if(!_result.next_mass || _result.next_mass == _current_mass)	// 異常値もしくは移動していない
			{
				return (false, _result);	// 処理しない
			}
		}

		// 変数宣言
		_result.direction = Vector3.Angle(Vector3.forward, _result.next_mass.transform.position - _current_mass.transform.position);	// 終了時点での向き

		// 補正
		if ((_result.next_mass.transform.position - _current_mass.transform.position).x < 0.0f)	// 取得できる角度は 0to180 なので反対方向は補正してあげる必要がある
		{
			_result.direction = _ROUND_DEGREE - _result.direction;	// 180to360の部分の角度として補正する
		}

		// 提供
		return (true, _result);	// 演算結果
	}


	/// <summary>
	/// <para>移動先マス算出</para>
	/// </summary>
	/// <param name="shift_mass">マス上の移動値</param>
	/// <returns>移動先のマス</returns>
	private Mass CalculateMovedMass(Vector2 shift_mass)
	{
		// 変数宣言
		Mass _current_mass = GetCurrentMass();	// 現在マス
		Vector2Int _current_mass_idx = Map.PositionToMass(_current_mass.transform.position);	// 現在マスの番号
		Vector3 _world_moved = _current_mass.transform.position;	// 移動先のワールド座標

		// 初期化
		_world_moved.x += shift_mass.x * Settings.Instance.Map.MassSize;	// 入力値のx射影に沿って移動
		_world_moved.z += shift_mass.y * Settings.Instance.Map.MassSize;	// 入力値のy射影に沿って移動
		
		// 変数宣言
		Vector2Int _next_mass_idx = Map.PositionToMass(_world_moved);	// 移動先のワールド座標から該当マスの番号を取得

		// 保全
		if (_next_mass_idx.x < 0 || _next_mass_idx.x >= DungeonScene.FloorData.MapData.Masses.GetLength(1))	// x軸方向に見てマップ外のマス
		{
			_next_mass_idx.x = _current_mass_idx.x;	// 移動先として選択させない
		}
		if (_next_mass_idx.y < 0 ||_next_mass_idx.y >= DungeonScene.FloorData.MapData.Masses.GetLength(0))	// y軸方向に見てマップ外のマス
		{
			_next_mass_idx.y = _current_mass_idx.y;	// 移動先として選択させない
		}

		// 変数宣言
		Mass _next_mass = DungeonScene.FloorData.MapData.Masses[_next_mass_idx.y, _next_mass_idx.x];	// 移動先のマス番号からマス本体を取得
		
		// 移動可否検査
		if(!IsMovable(_next_mass))	// 移動不可能
		{
			// 更新
			_next_mass = _current_mass;	// 移動出来ないので移動先を元のマスに戻しておく
		}

		// 提供
		return _next_mass;	// 演算結果
	}
}