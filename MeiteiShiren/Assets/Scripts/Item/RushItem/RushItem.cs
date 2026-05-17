/*=====
<RushItem.cs>

-author
	mizunose

-about
	突進して効果を与えるアイテムを実装
=====*/

// 名前空間宣言
using System.Collections;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>突進するアイテム</para>
/// </summary>
[RequireComponent(typeof(StraightMove)), DisallowMultipleComponent]
public abstract class RushItem : Item
{
	// 定数定義
	private static readonly SplitedDirections _movable_directions = new EightDirections();	// 移動可能な方向

	// 変数宣言
	[SerializeField, Tooltip("データ")] private RushItemData _data;
	private StraightMove _mover;	// 移動機能

	// プロパティ定義

	/// <value>現在シーンがダンジョンならインスタンスを取得</value>
	protected Dungeon DungeonScene => SceneLoader.Instance.CurrentScene as Dungeon;

	/// <value><see cref="_data"/></value>
	public override ItemData Data => _data;


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	protected virtual void Awake()
	{
		// 初期化
		_mover = GetComponent<StraightMove>();	// 移動機能取得
	}


	/// <summary>
	/// <para>使用モーション処理</para>
	/// </summary>
	/// <param name="user">使用者</param>
	/// <returns>遅延処理用のインターフェース体</returns>
	protected　override IEnumerator _UseMotion(GameObject user)
	{
		// 変数宣言
		var _rush_direction = _movable_directions.CalculateSplitedDirectionInt(user.gameObject.transform.eulerAngles.y);	// 突撃方向
		Mass _user_mass = user.transform.parent.GetComponent<Mass>();	// 使用者が居るマス
		var _start_mass_idx = Map.PositionToMass(_user_mass.transform.position) + _rush_direction;	// モーション開始マス番号
		var _start_mass = DungeonScene.FloorData.MapData.Masses[_start_mass_idx.y, _start_mass_idx.x];	// モーション開始時の配置マス
		Mass _end_mass = null;	// モーション終了時の到達マス

		// 初期化
		transform.SetParent(_start_mass.transform, false);	// マスに顕現する	※使用する時、キャッシュに退避されている可能性が高く、挙動を移すためにマスに現れる必要がある
		transform.eulerAngles = user.transform.eulerAngles;	// 使用者の向きに合わせる
		
		// 移動処理
		while(true)	// 経路マス単位でのループ
		{
			// 変数宣言
			var _move_information = _mover.Simulate();	// 今回の移動の情報

			// 衝突
			if (!_move_information.is_actionable)	// 壁に衝突
			{
				// ループ終了
				break;	// 移動完了
			}
			if (_move_information.result.next_mass.AboveCharacter)	// キャラクタに衝突
			{
				// 効果発動
				_data.Affects.BootAffects(gameObject, user);	// 衝突相手に効果発動

				// 通過性
				if (!_data.IsPenetrating)	// 貫通しない
				{
					// ループ終了
					break;	// 移動完了
				}
			}

			// モーション再生
			yield return _mover.MoveMotion(_move_information.result);	// 移動モーション

			// 更新
			_end_mass = _move_information.result.next_mass;	// 移動した後のマスを記録
		}
		
		// 保全
		if (_end_mass?.AboveCharacter && _end_mass.AboveCharacter == gameObject)	// キャラクタとしてマスに登録済み
		{
			_end_mass.ReleaseCharacter();	// マスの占有を解除する
		}

		// 親子付け
		transform.SetParent(CacheContainer.Instance.transform);	// 非表示になるため、キャッシュへ退避

		// 提供
		yield break;	// 処理終了
	}
}