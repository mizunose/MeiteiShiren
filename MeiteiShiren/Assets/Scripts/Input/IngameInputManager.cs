/*=====
<IngameInputManager.cs>

-author
	mizunose

-about
	インゲーム画面でのプレイヤー入力管理を実装
=====*/

// クラス定義
using UnityEngine.InputSystem;

/// <summary>
/// <para>インゲーム画面での入力</para>
/// </summary>
public class IngameInputManager : MonoSingleton<IngameInputManager>
{
	// クラス定義
	/// <summary>
	/// <para>プレイヤー入力管理</para>
	/// </summary>
	public class IngamePlayerInput : InputMapManager
	{
		//	変数宣言
		IngameInputManager _parent;	// 親クラスの実体

		// プロパティ定義
	
		/// <summary>
		/// <para>扱う対象のInputMap</para>
		/// </summary>
		/// <value>管理しているInputMap</value>
		protected override InputActionMap Target => _parent.Target.Player;

		/// <summary>
		/// <para>移動入力値</para>
		/// </summary>
		/// <value>移動入力受付インスタンス</value>
		public InputAction Move => _parent.Target.Player.Move;

		/// <summary>
		/// <para>攻撃入力値</para>
		/// </summary>
		/// <value>攻撃入力受付インスタンス</value>
		public InputAction Attack => _parent.Target.Player.Attack;


		/// <summary>
		/// <para>コンストラクタ</para>
		/// </summary>
		/// <param name="parent">親クラス</param>
		public IngamePlayerInput(in IngameInputManager parent)
		{
			// 初期化
			_parent = parent;	// 親クラスのインスタンスを登録
		}
	}

	// 変数宣言
	private IngameInput _target;	// 管理対象マップを所持するインスタンス
	private IngamePlayerInput _player;	// プレイヤー入力の管理インスタンス

	// プロパティ定義

	/// <summary>
	/// <para>子クラスで管理するInputMap</para>
	/// </summary>
	/// <value><see cref="_target"/></value>
	private IngameInput Target
	{
		get
		{
			// 保全
			if (_target  == null)	// ヌルチェック
			{
				_target = new();	// 作成
			}
			
			// 提供
			return _target;	// インスタンス本体
		}
	}

	/// <summary>
	/// <para>プレイヤー入力管理</para>
	/// </summary>
	/// <value><see cref="_player"/></value>
	public IngamePlayerInput Player
	{
		get
		{
			// 保全
			if (_player == null)	// ヌルチェック
			{
				_player = new(this);	// 作成
			}
			
			// 提供
			return _player;	// インスタンス本体
		}
	}


	/// <summary>
	/// <para>破棄時処理</para>
	/// </summary>
	protected override void CustomOnDestroy()
	{
		// 解放
		if (_target != null)	// ヌルチェック
		{
			_target.Disable();	// 無効化
			_target.Dispose();	// 内部状態を解放(GC対象外)
		}
	}
}