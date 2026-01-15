/*=====
<UIInputManager.cs>

-author
	mizunose

-about
	UI入力管理を実装
=====*/

// クラス定義
using UnityEngine.InputSystem;

/// <summary>
/// <para>UIの入力</para>
/// </summary>
public class UIInputManager : ActionMapsManager<UIInputManager>
{
	// クラス定義
	/// <summary>
	/// <para>ドロップダウン入力管理</para>
	/// </summary>
	public class UIDropDownInput : InputMapManager
	{
		//	変数宣言
		UIInputManager _parent;	// 親クラスの実体
		InputActionManager _vertical_move;	// 垂直移動入力受付インスタンス
		InputActionManager _decide;	// 決定入力受付インスタンス

		// プロパティ定義
	
		/// <value>管理しているInputMap</value>
		protected override InputActionMap Target => _parent.Maps.DropDown;

		/// <value><see cref="_vertical_move"/></value>
		public InputActionManager VerticalMove => _vertical_move;

		/// <value><see cref="_decide"/></value>
		public InputActionManager Decide => _decide;


		/// <summary>
		/// <para>コンストラクタ</para>
		/// </summary>
		/// <param name="parent">親クラス</param>
		public UIDropDownInput(in UIInputManager parent)
		{
			// 初期化
			_parent = parent;	// 親クラスのインスタンスを登録
			_vertical_move = new InputActionManager(_parent.Maps.DropDown.VerticalMove);	// 垂直移動入力受付を生成
			_decide = new InputActionManager(_parent.Maps.DropDown.Decide);	// 決定入力受付を生成
		}
	}

	// 変数宣言
	private UIInput _maps;	// 管理対象マップを所持するインスタンス
	private UIDropDownInput _drop_down;	// ドロップダウン入力の管理インスタンス

	// プロパティ定義

	/// <value>有効状態管理対象</value>
	protected override IInputActionCollection2 Target => _maps;

	/// <value><see cref="_maps"/></value>
	private UIInput Maps
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

	/// <value><see cref="_drop_down"/></value>
	public UIDropDownInput DropDown
	{
		get
		{
			// 保全
			if (_drop_down == null)	// ヌルチェック
			{
				_drop_down = new(this);	// 作成
			}
			
			// 提供
			return _drop_down;	// インスタンス本体
		}
	}


	/// <summary>
	/// <para>含有Mapを有効状態へ近づける</para>
	/// </summary>
	public override void TrendEnable()
	{
		// 状態遷移
		DropDown.TrendEnable();	// ドロップダウン入力の有効化
	}


	/// <summary>
	/// <para>含有Mapを無効状態へ近づける</para>
	/// </summary>
	public override void TrendDisable()
	{
		// 状態遷移
		DropDown.TrendDisable();	// ドロップダウン入力の無効化
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