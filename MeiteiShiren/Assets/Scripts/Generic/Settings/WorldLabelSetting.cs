/*=====
<WorldLabelSetting.cs>

-author
	mizunose

-about
	移動のプロパティ値を実装
=====*/

// 名前空間宣言
using TMPro;
using UnityEngine;

/// <summary>
/// <para>移動のプロパティ値</para>
/// </summary>
[CreateAssetMenu(menuName = Settings.SETTING_MENU_TAB_NAME + _NAME, fileName = _NAME)]
public class WorldLabelSetting : ScriptableObject
{
	// 定数定義
	private const string _NAME = "WorldLabelSetting";   // タブ名称

	// 変数宣言
	[Header("テキスト")]
	[SerializeField, Tooltip("フォント")] private TMP_FontAsset _font = null;
	[SerializeField, Tooltip("フォントサイズ"), Min(0.0f)] private float _font_size = 38.0f;
	[Header("ステータス")]
	[SerializeField, Tooltip("表示位置の補正値")] private Vector3 _default_shift = new Vector3(0.0f, 15.0f, 0.0f);
	[Header("表示中")]
	[SerializeField, Tooltip("表示開始後、消滅開始までの時間")] private float _print_time = 2.0f;
	[SerializeField, Tooltip("表示中のアニメーション振幅"), Min(0.0f)] private float _amplitude;
	[SerializeField, Tooltip("表示中のアニメーション振動数"), Min(0.0f)] private float _frequency;
	[Header("消滅中")]
	[SerializeField, Tooltip("表示終了後、消滅にかける時間")] private float _disappear_time = 1.0f;
	[SerializeField, Tooltip("消滅しながら浮く速度")] private float _rise_spead = 10.0f;

	// プロパティ定義

	/// <summary>
	/// <para>フォント</para>
	/// </summary>
	/// <value><see cref="_font"/></value>
	public TMP_FontAsset Font => _font;

	/// <summary>
	/// <para>フォントサイズ</para>
	/// </summary>
	/// <value><see cref="_font_size"/></value>
	public float FontSize => _font_size;

	/// <summary>
	/// <para>固定補正値</para>
	/// </summary>
	/// <value><see cref="_default_shift"/></value>
	public Vector3 DefaultShift => _default_shift;

	/// <summary>
	/// <para>表示時間</para>
	/// </summary>
	/// <value><see cref="_print_time"/></value>
	public float PrintTime => _print_time;

	/// <summary>
	/// <para>振幅</para>
	/// </summary>
	/// <value><see cref="_amplitude"/></value>
	public float Amplitude => _amplitude;

	/// <summary>
	/// <para>振動数</para>
	/// </summary>
	/// <value><see cref="_frequency"/></value>
	public float Frequency => _frequency;

	/// <summary>
	/// <para>消滅時間</para>
	/// </summary>
	/// <value><see cref="_disappear_time"/></value>
	public float DisappearTime => _disappear_time;

	/// <summary>
	/// <para>浮遊速度</para>
	/// </summary>
	/// <value><see cref="_rise_spead"/></value>
	public float RiseSpeed => _rise_spead;
}