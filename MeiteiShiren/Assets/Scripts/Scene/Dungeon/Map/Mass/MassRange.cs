/*=====
<MassRange.cs>

-author
	mizunose

-about
	マスの範囲を実装

-note
・鉛直上向きを正面としている。
=====*/

// 名前空間宣言
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

// クラス定義

/// <summary>
/// <para>マス範囲</para>
/// </summary>
public class MassRange : CreatableData
{
	#if UNITY_EDITOR
	// クラス定義

	/// <summary>
	/// <para>マス範囲のインスペクタ表示拡張</para>
	/// </summary>
	[CustomEditor(typeof(MassRange))]
	private class MassRangeEditor : Editor
	{
		// 定数定義
		private const string _BASE_POSITION_TOOLTIP = "occurred";	// 基準位置に表示するツールチップ
		private static readonly Color _NONE_RANGED_BASE_POSITION_COLOR = Color.cyan;	// 範囲外の基準位置表示色
		private static readonly Color _RANGED_BASE_POSITION_COLOR = Color.yellow;	// 範囲内の基準位置表示色
		private static readonly Color _NONE_RANGED_COLOR = Color.green;	// 範囲外の表示色
		private static readonly Color _RANGED_COLOR = Color.red;	// 範囲内の表示色


		/// <summary>
		/// <para>インスペクター上での描画表示</para>
		/// </summary>
		public override void OnInspectorGUI()
		{
			// 変数宣言
			MassRange _instance = target as MassRange;	// インスペクタのインスタンスを取得

			// 通常描画
			DrawDefaultInspector();	// 通常のインスペクタ表示を1通り行う

			// UI表示
			if (_instance._type == RangeType.RANGED)	// 限定範囲の編集をする必要があるときのみ
			{
				// リストのプロパティ表示
				serializedObject.Update();	// 最新データを取得
				EditorGUILayout.PropertyField (serializedObject.FindProperty ("_range"));	// 範囲を格納するプロパティ
				serializedObject.ApplyModifiedProperties();	// プロパティの変更を反映

				// 変数宣言
				RectInt _draw_property = _instance.CreateDrawProperty();	// グリッド描画領域
				float _cell_size = EditorGUIUtility.currentViewWidth / (_draw_property.width);	// 正方マスのサイズ
				Rect _draw_space = GUILayoutUtility.GetRect(0, 0, GUILayout.MaxWidth(_draw_property.width * _cell_size), GUILayout.MaxHeight(_draw_property.height * _cell_size));	// グリッドの描画領域をインスペクタから取得
				GUIContent[,] _contents = new GUIContent[_draw_property.height, _draw_property.width];	// 各マスのUIデータ

				// グリッド描画
				for (int _y_idx = _draw_property.yMin; _y_idx < _draw_property.yMax; _y_idx++)	// グリッドy位置単位でのループ
				{
					for (int _x_idx = _draw_property.xMin; _x_idx < _draw_property.xMax; _x_idx++)	// グリッドx位置単位でのループ
					{
						// 変数宣言
						GUIContent content = new GUIContent();	// マスのUIデータ

						// 初期化
						content.text = $"{_x_idx},{_y_idx}";	// テキストデータを設定
						if (_x_idx == 0 && _y_idx == 0)	// 基準位置
						{
							content.tooltip = _BASE_POSITION_TOOLTIP;	// ツールチップを設定
						}

						// リスト更新
						_contents[_y_idx - _draw_property.yMin, _x_idx - _draw_property.xMin] = content;	// UIデータを登録

						// 範囲外マスの色設定
						if (_x_idx == 0 && _y_idx == 0)	// 基準位置
						{
							GUI.backgroundColor = _NONE_RANGED_BASE_POSITION_COLOR;	// 範囲外かつ基準位置の色
						}
						else	// 基準位置でない
						{
							GUI.backgroundColor = _NONE_RANGED_COLOR;	// 範囲外の色
						}

						// 範囲内マスの色設定
						foreach (Vector2Int _mass_idx in _instance.Range)	// 範囲におけるマス単位でのループ
						{
							if (_x_idx == _mass_idx.x && _y_idx == _mass_idx.y)	// 範囲に含まれている
							{
								if (_x_idx == 0 && _y_idx == 0)	// 基準位置
								{
									GUI.backgroundColor = _RANGED_BASE_POSITION_COLOR;	// 範囲内かつ基準位置の色
								}
								else
								{
									GUI.backgroundColor = _RANGED_COLOR;	// 範囲内の色
								}
								break;	// 範囲を見つけられたためループ終了
							}
						}

						// ボタン作成
						if (GUI.Button(_instance.CreateCellRect(_draw_space.center, _cell_size, new Vector2Int(_x_idx, _y_idx), _draw_property), _contents[_y_idx - _draw_property.yMin, _x_idx - _draw_property.xMin]))	// マスの操作UI
						{
							// 変数宣言
							Vector2Int _clicked = new Vector2Int(_x_idx, _y_idx);	// クリックされたマス

							// 範囲設定
							if (_instance._range.Contains(_clicked))	// 範囲に含まれていた
							{
								while (_instance._range.Contains(_clicked))	// 含まれている同値が無くなるまでループ
								{
									_instance._range.Remove(_clicked);	// 範囲から解除
								}
							}
							else	// 範囲に含まれていない
							{
								_instance._range.Add(_clicked);	// 範囲に登録
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// <para>マス範囲のプレビュー表示拡張</para>
	/// </summary>
	[CustomPreview(typeof(MassRange))]
	private class TilemapPreview : ObjectPreview
	{
		// 定数定義
		private static readonly Color _NONE_RANGED_BASE_POSITION_COLOR = Color.cyan;	// 範囲外の基準位置表示色
		private static readonly Color _RANGED_BASE_POSITION_COLOR = Color.yellow;	// 範囲内の基準位置表示色
		private static readonly Color _NONE_RANGED_COLOR = Color.green;	// 範囲外の表示色
		private static readonly Color _RANGED_COLOR = Color.red;	// 範囲内の表示色
		private const string _FRONT_LINE_RANGE_TEXT = "直線上";	// 直線範囲のラベル表示内容
		private const string _ROOM_RANGE_TEXT = "部屋全体";	// 部屋範囲のラベル表示内容
		private const string _MAP_RANGE_TEXT = "マップ全体";	// マップ範囲のラベル表示内容
		private const float _DRAW_LABEL_RATIO = 0.8f;	// ラベル表示時、描画可能領域に対する表示領域の割合(0 to 1)


		/// <summary>
		/// <para>プレビュー表示状態</para>
		/// </summary>
		/// <returns>プレビュー表示をするならtrue, そうでなければfalse</returns>
		public override bool HasPreviewGUI()
		{
			// 提供
			return true;	// プレビュー表示を作成するので、表示状態を上書きし描画機能を稼働させる
		}


		/// <summary>
		/// <para>プレビュー描画</para>
		/// </summary>
		/// <param name="preview_space">プレビュー表示領域</param>
		/// <param name="background">プレビューのデフォルト背景</param>
		public override void OnPreviewGUI(Rect preview_space, GUIStyle background)
		{
			// 変数宣言
			MassRange _instance = target as MassRange;	// インスペクタのインスタンスを取得

			// プレビュー描画
			switch (_instance._type)	// 範囲によって描画内容を変更
			{
				// 限定範囲
				case RangeType.RANGED:

					// 変数宣言
					RectInt _draw_property = _instance.CreateDrawProperty();	// グリッド描画領域
					float _cell_size = preview_space.width / (_draw_property.width) < preview_space.height / (_draw_property.height) ? 
						preview_space.width / (_draw_property.width) : preview_space.height / (_draw_property.height);	// 正方マスのサイズ
					Texture2D _texture = new(1, 1, TextureFormat.RGBA32, false);	// マスの表示テクスチャ

					// グリッド描画
					for (int _y_idx = _draw_property.yMin; _y_idx < (_draw_property.yMax); _y_idx++)	// グリッドy位置単位でのループ
					{
						for (int _x_idx = _draw_property.xMin; _x_idx < (_draw_property.xMax); _x_idx++)	// グリッドx位置単位でのループ
						{
							// 範囲外マスの色設定
							if (_x_idx == 0 && _y_idx == 0)	// 基準位置
							{
								_texture.SetPixel(0, 0, _NONE_RANGED_BASE_POSITION_COLOR);	// 範囲外かつ基準位置の色
							}
							else	// 基準位置でない
							{
								_texture.SetPixel(0, 0, _NONE_RANGED_COLOR);	// 範囲外の色
							}

							// 範囲内マスの色設定
							foreach (Vector2Int _mass_idx in _instance.Range)	// 範囲におけるマス単位でのループ
							{
								if (_x_idx == _mass_idx.x && _y_idx == _mass_idx.y)	// 範囲に含まれている
								{
									if (_x_idx == 0 && _y_idx == 0)	// 基準位置
									{
										_texture.SetPixel(0, 0, _RANGED_BASE_POSITION_COLOR);	// 範囲内かつ基準位置の色
									}
									else
									{
										_texture.SetPixel(0, 0, _RANGED_COLOR);	// 範囲内の色
									}
									break;	// 範囲を見つけられたためループ終了
								}
							}

							// 色を登録
							_texture.Apply();	// テクスチャの変更内容を確定

							// 描画
							GUI.DrawTexture(_instance.CreateCellRect(preview_space.center, _cell_size, new Vector2Int(_x_idx, _y_idx), _draw_property), _texture);	// 該当テクスチャ描画
						}
					}

					// 終了
					break;	// 分岐処理完了

				// 直線範囲
				case RangeType.FRONT_LINE:
					CreateLabel(preview_space, _FRONT_LINE_RANGE_TEXT);	// ラベル表示
					break;	// 分岐処理完了

				// 部屋全体
				case RangeType.ROOM:
					CreateLabel(preview_space, _ROOM_RANGE_TEXT);	// ラベル表示
					break;	// 分岐処理完了
					
				// マップ全体
				case RangeType.WORLD:
					CreateLabel(preview_space, _MAP_RANGE_TEXT);	// ラベル表示
					break;	// 分岐処理完了
					
				// その他
				default:
					Debug.LogError("対応の定義されていない範囲が使用されています");
					break;	// 分岐処理完了
			}
		}


		/// <summary>
		/// <para>プレビュー表示ラベル作成</para>
		/// </summary>
		/// <param name="preview_space">プレビュー領域</param>
		/// <param name="text">表示テキスト</param>
		private void CreateLabel(Rect preview_space, string text)
		{
			// 変数宣言
			GUIStyle _lb_style = new GUIStyle();	// ラベルの詳細設定

			// 初期化
			_lb_style.alignment = TextAnchor.MiddleCenter;	// 中央揃え
			_lb_style.normal.textColor = Color.gray;	// テキスト色
			_lb_style.fontSize = Mathf.RoundToInt(Mathf.Min(preview_space.width * _DRAW_LABEL_RATIO / text.Length, preview_space.height * _DRAW_LABEL_RATIO));	// フォントサイズ

			// 描画
			GUI.Label(preview_space, text, _lb_style);	// ラベル作成
		}
	}
#endif	// end UNITY_EDITOR

	// 列挙定義
	public enum RangeType	// 範囲の種類
	{
		RANGED,	// 限定範囲
		FRONT_LINE,	// 直線範囲
		ROOM,	// 部屋全体
		WORLD,	// マップ全体
	}

	// 変数変更
	[SerializeField, Tooltip("種類")] private RangeType _type;
	[HideInInspector, SerializeField, Tooltip("範囲 ※優先度順")] private List<Vector2Int> _range;

	// プロパティ定義

	/// <value><see cref="_type"/></value>
	public RangeType Type => _type;

	/// <value><see cref="_range"/></value>
	public List<Vector2Int> Range
	{
		get
		{
			// 提供
			if (Type == RangeType.RANGED)	// 一覧表記が必要
			{
				return _range;	// 範囲に含まれるマスの一覧
			}
			else	// 一覧表記を使用しない
			{
				return null;	// データは無いものとして扱える
			}
		}
	}


	/// <summary>
	/// <para>相対範囲を絶対範囲に変換</para>
	/// </summary>
	/// <param name="base_idx">基準マスの番号</param>
	/// <param name="direction">回転方向</param>
	/// <returns>変換後のマス情報</returns>
	public List<Mass> CalculateAroundTarget(in Mass base_mass, in Vector2Int direction, Mass[,] masses_map)
	{
		// 変数宣言
		List<Mass> target_masses = new();	// 範囲格納場所
		Vector2Int _base_idx = Map.PositionToMass(base_mass.transform.position);	// 基準マスの番号


		// 初期化
		switch (_type)	// 範囲の定義によって分岐
		{
			// 限定範囲
			case RangeType.RANGED:
			{
				// 走査
				foreach (Vector2Int _shift in Range)	// マス単位でのループ
				{
					// 変数宣言
					Vector2Int _target_idx = _base_idx + new Vector2Int(
						_shift.x * direction.y + _shift.y * direction.x,
						_shift.y * direction.y - _shift.x * direction.x
						);	// 対象マスの番号	※鉛直上向きを基準に範囲を回転させている

					// 保全
					if (_target_idx.x < 0 || _target_idx.x >= masses_map.GetLength(1)
						|| _target_idx.y < 0 || _target_idx.y >= masses_map.GetLength(0))	// マップ外のマス
					{
						continue;	// マスとして処理できないので次の処理へ移る
					}

					// 変数宣言
					Mass _target_mass = masses_map[_target_idx.y, _target_idx.x];	// 対象となるマス本体を取得

					// 保全
					if (!_target_mass)	// ヌルチェック
					{
						continue;	// マスが存在しないので次の処理へ移る
					}

					// リスト更新
					target_masses.Add(_target_mass);	// 効果範囲に登録
				}

				// 終了
				break;	// 分岐処理完了
			}

			// 直線範囲
			case RangeType.FRONT_LINE:
			{
				// 変数宣言
				Vector2Int _target_idx = _base_idx;	// 対象マスの番号

				// 線上走査
				while (true)	// マス単位でのループ
				{
					// 更新
					_target_idx += direction;	// 移動して走査

					// 終了条件
					if (_target_idx.x < 0 || _target_idx.x >= masses_map.GetLength(1)
						|| _target_idx.y < 0 || _target_idx.y >= masses_map.GetLength(0))	// マップ外のマス
					{
						break;	// マップ外へ進出させない
					}

					// 変数宣言
					Mass _target_mass = masses_map[_target_idx.y, _target_idx.x];	// 対象となるマス本体を取得

					// 終了条件
					if (!_target_mass)	// ヌルチェック
					{
						break;	// マスが存在しないので打ち止めにする
					}
					//TODO:壁で妨害されていて先に届かない

					// リスト更新
					target_masses.Add(_target_mass);	// 効果範囲に登録
				}

				// 終了
				break;	// 分岐処理完了
			}

			// 部屋全体
			case RangeType.ROOM:
			{
				// 変数宣言
				Room _base_room = null;	// 基準位置のある部屋

				// 初期化
				if (base_mass.transform.parent)	// ヌルチェック
				{
					_base_room = base_mass.transform.parent.GetComponent<Room>();	// 基準マスから部屋を取得
				}

				// 処理分岐
				if (_base_room)	// ヌルチェック
				{
					// リスト更新
					foreach (Mass _target_mass in _base_room.GetComponentsInChildren<Mass>())	// 部屋に含まれるマス単位でのループ
					{
						target_masses.Add(_target_mass);	// 効果範囲に登録
					}
				}
				else	// 部屋に対する行動ができない
				{
					// 変数宣言
					Vector2Int _target_idx = _base_idx + direction;	// 対象マスの番号

					// 保全
					if (_target_idx.x < 0 || _target_idx.x >= masses_map.GetLength(1)
						|| _target_idx.y < 0 || _target_idx.y >= masses_map.GetLength(0))	// マップ外のマス
					{
						break;	// マップ外へ進出させない
					}

					// 変数宣言
					Mass _target_mass = masses_map[_target_idx.y, _target_idx.x];	// 対象となるマス本体を取得
		
					// リスト更新
					if (_target_mass)	// ヌルチェック
					{
						target_masses.Add(_target_mass);	// 効果範囲に登録
					}
				}

				// 終了
				break;	// 分岐処理完了
			}

			// マップ全体
			case RangeType.WORLD:
			{
				// 攻撃処理
				foreach (Mass _mass in masses_map)	// マス単位でのループ
				{
					// 保全
					if (!_mass)	// ヌルチェック
					{
						continue;	// マスが存在しないので処理できない
					}
					
					// リスト更新
					target_masses.Add(_mass);	// 効果範囲に登録
				}

				// 終了
				break;	// 分岐処理完了
			}

			// その他
			default:
				Debug.LogError("対応の定義されていない範囲が使用されています");
				break;	// 分岐処理完了
		}

		// 提供
		return target_masses;	// 範囲該当マス
	}


#if UNITY_EDITOR
	/// <summary>
	/// <para>範囲UI描画グリッドの詳細を作成</para>
	/// </summary>
	/// <returns>グリッド描画領域</returns>
	private RectInt CreateDrawProperty()
	{
		// 変数宣言
		RectInt _result = new();	// 演算結果格納用

		// 初期化
		_result.SetMinMax(Vector2Int.zero, Vector2Int.zero);	// 基準位置を必ず含める
		foreach (var _mass_idx in Range)	// 範囲内マス単位でのループ
		{
			// 描画領域を初期化
				// x方向
				if (_result.xMin > _mass_idx.x)	// 描画領域の左領域が不足している
				{
					_result.xMin = _mass_idx.x;	// 描画領域を拡張する
				}
				else if (_result.xMax < _mass_idx.x)	// 描画領域の右領域が不足している
				{
					_result.xMax = _mass_idx.x;	// 描画領域を拡張する
				}

				// y方向
				if (_result.yMin > _mass_idx.y)	// 描画領域の下領域が不足している
				{
					_result.yMin = _mass_idx.y;	// 描画領域を拡張する
				}
				else if (_result.yMax < _mass_idx.y)	// 描画領域の上領域が不足している
				{
					_result.yMax = _mass_idx.y;	// 描画領域を拡張する
				}
		}

		//周囲に1マスの余白を持たせる
		_result.min -= Vector2Int.one;	// 左端,下端に余白を持たせる
		_result.max += Vector2Int.one;	// 右端,上端に余白を持たせる

		// 補正
		_result.max += Vector2Int.one;	// サイズ(右端,上端)の調整

		// 提供
		return _result;	// 演算結果を提供
	}


	/// <summary>
	/// <para>マス描画領域を作成</para>
	/// </summary>
	/// <param name="center">描画領域の中心</param>
	/// <param name="size">正方マスの表示サイズ</param>
	/// <param name="idx">マスの番号</param>
	/// <param name="draw_property">グリッド描画領域</param>
	/// <returns>該当マスの描画領域</returns>
	private Rect CreateCellRect(Vector2 center, float size, Vector2Int idx, RectInt draw_property)
	{
		// 変数宣言
		Rect _result = new();	// 演算結果格納用
		
		// 初期化
		_result.size = Vector2.one * size;	// マスの領域を初期化
		_result.position = center + (new Vector2(idx.x - draw_property.xMin, -idx.y + draw_property.yMax - 1) - (draw_property.size * Vector2.one * 0.5f)) * size;	// マスの描画位置	※y座標は上を正とする補正をしている

		// 提供
		return _result;	// マスの描画領域
	}
#endif	// end UNITY_EDITOR
}