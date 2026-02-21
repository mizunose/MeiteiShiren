/*
<PublicCanvasData.cs>

-author
	mizunose

-about
	共有キャンバスのデータ
*/

// 名前空間宣言
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// クラス定義
[CreateAssetMenu(menuName = _NAME, fileName = _NAME + "Data")]
public class PublicCanvasData : ScriptableObject
{
	// 定数定義
	protected const string _NAME = "PublicCanvas";	// アセット名

	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("基準となるスクリーンサイズ")] private Vector2 _virtual_size = new Vector2(1920.0f, 1080.0f);
	private Canvas _canvas_instance = null;	// キャンバス

	// プロパティ定義

	/// <value><see cref="_canvas_instance"/></value>
	public Canvas Instance	// 継承先オブジェクトのインスタンス
	{
		get
		{
			// 保全
			if (_canvas_instance == null)	// ヌルチェック
			{
				// 変数宣言
				GameObject _canvas_object = new GameObject();	// インスタンス

				// 初期化
				_canvas_instance = _canvas_object.AddComponent<Canvas>();	// 自身のコンポーネント登録
				_canvas_instance.renderMode = RenderMode.ScreenSpaceOverlay;	// UIを最前面に出す
				_canvas_instance.AddComponent<GraphicRaycaster>();	// キャンバスへのレイ判定
				_canvas_instance.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;	// シェーダーセマンティクス：テクスチャ座標
				_canvas_instance.additionalShaderChannels |= AdditionalCanvasShaderChannels.Normal;	// シェーダーセマンティクス：法線
				_canvas_instance.additionalShaderChannels |= AdditionalCanvasShaderChannels.Tangent;	// シェーダーセマンティクス：接線
			#if UNITY_EDITOR
				_canvas_object.name = "PublicCanvas";	// デバッグ時にはわかりやすいように命名しておく
			#endif	// end UNITY_EDITOR

				// 変数宣言
				var _scaler = _canvas_instance.AddComponent<CanvasScaler>();	// UIのスケール制御

				// 初期化
				_scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;	// スクリーンサイズ参照のフィッティング
				_scaler.referenceResolution = _virtual_size;	// スクリーンサイズを登録
			}

			// 提供
			return _canvas_instance;	// インスタンス提供
		}
	}

	/// <value>基準となるスクリーンサイズ</value>
	public Vector2 VirtualSize => _virtual_size;
}