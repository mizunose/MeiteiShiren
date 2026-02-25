/*=====
<Level.cs>

-author
	mizunose

-about
	レベルを実装
=====*/

// 名前空間宣言
using System;
using UnityEngine;

// クラス定義
/// <summary>
/// <para>レベル</para>
/// </summary>
public class Level : MonoBehaviour
{
	// 定数定義
	const uint _INITIAL_LEVEL = 1;	// 初期レベル

	// イベント定義
	public event Action OnLevelUp;	// レベルアップ時のイベント

	// 変数宣言
	[SerializeField, Tooltip("経験値テーブル")] private ExperienceTable _experience_table;
	private uint _experience;	// 経験値

	// プロパティ定義

	/// <value>次レベルまでに積み立てる経験値</value>
	public uint Experience
	{
		get
		{
			// 提供
			return _experience;	// 所持経験値を返す
		}
		set
		{
			// 更新
			_experience = value;	// 経験値を更新

			// レベルアップ
			while (true)	// レベル単位でのループ
			{
				if(_experience_table.Table.Length > Value - _INITIAL_LEVEL)	// 次のレベルがある
				{
					if (_experience > _experience_table.Table[Value - _INITIAL_LEVEL])	// 必要な経験値を満たした
					{
						// 補正
						_experience -= _experience_table.Table[Value - _INITIAL_LEVEL];	// レベルに必要な分の経験値を消費
					
						// レベル上昇
						Value++;	// レベル値を加算

						// イベント発行
						if (OnLevelUp != null)	// ヌルチェック
						{
							OnLevelUp.Invoke();	// レベルアップ時イベント発行
						}
					}
					else	// 経験値不足
					{
						// 終了
						break;	// ループ処理終了
					}
				}
				else	// レベル上限
				{
					// 補正
					_experience = 0;	// 経験値を積み重ねる必要がない

					// 終了
					break;	// ループ処理終了
				}
			}
		}
	}

	/// <value>レベル値</value>
	public uint Value { get; private set; } = _INITIAL_LEVEL;


	/// <summary>
	/// <para>レベルアップ処理</para>
	/// </summary>
	public void LevelUp()
	{
		// レベル上げ
		if(_experience_table.Table.Length > Value - _INITIAL_LEVEL)	// 次のレベルがある
		{
			Experience += _experience_table.Table[Value - _INITIAL_LEVEL];	// 今レベルでの必要経験値を獲得(確定でレベルが上がる)
		}
	}
}