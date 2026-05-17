/*
<InventoryData.cs>

-author
	mizunose

-about
	インベントリのデータ
*/

// 名前空間宣言
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// クラス定義

/// <summary>
/// <para>インベントリのデータ</para>
/// </summary>
public class ItemInventoryData : CreatableData
{
	// イベント定義
	public event Action OnListChanged;	// リスト操作時のイベント

	// 変数宣言
	[SerializeField, Tooltip("捨てる範囲	※データ内での設定順に優先度付けされます")] private MassRange _drop_range;
	private List<Item> _items = new();	// 管理領域


	// プロパティ定義

	/// <value>使用者</value>
	public GameObject User { get; set; }

	/// <value><see cref="_items"/></value>
	public List<Item> ItemInventory => _items;

	/// <value>現在シーンがダンジョンならインスタンスを取得</value>
	protected Dungeon DungeonScene => SceneLoader.Instance.CurrentScene as Dungeon;


	/// <summary>
	/// <para>アイテム保存処理</para>
	/// </summary>
	public void Add(Item item)
	{
		// 更新
		_items.Add(item);	// リスト登録

		// イベント接続
		item.OnDestroyed += ()=>{
			// 更新
			_items.Remove(item);	// 消えるアイテムを管理対象から外す

			// イベント発行
			OnListChanged?.Invoke();	// リスト操作時イベント発行
		};	// アイテムを失ったときリストを更新する

		// イベント発行
		OnListChanged?.Invoke();	// リスト操作時イベント発行
	}


	/// <summary>
	/// <para>リスト初期化</para>
	/// </summary>
	public void Clear()
	{
		// 初期化
		_items.Clear();	// リストをクリアする
		User = null;	// 所有者をリセットする
	}


	/// <summary>
	/// <para>アイテム放棄</para>
	/// </summary>
	/// <param name="item">対象アイテム</param>
	/// <returns>成否</returns>
	public bool Drop(Item item)
	{
		// 変数宣言
		Transform _user_transform = User?.transform;	// 所持者の姿勢情報
		Mass _user_mass = _user_transform?.parent?.GetComponent<Mass>();	// 所持者の居るマス

		// 保全
		if (! _user_transform || !_user_mass)	// 相対座標が無い
		{
			// 終了
			return false;	// 処理できない
		}

		// 変数宣言
		var _dropable_masses = _drop_range.CalculateAroundTarget(_user_mass, Vector2Int.up, DungeonScene.FloorData.MapData.Masses);	// 置くマスの候補
		Mass _drop_mass = null;	// 実際に置くマス

		// 配置可能マス走査
		foreach (var _dropable_mass in _dropable_masses)	// 配置候補マス単位でのループ
		{
			if (!_dropable_mass.AboveItem)	// アイテムを置けるマス
			{
				_drop_mass = _dropable_mass;	// 配置場所として採用
				break;	// ループ終了
			}
		}

		// 保全
		if (!_drop_mass)	// 置く場所がない
		{
			// 終了
			return false;	// 処理失敗
		}

		// 更新
		_drop_mass.AddItem(item);	// アイテムを落とす
		_items.Remove(item);	// 落とすアイテムを管理対象から外す

		// イベント発行
		OnListChanged?.Invoke();	// リスト操作時イベント発行

		// 提供
		return true;	// 処理成功
	}
}