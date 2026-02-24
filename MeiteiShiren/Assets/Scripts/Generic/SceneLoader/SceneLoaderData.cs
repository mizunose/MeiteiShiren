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
public class SceneLoaderData : CreatableData
{
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("最初のシーン")]private SceneData _first_scene = null;
	[SerializeField, Tooltip("最初のトランジション")]private TransitionDatas _first_transitions = null;

	//プロパティ定義

	/// <value><see cref="_first_scene"/></value>
	public SceneData FirstScene => _first_scene;

	/// <value><see cref="_first_transitions"/></value>
	public TransitionDatas FirstTransitions => _first_transitions;
}