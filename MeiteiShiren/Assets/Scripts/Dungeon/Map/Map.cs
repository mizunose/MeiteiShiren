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
	
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("ミニマップ表示透明度")]private const float _alpha = 0.21f;

	public MapData Data { private get; set; }

	public Mass[,] Masses { get { return Data.MapMasses; } }
	public GameObject Player { get { return Data.Player; } }


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Start()
	{
		if(Data != null)
		{
			Data.Generate();
		}
		else
		{
			Debug.Log("マップデータ不足");
		}

		
		
			// 変数宣言
			GameObject _canvas_object = new();	// キャンバスオブジェクト作成
#if UNITY_EDITOR
				_canvas_object.name = "MapCanvas";	// デバッグ時にはわかりやすいように命名しておく
#endif	// end UNITY_EDITOR

			//　キャンバス作成・設定
			Canvas _Canvas = _canvas_object.AddComponent<Canvas>();	// 機能をキャンバス化
			_Canvas.renderMode = RenderMode.ScreenSpaceOverlay;	// UIを最前面に出す
			_Canvas.AddComponent<CanvasScaler>();	// UIのスケール制御
			_Canvas.AddComponent<GraphicRaycaster>();	// キャンバスへのレイ判定
			_Canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;	// シェーダーセマンティクス：テクスチャ座標
			_Canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Normal;	// シェーダーセマンティクス：法線
			_Canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Tangent;	// シェーダーセマンティクス：接線

			// 変数宣言
			GameObject _image_object = new(); //画像表示用オブジェクト

#if UNITY_EDITOR
			_image_object.name = "MiniMap";	// デバッグ時にはわかりやすいように命名しておく
#endif	// end UNITY_EDITOR
			_image_object.transform.parent = _canvas_object.transform;	// 子オブジェクトに追加

			// 画像読み込み
			var _image = _image_object.AddComponent<Image>();
			_image.material.SetTexture("_MainTex", Data.MapTexture);
			var _color = _image.color;
			_color.a = _alpha;
			_image.color = _color;

			// 平面ポリゴン
			var _image_transform = _image_object.GetComponent<RectTransform>();	// 自動で付与されるはず
			if (_image_transform != null)	// ヌルチェック
			{
				_image_transform.anchorMin = Vector2.up;	// 左上アンカー
				_image_transform.anchorMax = Vector2.up;	// 左上アンカー
				_image_transform.anchoredPosition = Data._anchor_position;	// アンカー基準の位置
				_image_transform.sizeDelta = Data._minimap_size;	// 画像サイズ
			}
#if UNITY_EDITOR	// エディタ使用中
			else
			{
				Debug.LogWarning("画像が登録できません");
			}
#endif	// end UNITY_EDITOR
	}
}