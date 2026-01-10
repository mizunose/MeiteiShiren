/*=====
<ChaseData.cs>

-author
	mizunose

-about
	シーン切替のデータを実装
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義
/// <summary>
/// <para>シーン切替データ</para>
/// </summary>
[CreateAssetMenu(menuName = _NAME, fileName = _NAME)]
public class SceneLoaderData : ScriptableObject
{
	// 定数定義
	private const string _NAME = "SceneLoader";	// タブ名称

	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("最初のシーン")]private SceneData _first_scene = null;
	[SerializeField, Tooltip("最初のトランジション")]private TransitionDatas _first_transition = null;

	//プロパティ定義

	/// <value><see cref="_first_scene"/></value>
	public SceneData FirstScene => _first_scene;

	/// <value><see cref="_first_transition"/></value>
	public TransitionDatas FirstTransition => _first_transition;
}