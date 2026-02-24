/*=====
<MonoSingleton.cs>

-author
	mizunose

-about
	MonoBehaviour用シングルトンの実装

-note
・ジェネリック型です。継承時に型指定してください。
=====*/

// 名前空間定義
using UnityEngine;

// クラス定義
/// <summary>
/// <para>MonoBehaviorのシングルトン</para>
/// </summary>
public abstract class MonoSingleton<MonoType> : VirtualizeMono where MonoType : MonoSingleton<MonoType>	// where文で継承ツリーを明示：MonoType←MonoSingleton<MonoType>←VirtualizeMono←MonoBehaviour
{
	// 変数宣言
	static private MonoType _instance;	// インスタンス格納用

	// プロパティ定義

	/// <value><see cref="_instance"/></value>
	public static MonoType Instance	// 継承先オブジェクトのインスタンス
	{
		get
		{
			// 保全
			if (_instance == null)	// ヌルチェック
			{
				GameObject _GameObject = new GameObject();	// インスタンス作成
				_instance = _GameObject.AddComponent<MonoType>();	// 自身のコンポーネント登録
#if UNITY_EDITOR
				_instance.gameObject.name = _instance.InstanceName;	// 命名
#endif	// end UNITY_EDITOR
			}

			// 提供
			return _instance;	// インスタンス提供
		}
	}

	/// <value><see cref="インスタンスのヌル検証"/></value>
	public static bool NullCheck => _instance != null;

#if UNITY_EDITOR
	/// <value>生成インスタンスに付ける名前</value>
	protected abstract string InstanceName { get; }
#endif	// !UNITY_EDITOR


	/// <summary>
	/// <para>初期化処理</para>
	/// </summary>
	protected override sealed void Awake()
	{
		// 自身がいくつ目か
		if (_instance != null && _instance.gameObject != null)	// すでに自身と同一のものがある
		{
			// 生成キャンセル
			Destroy(this.gameObject);	// 自身の生成をなかったことにする
			return;	// 虚無は処理されない
		}

		// インスタンス登録
		_instance = (MonoType)this;	// 自身をインスタンスとして登録

		// 追加の処理
		CustomAwake();	// 子クラスがこのタイミングで行いたい処理
	}


	/// <summary>
	/// <para>破棄時処理</para>
	/// </summary>
	protected override sealed void OnDestroy()
	{
		// 生成キャンセル判定
		if (this != _instance)	// 生成キャンセルのために行われた破棄である
		{
			// 終了
			return;	// これ以降の処理は虚無に行われるものではない
		}

		// インスタンス破棄
		if (_instance != null)	// インスタンスとして登録されている
		{
			_instance = null;	// インスタンスをヌルに初期化
		}

		// 追加の処理
		CustomOnDestroy();	// 子クラスがこのタイミングで行いたい処理
	}
}