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
public class IngameInputManager : ActionMapsManager<IngameInputManager>
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
	
		/// <value>管理しているInputMap</value>
		protected override InputActionMap Target => _parent.Maps.Player;

		/// <value>移動入力受付インスタンス</value>
		public InputAction Move => _parent.Maps.Player.Move;

		/// <value>攻撃入力受付インスタンス</value>
		public InputAction Attack => _parent.Maps.Player.Attack;


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
	/// <summary>
	/// <para>UI入力管理</para>
	/// </summary>
	public class IngameUIInput : InputMapManager
	{
		//	変数宣言
		IngameInputManager _parent;	// 親クラスの実体

		// プロパティ定義
	
		/// <value>管理しているInputMap</value>
		protected override InputActionMap Target => _parent.Maps.UI;

		/// <value>カーソル移動入力受付インスタンス</value>
		public InputAction Move => _parent.Maps.UI.Cursor;


		/// <summary>
		/// <para>コンストラクタ</para>
		/// </summary>
		/// <param name="parent">親クラス</param>
		public IngameUIInput(in IngameInputManager parent)
		{
			// 初期化
			_parent = parent;	// 親クラスのインスタンスを登録
		}
	}

	// 変数宣言
	private IngameInput _maps;	// 管理対象マップを所持するインスタンス
	private IngamePlayerInput _player;	// プレイヤー入力の管理インスタンス
	private IngameUIInput _ui;	// UI入力の管理インスタンス

	// プロパティ定義

	/// <value>有効状態管理対象</value>
	protected override IInputActionCollection2 Target => _maps;

	/// <value><see cref="_maps"/></value>
	private IngameInput Maps
	{
		get
		{
			// 保全
			if (_maps  == null)	// ヌルチェック
			{
				_maps = new();	// 作成
			}
			
			// 提供
			return _maps;	// インスタンス本体
		}
	}

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

	/// <value><see cref="_ui"/></value>
	public IngameUIInput UI
	{
		get
		{
			// 保全
			if (_ui == null)	// ヌルチェック
			{
				_ui = new(this);	// 作成
			}
			
			// 提供
			return _ui;	// インスタンス本体
		}
	}


	/// <summary>
	/// <para>含有Mapを有効状態へ近づける</para>
	/// </summary>
	public override void TrendEnable()
	{
		// 状態遷移
		Player.TrendEnable();	// プレイヤー入力の有効化
		UI.TrendEnable();	// UI入力の有効化
	}


	/// <summary>
	/// <para>含有Mapを無効状態へ近づける</para>
	/// </summary>
	public override void TrendDisable()
	{
		// 状態遷移
		Player.TrendDisable();	// プレイヤー入力の無効化
		UI.TrendDisable();	// UI入力の無効化
	}


	/// <summary>
	/// <para>破棄時処理</para>
	/// </summary>
	protected override void CustomOnDestroy()
	{
		// 解放
		if (_maps != null)	// ヌルチェック
		{
			_maps.Disable();	// 無効化
			_maps.Dispose();	// 内部状態を解放(GC対象外)
		}
	}
}