/*
<SceneLoader.cs>

-author
	mizunose

-about
	シーン切替を実装
*/

// 名前空間宣言
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// クラス定義
/// <summary>
/// <para>シーン切替</para>
/// </summary>
public class SceneLoader : MonoSingleton<SceneLoader>
{
	// 定数定義
	#if UNITY_EDITOR
	private const string _INSTANCE_NAME = "SceneLoader";	// 自動生成された時のインスタンス名
	#endif	// end UNITY_EDITOR

	// 変数宣言
	[SerializeField, Tooltip("データ")]private SceneLoaderData _data = null;
	private GameObject _current_scene = null;	// 現在のシーン
	private List<GameObject> _breadcrumb_list = new();	// 保留シーンのパンくずリスト

	// プロパティ定義

	#if UNITY_EDITOR
	/// <value><see cref="_INSTANCE_NAME"/></value>
	protected override string InstanceName => _INSTANCE_NAME;
	#endif	// !UNITY_EDITOR

	/// <value><see cref="_current_scene"/></value>
	public GameObject CurrentScene => _current_scene;


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	protected override sealed void Start()
	{
		// 初期化
		BootChangeScene(_data.FirstScene, _data.FirstTransitions);	// 初期シーンを展開
	}


	/// <summary>
	/// <para>シーン遷移の起動</para>
	/// </summary>
	/// <param name="next_scene">遷移先のシーン</param>
	/// <param name="transitions">遷移演出データ</param>
	public void BootChangeScene(SceneData next_scene, TransitionDatas transitions)
	{
		StartCoroutine(ChangeScene(next_scene, transitions));	// 初期シーンを展開
	}


	/// <summary>
	/// <para>シーン遷移処理(現在シーンを廃棄する)</para>
	/// </summary>
	/// <param name="next_scene">遷移先のシーン</param>
	/// <param name="transitions">遷移演出データ</param>
	/// <returns>遅延処理用のインターフェース体</returns>
	private IEnumerator ChangeScene(SceneData next_scene, TransitionDatas transitions)
	{
		//TODO:新旧シーンを止めておく

		// 遷移開始
		if (transitions?.OutData)	// ヌルチェック
		{
			yield return transitions.OutData.Act();	// 画面転換アニメーション
		}

		// 変数宣言
		Coroutine _coroutine = null;	// 待機画面の描画スレッド
		
		// 待機開始
		if (transitions?.WaitData) // ヌルチェック
		{
			_coroutine = StartCoroutine(transitions.WaitData.Act());	// 待機画面表示
		}

		// シーン転換
		if (_current_scene)	// ヌルチェック
		{
			Destroy(_current_scene.gameObject);	// 現在シーンを破棄

			yield return null;
		}
		_current_scene = next_scene.CreateScene();	// 新規シーンを作成
		//_current_scene.SetUp();	// 新規シーンの準備

		// 待機終了
		if (_coroutine != null)	// ヌルチェック
		{
			StopCoroutine(_coroutine);	// コルーチンを止める
		}

		// 遷移再開
		if (transitions?.InData)	// ヌルチェック
		{
			yield return transitions?.InData.Act();	// 画面転換アニメーション
		}

		//TODO:新シーンを動かす

		// 遷移完了
		yield break;	// 処理終了
	}


	/// <summary>
	/// <para>シーン展開の起動</para>
	/// </summary>
	/// <param name="next_scene">展開するシーン</param>
	/// <param name="transitions">遷移演出データ</param>
	public void BootOpenScene(SceneData next_scene, TransitionDatas transitions)
	{
		StartCoroutine(OpenScene(next_scene, transitions));	// 初期シーンを展開
	}


	/// <summary>
	/// <para>シーン展開処理(現在シーンを保存する)</para>
	/// </summary>
	/// <param name="next_scene">展開するシーン</param>
	/// <param name="transitions">遷移演出データ</param>
	/// <returns>遅延処理用のインターフェース体</returns>
	private IEnumerator OpenScene(SceneData next_scene, TransitionDatas transitions)
	{
		//TODO:新旧シーンを止めておく

		//TODO:どこのトランジションを起用するかはもう少し煮詰める必要あり
		// 遷移開始
		yield return transitions?.OutData?.Act();	// 画面転換アニメーション
		
		// 変数宣言
		var _coroutine = StartCoroutine(transitions?.WaitData?.Act());	// 待機画面表示

		// 経路更新
		_breadcrumb_list.Add(_current_scene);	// パンくずリストの末尾を追加
		
		// シーン転換
		if (_current_scene)	// ヌルチェック
		{
			_current_scene.gameObject.SetActive(false);	// 無効化
		}
		_current_scene = next_scene.CreateScene();	// 新規シーンを作成
		//next_scene.SetUp();	// 新規シーンの準備

		// 待機終了
		StopCoroutine(_coroutine);	// コルーチンを止める
		
		// 遷移再開
		yield return transitions?.InData?.Act();	// 画面転換アニメーション

		//TODO:新シーンを動かす

		// 遷移完了
		yield break;	// 処理終了
	}


	/// <summary>
	/// <para>シーン閉鎖の起動</para>
	/// </summary>
	/// <param name="transitions">遷移演出データ</param>
	public void BootCloseScene(TransitionDatas transitions)
	{
		StartCoroutine(CloseScene(transitions));	// 初期シーンを展開
	}


	/// <summary>
	/// <para>シーン閉鎖処理(過去シーンに回帰する)</para>
	/// </summary>
	/// <param name="transitions">遷移演出データ</param>
	/// <returns>遅延処理用のインターフェース体</returns>
	private IEnumerator CloseScene(TransitionDatas transitions)
	{
		// 保全
		if (_breadcrumb_list.Count < 1)	// 戻るシーンがない
		{
			#if UNITY_EDITOR
			Debug.LogError("展開されていない状態でシーンが閉じられています");
			#endif	// end UNITY_EDITOR

			// 終了
			yield break;	// 処理できないので中断する
		}

		// 変数宣言
		GameObject _back_scene = _breadcrumb_list[_breadcrumb_list.Count - 1];	// シーン展開前のシーンを取得

		//TODO:新旧シーンを止めておく
		
		// 遷移開始
		yield return transitions?.OutData?.Act();	// 画面転換アニメーション
		
		// 変数宣言
		var _coroutine = StartCoroutine(transitions?.WaitData?.Act());	// 待機画面表示

		// シーン転換
		if (_current_scene)	// ヌルチェック
		{
			Destroy(_current_scene.gameObject);	// 現在シーンを破棄
		}
		_back_scene.gameObject.SetActive(true);	// 再表示
		
		// 経路更新
		_breadcrumb_list.RemoveAt(_breadcrumb_list.Count - 1);	// 使用したシーンを退却

		// 待機終了
		StopCoroutine(_coroutine);	// コルーチンを止める

		// 遷移再開
		yield return transitions?.InData?.Act();	// 画面転換アニメーション

		//TODO:新シーンを動かす

		// 遷移完了
		yield break;	// 処理終了
	}
}