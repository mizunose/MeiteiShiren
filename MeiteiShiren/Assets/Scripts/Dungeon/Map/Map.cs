/*=====
<Map.cs>

-author
	mizunose

-about
	マップを実装
=====*/

// 名前空間宣言
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// クラス定義
/// <summary>
/// <para>マップ</para>
/// </summary>
public class Map : MonoBehaviour
{
	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Start()
	{
		// 変数宣言
		MapData _data = Dungeon.Instance.FloorData.MapData;	// データ

		// マップ生成
		if(_data)	// データ有り
		{
			_data.Generate();	// データからマップを作成
		}
		else	// データ無し
		{
#if UNITY_EDITOR
			Debug.Log("マップデータが不足しています");
#endif	// end UNITY_EDITOR
			return;	// 異常中断
		}

		// 変数宣言
		GameObject _canvas_object = new();	// キャンバス用インスタンス

#if UNITY_EDITOR
		// 初期化
		_canvas_object.name = "MapCanvas";	// デバッグ時にはわかりやすいように命名しておく
#endif	// end UNITY_EDITOR
		_canvas_object.transform.SetParent(transform, false);	// 自身の子に登録

		// 変数宣言
		Canvas _canvas = _canvas_object.AddComponent<Canvas>();	// キャンバス機能

		// 初期化
		_canvas.renderMode = RenderMode.ScreenSpaceOverlay;	// UIを最前面に出す
		_canvas.AddComponent<CanvasScaler>();	// UIのスケール制御
		_canvas.AddComponent<GraphicRaycaster>();	// キャンバスへのレイ判定
		_canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;	// シェーダーセマンティクス：テクスチャ座標
		_canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Normal;	// シェーダーセマンティクス：法線
		_canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Tangent;	// シェーダーセマンティクス：接線

		// 変数宣言
		GameObject _image_object = new(); //画像表示用インスタンス

		// 初期化
#if UNITY_EDITOR
		_image_object.name = "MiniMap";	// デバッグ時にはわかりやすいように命名しておく
#endif	// end UNITY_EDITOR
		_image_object.transform.parent = _canvas_object.transform;	// キャンバスに親子付け

		// 変数宣言
		var _image = _image_object.AddComponent<Image>();	// 画像表示機能

		// 画像読み込み
		_image.material.SetTexture("_MainTex", _data.MiniMapTexture);	// テクスチャ登録
		var _color = _image.color;	// 構造体の取り出し(CS1612エラーの回避)
		_color.a = Settings.Instance.Map.Alpha;	// 表示透明度を変更
		_image.color = _color;	// 変更を反映

		// 変数宣言
		var _image_transform = _image_object.GetComponent<RectTransform>();	// 平面ポリゴン	※自動で付与されているはず
		
		// 初期化
		if (_image_transform != null)	// ヌルチェック
		{
			_image_transform.anchorMin = Vector2.up;	// 左上アンカー
			_image_transform.anchorMax = Vector2.up;	// 左上アンカー
			_image_transform.anchoredPosition = _data.AnchorPosition;	// アンカー基準の位置
			_image_transform.sizeDelta = _data.MiniMapSize;	// 画像サイズ
		}
#if UNITY_EDITOR	// エディタ使用中
		else
		{
			Debug.LogWarning("画像の表示座標を設定できませんでした");
		}
#endif	// end UNITY_EDITOR
	}


	/// <summary>
	/// <para>float型の座標をグリッド座標(マス番号)に変更</para>
	/// </summary>
	/// <param name="position">変換する座標</param>
	/// <returns>変換先データ</returns>
	public static int PositionToMass(float position)
	{
		// 提供
		return (int)((position + Settings.Instance.Map.MassSize / 2.0f) / Settings.Instance.Map.MassSize);	// どのマス目に含まれるかを演算
	}


	/// <summary>
	/// <para>Vector2型の座標をグリッド座標(マス番号)に変更</para>
	/// </summary>
	/// <param name="position">変換する座標</param>
	/// <returns>変換先データ</returns>
	public static Vector2Int PositionToMass(Vector2 position)
	{
		// 提供
		return new Vector2Int(PositionToMass(position.x), PositionToMass(position.y));	// 各要素で座標変換
	}


	/// <summary>
	/// <para>Vector3型の座標をグリッド座標(マス番号)に変更</para>
	/// </summary>
	/// <param name="position">変換する座標</param>
	/// <returns>変換先データ</returns>
	public static Vector2Int PositionToMass(Vector3 position)
	{
		// 提供
		return PositionToMass(new Vector2(position.x, position.z));	// xz面で切り出し、座標変換
	}
}