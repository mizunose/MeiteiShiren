/*=====
<MoveSetting.cs>

-author
	mizunose

-about
	移動のプロパティ値を実装
=====*/

// 名前空間宣言
using UnityEngine;

/// <summary>
/// <para>移動のプロパティ値</para>
/// </summary>
[CreateAssetMenu(menuName = Settings.SETTING_MENU_TAB_NAME + _NAME, fileName = _NAME)]
public class MoveSetting : ScriptableObject
{
	// 定数定義
	private const string _NAME = "MoveSetting";	// タブ名称

	// 変数宣言
	[SerializeField, Tooltip("再生時間")] private float _spend = 0.25f;
	[SerializeField, Tooltip("再生速度")] private float _spend_ratio = 1.0f;

	// プロパティ定義

	/// <para>移動にかける時間</para>
	/// </summary>
	/// <value>補正済みの移動にかける時間</value>
	public float Spend => _spend * _spend_ratio;
}