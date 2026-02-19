/*=====
<Stair.cs>

-author
	mizunose

-about
	階段を実装
=====*/

// 名前空間宣言
using System;
using UnityEditor;
using UnityEngine;

// クラス定義
/// <summary>
/// <para>階段マス</para>
/// </summary>
public class Stair : Mass
{
	// 定数定義
	private static readonly int[] _INDICES = {	// マスメッシュの頂点インデックス
			0, 1, 2,	// 左側三角形
			1, 3, 2,	// 右側三角形
		};
	private static readonly Vector2[] _UVS = {	// マスメッシュのテクスチャ座標
			new Vector2(0.0f, 0.0f),	// 左上
			new Vector2(1.0f, 0.0f),	// 右上
			new Vector2(0.0f, 1.0f),	// 左下
			new Vector2(1.0f, 1.0f),	// 右下
		};
	private static readonly Vector3[] _VERTICES = {	// マスメッシュの頂点情報
			new Vector3(-Settings.Instance.Map.MassSize * 0.5f, 0.0f, Settings.Instance.Map.MassSize * 0.5f),	// 左上
			new Vector3(Settings.Instance.Map.MassSize * 0.5f, 0.0f, Settings.Instance.Map.MassSize * 0.5f),	// 右上
			new Vector3(-Settings.Instance.Map.MassSize * 0.5f, 0.0f, -Settings.Instance.Map.MassSize * 0.5f),	// 左下
			new Vector3(Settings.Instance.Map.MassSize * 0.5f, 0.0f, -Settings.Instance.Map.MassSize * 0.5f),	// 右下
		};

	// 変数宣言
	private YesNoDropDown _confirm_drop_down;


	/// <summary>
	/// <para>無効時処理</para>
	/// </summary>
	protected override sealed void OnDisable()
	{
		// イベント接続解除
		if (_confirm_drop_down) // ヌルチェック
		{
			_confirm_drop_down.YesEvent.signal -= ConfirmedBootYes;
			_confirm_drop_down.NoEvent.signal -= ConfirmedBootNo;
		}
	}


	/// <summary>
	/// <para>視覚化処理</para>
	/// </summary>
	protected override void Visualize()
	{
		// 変数宣言
		Mesh _mesh = new Mesh();	//メッシュ本体
		var _mesh_filter = gameObject.AddComponent<MeshFilter>();	// メッシュ管理機能

		// メッシュ作成
		gameObject.AddComponent<MeshRenderer>().material = Dungeon.Instance.FloorData.MapData.GroundTexture;	// メッシュの描画機能を追加し、その参照マテリアルをマップに合わせて変更
		_mesh.vertices = _VERTICES;	// メッシュの頂点情報を設定
		_mesh.triangles = _INDICES;	// メッシュの頂点インデックスを設定
		_mesh.RecalculateNormals();	// 法線を再計算
		_mesh.uv = _UVS;	// テクスチャ座標を設定
		_mesh_filter.sharedMesh = _mesh;	// 作成したメッシュを登録
	}


	/// <summary>
	/// <para>機能を起動</para>
	/// </summary>
	public override void Boot()
	{
		// 変数宣言
		_confirm_drop_down = Instantiate(Dungeon.Instance.FloorData.MapData.StairData.ConfirmDropDown);	// 選択UIのインスタンス生成

		// イベント接続
		_confirm_drop_down.YesEvent.signal += ConfirmedBootYes;	// 「はい」選択時の処理
		_confirm_drop_down.NoEvent.signal += ConfirmedBootNo;	// 「いいえ」選択時の処理
	}


	/// <summary>
	/// <para>起動確認で何かを選択</para>
	/// </summary>
	private void ConfirmedBoot()
	{
		Destroy(_confirm_drop_down.gameObject);	// 確認UIを削除
	}


	/// <summary>
	/// <para>起動確認で「はい」を選択</para>
	/// </summary>
	private void ConfirmedBootYes()
	{
		ConfirmedBoot();	// 選択時処理
		Dungeon.Instance.BootSwitchFloor();	// 階層移動を実行
	}


	/// <summary>
	/// <para>起動確認で「いいえ」を選択</para>
	/// </summary>
	private void ConfirmedBootNo()
	{
		ConfirmedBoot();	// 選択時処理
	}
}