/*=====
<DynamicMap.cs>

-author
	mizunose

-about
	動的マップ(自動生成するマップ)のデータを実装
=====*/

// 名前空間宣言
using System;
using System.Collections.Generic;
using UnityEngine;

// クラス定義
/// <summary>
/// 自動生成マップデータ
/// </summary>
[CreateAssetMenu(menuName = _MENU_TAB_NAME + _NAME, fileName = _NAME)]
class DynamicMap : MapData
{
	// 定数定義
	private const string _NAME = "DynamicMap";	// タブ名称
	private const float _MASS_SIZE = 1.0f;	// 1マスあたりの大きさ
	private static readonly Vector3[] _MASS_VERTICES = {	// マスメッシュの頂点情報
			new Vector3(-_MASS_SIZE * 0.5f, 0.0f, _MASS_SIZE * 0.5f),	// 左上
			new Vector3(_MASS_SIZE * 0.5f, 0.0f, _MASS_SIZE * 0.5f),	// 右上
			new Vector3(-_MASS_SIZE * 0.5f, 0.0f, -_MASS_SIZE * 0.5f),	// 左下
			new Vector3(_MASS_SIZE * 0.5f, 0.0f, -_MASS_SIZE * 0.5f),	// 右下
		};
	private static readonly int[] _MASS_INDICES = {	// マスメッシュの頂点インデックス
			0, 1, 2,	// 左側三角形
			1, 3, 2,	// 右側三角形
		};
	private static readonly Vector2[] _MASS_UVS = {	// マスメッシュのテクスチャ座標
			new Vector2(0.0f, 0.0f),	// 左上
			new Vector2(1.0f, 0.0f),	// 右上
			new Vector2(0.0f, 1.0f),	// 左下
			new Vector2(1.0f, 1.0f),	// 右下
		};
	

	/// <summary>
	/// <para>マップを作成</para>
	/// <para>
	/// 生成物は<br></br>
	/// ・部屋<br></br>
	/// ・通路<br></br>
	/// ・プレイヤー生成位置(スタート地点)<br></br>
	/// ・階段(ゴール地点)<br></br>
	/// ・ショップ(部屋設定)<br></br>
	/// </para>
	/// </summary>
	public override void Generate()
	{
		for (int _idx_y = 0; _idx_y < _size.y; _idx_y++)
		{
			for(int _idx_x = 0; _idx_x < _size.x; _idx_x++)
			{
				MakeMass(new Vector3(_idx_x, 0.0f,  _idx_y) * _MASS_SIZE);	// マス作成
			}
		}
	}


	/// <summary>
	/// <para>マスをインスタンスとして作成</para>
	/// </summary>
	/// <param name="position">生成位置</param>
	private void MakeMass(Vector3 position)
	{
		// 変数宣言
		Mesh _mesh = new Mesh();	//メッシュ本体

		// 作成
		GameObject _object = new GameObject();	// マスのインスタンス作成
		var _mesh_filter = _object.AddComponent<MeshFilter>();	// メッシュ管理機能追加
		_object.AddComponent<MeshRenderer>().material = _ground_texture;	// メッシュの描画機能を追加し、その参照マテリアルをマップに合わせて変更

		// 初期化
		_object.transform.position = position;	// 生成位置を設定
		_mesh.vertices = _MASS_VERTICES;	// メッシュの頂点情報を設定
		_mesh.triangles = _MASS_INDICES;	// メッシュの頂点インデックスを設定
		_mesh.RecalculateNormals();	// 法線を再計算
		_mesh.uv = _MASS_UVS;	// テクスチャ座標を設定
		_mesh_filter.sharedMesh = _mesh;	// 作成したメッシュを登録
	}
}