/*=====
<WorldLabel.cs>

-author
	mizunose

-about
	ワールド空間上で配置できるラベルを実装
=====*/

// 名前空間宣言
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// クラス定義
/// <summary>
/// <para>ワールド空間からテキスト表示する</para>
/// </summary>
public class WorldLabel : MonoBehaviour
{
	// 変数宣言
	[Header("ステータス")]
	private float _timer = 0.0f;	// 経過時間
	private TextMeshProUGUI _text_label;	// テキスト
	private Transform _target;	// 表示場所の基準(ワールド空間)
	private Vector3 _shift;	// 表示位置のずらし(アニメーション用)


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	private void Awake()
	{
		// 変数宣言
		GameObject _canvas_object = new();	// キャンバスのインスタンス
		Canvas _canvas = _canvas_object.AddComponent<Canvas>();	// キャンバス機能

		// 初期化
		_canvas_object.transform.SetParent(transform, false);	// 親子付け
		_canvas.renderMode = RenderMode.ScreenSpaceOverlay;	// UIを最前面に出す
		_canvas.AddComponent<CanvasScaler>();	// UIのスケール制御
		_canvas.AddComponent<GraphicRaycaster>();	// キャンバスへのレイ判定
		_canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;	// シェーダーセマンティクス：テクスチャ座標
		_canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Normal;	// シェーダーセマンティクス：法線
		_canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Tangent;	// シェーダーセマンティクス：接線
#if UNITY_EDITOR
		_canvas_object.name = "DamageCanvas";	// デバッグ時にはわかりやすいように命名しておく
#endif	// end UNITY_EDITOR

		// 変数宣言
		GameObject _text_object = new();	// テキストのインスタンス

		// 初期化
		_text_label = _text_object.AddComponent<TextMeshProUGUI>();	// テキスト表示機能
		_text_object.transform.SetParent(_canvas_object.transform, false);	// 親子付け
		if (Settings.Instance.WorldLabel.Font)
		{
			_text_label.font = Settings.Instance.WorldLabel.Font;	// フォント種
		}
#if UNITY_EDITOR
		else
		{
			Debug.LogError("表示フォントを設定してください");
		}
#endif	// end UNITY_EDITOR
		_text_label.fontSize = Settings.Instance.WorldLabel.FontSize;	// フォントの大きさ
		_text_label.alignment = TextAlignmentOptions.Center;	// 中央揃え
#if UNITY_EDITOR
		_text_object.name = "DamageText";	// デバッグ時にはわかりやすいように命名しておく
#endif	// end UNITY_EDITOR
	}


	/// <summary>
	/// <para>更新処理</para>
	/// </summary>
	private void Update()
	{
		// 更新
		ResetPrintPosition();	// 表示位置更新

		// 初期化
		_shift = Settings.Instance.WorldLabel.DefaultShift;	// 表示位置の補正値

		// 状態遷移
		if (_timer < Settings.Instance.WorldLabel.PrintTime)	// 通常表示中
		{
			// 移動値算出
			_shift += Vector3.right * Settings.Instance.WorldLabel.Amplitude * Mathf.Sin(2.0f * Mathf.PI * _timer * Settings.Instance.WorldLabel.Frequency / Settings.Instance.WorldLabel.PrintTime);
		}
		else	// 表示終了
		{
			// 変数宣言
			float _disappear_timer = _timer - Settings.Instance.WorldLabel.PrintTime;	// 消滅アニメーションの経過時間

			// 状態遷移
			if (_disappear_timer < Settings.Instance.WorldLabel.DisappearTime)	// 透過中
			{
				// 表示透過
				_text_label.alpha = 1.0f - _disappear_timer / Settings.Instance.WorldLabel.DisappearTime;	// テキスト透過

				// 移動値算出
				_shift += Vector3.up * Settings.Instance.WorldLabel.RiseSpeed * _disappear_timer;	// 時間経過に合わせて浮く
			}
			else	// 透過完了
			{
				// 消去
				Destroy(gameObject);	// 自滅する
			}
		}

		// カウント
		_timer += Time.deltaTime;	// 時間

		// 補正
		_text_label.rectTransform.position += _shift;	// 移動値を反映
	}


	/// <summary>
	/// <para>テキスト文設定</para>
	/// </summary>
	/// <param name="text">表示内容</param>
	/// <param name="target">表示位置基準</param>
	public void SetValue(string text, Transform target)
	{
		// 初期化
		_target = target;	// 基準値設定
		ResetPrintPosition();	// 表示位置更新
		_text_label.text = text;	// テキスト表示変更
	}


	/// <summary>
	/// <para>ワールド基準値を基に、表示位置をスクリーン位置に移す</para>
	/// </summary>
	private void ResetPrintPosition()
	{
		// 更新
		if (_target)	// 基準が存在
		{
			_text_label.rectTransform.position = Camera.main.WorldToScreenPoint(_target.position);	// 表示位置更新
		}
		else	// 基準が失われた
		{
			_text_label.rectTransform.position -= _shift;	// 移動値から元の値を復元
		}
	}
}