/*
<TransitionData.cs>

-author
	mizunose

-about
	トランジションのデータを定義
*/

// 名前空間宣言
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

// クラス定義
/// <summary>
/// <para>トランジションデータ</para>
/// </summary>
public abstract class TransitionData : CreatableData
{
	// 定数定義
#if UNITY_EDITOR
	private const string _CANVAS_NAME = "TransitionCanvas";	// 作成するキャンバスの名前
	private const string _IMAGE_NAME = "TransitionImage";	// 作成する画像の名前
#endif	// end UNITY_EDITOR

	// プロパティ定義

	/// <value>トランジションの値</value>
	protected abstract TransitionPropertiesData _PropertiesData { get; }


	/// <summary>
	/// <para>遷移処理を準備し実処理実行</para>
	/// </summary>
	/// <returns>遅延処理用のインターフェース体</returns>
	public IEnumerator Act()
	{
		// 変数宣言
		GameObject _canvas_object = new();	// キャンバス用インスタンス
		GameObject _image_object = new();	// 描画用インスタンス
		Image _image = _image_object.AddComponent<Image>();	// 画像UI
		//var _volume_data = VolumeManager.instance.stack.GetComponent<CSlideCurtainL2R>();	// ボリュームデータ

		// 初期化
#if UNITY_EDITOR
		_canvas_object.name = _CANVAS_NAME;	// 命名
		_image_object.name = _IMAGE_NAME;	// 命名
#endif	// end UNITY_EDITOR
		// キャンバスの初期化
			_canvas_object.transform.SetParent(SceneLoader.Instance.transform);	// 親子付けして管理
			_canvas_object.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;	// ポストエフェクトの影響を受けないキャンバス
			_canvas_object.AddComponent<CanvasScaler>();	//UIのスケール制御
			_canvas_object.AddComponent<GraphicRaycaster>();	//キャンバスへのレイ判定
		// 画像の初期化
			_image_object.transform.SetParent(_canvas_object.transform);	// キャンバスに登録
			_image.rectTransform.anchorMin = Vector2.zero;
			_image.rectTransform.anchorMax = Vector2.one;
			_image.rectTransform.offsetMin = Vector2.zero;
			_image.rectTransform.offsetMax = Vector2.zero;
			_image.material = CoreUtils.CreateEngineMaterial(_PropertiesData.SupportedShader);	// シェーダーからマテリアル作成
		_canvas_object.layer = LayerMask.NameToLayer("UI");
		_image_object.layer = LayerMask.NameToLayer("UI");

		// 演出
		yield return _Performance(_image.material);	// 演出部実行

		// 破棄
		Destroy(_canvas_object);	// 使用したキャンバスと画像を削除
	}


	/// <summary>
	/// <para>遷移処理の演出部</para>
	/// </summary>
	/// <param name="material">適用済マテリアル</param>
	/// <returns>遅延処理用のインターフェース体</returns>
	protected abstract IEnumerator _Performance(Material material);
}