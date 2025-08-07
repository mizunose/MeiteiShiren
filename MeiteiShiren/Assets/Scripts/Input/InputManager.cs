/*=====
<InputManager.cs>

-author
	mizunose

-about
	入力管理を実装
=====*/

// クラス定義
/// <summary>
/// <para>入力系のインスタンス管理</para>
/// </summary>
public class InputManager : MonoSingleton<InputManager>
{
	// 変数宣言
	private IngameInput _ingame;	// インゲーム画面での入力受付

	// プロパティ定義

	/// <summary>
	/// <para>インゲームにおける入力系</para>
	/// </summary>
	/// <value><see cref="_ingame"></value>
	public IngameInput Ingame
	{ 
		get
		{
			// 保全
			if (_ingame == null)	// ヌルチェック
			{
				_ingame = new();	// 作成
			}

			// 提供
			return _ingame;	// インスタンス本体
		}
	}


	/// <summary>
	/// <para>破棄時処理</para>
	/// </summary>
	protected override void CustomOnDestroy()
	{
		// 解放
		if (_ingame != null)	// ヌルチェック
		{
			_ingame.Dispose();	// 内部状態を解放(GC対象外)
		}
	}
}