/*
<SceneData.cs>

-author
	mizunose

-about
	シーンのデータを実装
*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>シーンデータ</para>
/// </summary>
public class SceneData :  CreatableData
{
	// 定数定義
#if UNITY_EDITOR
	private const string _SCENE_NAME = "Scene";	// 自動生成された時のインスタンス名
#endif	// end UNITY_EDITOR

	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("シーン本体")] private Scene _scene_prefab;
	[SerializeField, Tooltip("配置物")] private GameObject[] _setups;

	// プロパティ定義

	/// <value><see cref="_setups"/></value>
	public GameObject[] Setups => _setups;


	/// <summary>
	/// <para>シーンを構築</para>
	/// </summary>
	/// <returns>作成したシーン</returns>
	public Scene CreateScene()
	{
		// 変数宣言
		Scene _scene = Instantiate(_scene_prefab);	// シーン本体


		// 初期化
#if UNITY_EDITOR
		_scene.name = _SCENE_NAME;	// 命名
#endif	// end UNITY_EDITOR

		// 配置
		foreach (var setup in _setups)	// 配置物単位でのループ
		{
			// 生成
			Instantiate(setup, _scene.transform);	// シーンに配置
		}

		// 提供
		return _scene;	// 作成が完了したシーンを提供
	}
}