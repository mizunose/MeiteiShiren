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
[CreateAssetMenu(menuName = "Datas/" + _NAME, fileName = _NAME)]
public class SceneData : ScriptableObject
{
	// 定数定義
	private const string _NAME = "Scene";	// タブ名称
	
	// 変数宣言
	[Header("ステータス")]
	[SerializeField, Tooltip("配置物")] private GameObject[] _setups;

	// プロパティ定義

	/// <summary>
	/// <para>配置物</para>
	/// </summary>
	/// <value><see cref="_setups"/></value>
	public GameObject[] Setups => _setups;


	/// <summary>
	/// <para>シーンを構築</para>
	/// </summary>
	/// <returns>作成したシーン</returns>
	public GameObject CreateScene()
	{
		// 変数宣言
		GameObject _scene = new GameObject();	// シーン本体

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