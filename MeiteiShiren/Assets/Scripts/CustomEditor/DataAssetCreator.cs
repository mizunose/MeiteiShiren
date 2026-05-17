/*=====
<DataAssetCreator.cs>

-author
	mizunose

-about
	データ作成ウィンドウを実装

-note
	[CreateAssetMenu]属性の代替機能となります。
	属性は記法を自動化できず、階層構造に対して保守性が失われていました。
	本機能は、各データクラスの表示方法を自動的に統一することで、データ作成の可読性を上げ、保全性を担保することを目的としています。
=====*/

// 名前空間宣言
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// クラス定義

/// <summary>
/// <para>データアセット作成ウィンドウ</para>
/// </summary>
public class DataAssetCreator : EditorWindow
{
	// クラス定義

	/// <summary>
	/// <para>クラスの関係性ツリー</para>
	/// </summary>
	/// <typeparam name="RootType">根とするクラス</typeparam>
	private class ClassTree<RootType>
	{
		// クラス定義

		/// <summary>
		/// <para>ツリーの葉</para>
		/// </summary>
		private class Reaf
		{
			// 変数宣言
			private Type _data_class;	// 扱う型
			public List<Reaf> children = new();	// 継承先
			public bool expanded = true;	// 表示タブが開かれているか。trueで開状態、falseで閉状態。

			// プロパティ定義

			/// <value><see cref="_data_class"/></value>
			public Type DataClass => _data_class;


			/// <summary>
			/// <para>コンストラクタ</para>
			/// </summary>
			/// <param name="data_class">対象の型</param>
			public Reaf(Type data_class)
			{
				// 初期化
				_data_class = data_class;	// 型情報登録
			}
		}

		// 定数定義
		private const string _GENERIC_ARGUMENTS_HEAD = "<";	// ジェネリック型名の接頭字
		private const string _GENERIC_ARGUMENTS_DELIMITER = ", ";	// ジェネリック型の型引数区切り
		private const string _GENERIC_ARGUMENTS_TAIL = ">";	// ジェネリック型名の接尾字
		private const int _BUTTON_INDENT_SCALE = 15;	// ボタンのインデントに掛ける倍率
		private static readonly Type _PROJECT_BROWSER_TYPE = Assembly.Load("UnityEditor.dll").GetType("UnityEditor.ProjectBrowser");	// Unity内ファイルブラウザウィンドウの型
		private const string _DATA_FILE_PATH_TAIL = ".asset";	// データファイルパスの接尾字

		// 変数宣言
		private Reaf _root = new Reaf(typeof(RootType));	// ツリーの根


		/// <summary>
		/// <para>コンストラクタ</para>
		/// </summary>
		public ClassTree()
		{
			// 初期化
			BuildTree();	// 初期状態を形成
		}


		/// <summary>
		/// <para>ツリー構築</para>
		/// </summary>
		public void BuildTree()
		{
			// 変数宣言
			var _data_classes = TypeCache.GetTypesDerivedFrom<RootType>().ToList();	// プロジェクト内のデータクラス一覧
			Dictionary<Type, Reaf> _data_dictionary = new();	// 生成データの管理領域

			// 並べ替え
			_data_classes.Sort((Type first, Type second) => first.Name.CompareTo(second.Name));	// 名前昇順に並べ替え

			// 初期化
			_root.children.Clear();	// キャッシュクリア
			foreach (var _data_class in _data_classes)	// データクラス単位でのループ
			{
				_data_dictionary.Add(_data_class, new Reaf(_data_class));	// 辞書を構築
			}
			_data_dictionary.Add(typeof(RootType), _root);	// 根をツリー構造に含める

			// 定義域内で親子付け
			foreach (var _data_class in _data_classes)	// データクラス単位でのループ
			{
				// 変数宣言
				Type _parent_type = _data_class.BaseType;	// 親クラス

				// 型情報の補正
				if (!_data_dictionary.ContainsKey(_parent_type))	// 特殊な型
				{
					if (_parent_type.IsGenericType)	// ジェネリックなためにキーがない
					{
						// 変数宣言
						Reaf parent_node = new Reaf(_parent_type);	// 型引数のない型情報

						// 更新
						_data_dictionary.Add(_parent_type, parent_node);	// 辞書に追加
						_data_dictionary[_parent_type.GetGenericTypeDefinition()].children.Add(parent_node);	// 派生を子クラスとして表現
					}
#if UNITY_EDITOR
					else	// 正体不明
					{
						Debug.LogError("想定されない型が検出されました。");
					}
#endif	// end UNITY_EDITOR
				}

				// 更新
				_data_dictionary[_parent_type].children.Add(_data_dictionary[_data_class]);	// ツリーの関連を更新
			}
		}


		/// <summary>
		/// <para>縦に葉の要素を並べて描画</para>
		/// </summary>
		public void DrawVerticalGUI()
		{
			// 変数宣言
			Dictionary<Type, (Reaf node, int draw_count, Type parent, int depth)> _throughed_reaves = new ();	// 通過済みノード一覧
			Type _element_class = typeof(RootType);	// ループで扱うクラス

			// 初期化
			_throughed_reaves.Add(typeof(RootType), (_root, 0, null, 0));	// ツリーの根を初期状態にする

			// 描画
			while (_throughed_reaves.Count > 0)	// データクラス単位でのループ
			{
				// UI作成
				if (_throughed_reaves[_element_class].draw_count == 0)	// 初回
				{
					// タブ表示
					if (_throughed_reaves[_element_class].node.children.Count > 0 || _throughed_reaves[_element_class].node.DataClass.BaseType == typeof(RootType))	// 1ジャンルと判定できる
					{
						// 変数宣言
						string _tab_name = string.Empty;	// タブの表示名
						Dictionary<Type, (Type[] children, int written_idx, Type parent)> _throughed_types = new();	// 経由した型一覧
						Type _tab_type = _throughed_reaves[_element_class].node.DataClass;	// タブで扱う型
						Type _tab_parent = null;	// タブの親
						bool _loop_flag = true;	// ループ条件

						// タブ名作成
						while (_loop_flag)	// タブ単位でのループ
						{
							// 変数宣言
							string _name_part = string.Empty;	// タブ名に足す文字列
							
							// 更新
							if (!_throughed_types.ContainsKey(_tab_type))	// 初回
							{
								_throughed_types.Add(_tab_type, (_tab_type.GetGenericArguments(), 0, _tab_parent));	// 辞書を更新
								_name_part = _tab_type.Name;	// 型名を表示に追加
					
								// 文字列加工
								if (_tab_type.IsGenericType)	// ジェネリック型のクラス
								{
									_name_part = _name_part.Substring(0, _name_part.IndexOf("`"));	// ジェネリック型は名前を取得したときに余分な情報がついてくるため除去する
									_name_part += _GENERIC_ARGUMENTS_HEAD;	// 派生を表示する前の文字
								}
							}

							// ループ値更新
							if (_throughed_types[_tab_type].children.Length > _throughed_types[_tab_type].written_idx)	// 扱っていない子クラスが居る
							{
								// 文字列加工
								if (_name_part == string.Empty)	// 2回目以降
								{
									_name_part = _GENERIC_ARGUMENTS_DELIMITER;	// 区分け文字
								}

								// 変数宣言
								var _new_type = _throughed_types[_tab_type].children[_throughed_types[_tab_type].written_idx];	// 次に扱うクラス

								// 処理対象を更新
								var _throughed_data = _throughed_types[_tab_type];	// タプルの取り出し(CS1612エラーの回避)
								_throughed_data.written_idx++;	// 書き込んだ回数を記録
								_throughed_types[_tab_type] = _throughed_data;	// 変更を反映
								_tab_parent = _tab_type;	// 現在のクラスが次の親となる
								_tab_type = _new_type;	// 扱うクラスを更新
							}
							else	// タブを閉じる
							{
								if (_tab_type.IsGenericType)	// ジェネリック型
								{
									_name_part = _GENERIC_ARGUMENTS_TAIL;	// 派生を表示した後の文字
								}
								if (_throughed_types[_tab_type].parent != null)
								{
									_tab_type = _throughed_types[_tab_type].parent; // 親階層へ戻る
								}
								else	// 根元まで戻ってきた
								{
									_loop_flag = false;	// ループを打ち切る
								}
							}
							_tab_name += _name_part;	// タブ名を更新
						}

						// UIを配置
						_throughed_reaves[_element_class].node.expanded = EditorGUILayout.Foldout(_throughed_reaves[_element_class].node.expanded, _tab_name, true);	// タブ作成
					}

					// ボタン表示
					if (!_throughed_reaves[_element_class].node.DataClass.IsAbstract)	// データ化できる継承先クラス
					{
						// 変数宣言
						var _temporal_indent = EditorGUI.indentLevel;	// 補正前のインデント値を退避

						// 補正
						if (_throughed_reaves[_element_class].node.DataClass.BaseType == typeof(RootType))	// 同名で先刻にタブを作成した
						{
							EditorGUI.indentLevel++;	// タブからボタンを離す
						}

						// 変数宣言
						GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);	// ボタンの表示スタイル

						// 初期化
						buttonStyle.alignment = TextAnchor.MiddleCenter;	// 中央揃え
						buttonStyle.margin = new RectOffset(EditorGUI.indentLevel * _BUTTON_INDENT_SCALE, 0, 0, 0);	// インデント

						// UIを配置
						if (GUILayout.Button(_throughed_reaves[_element_class].node.DataClass.Name, buttonStyle))	// ボタン作成
						{
							// ボタンを押したときの処理
							CreateAsset(_throughed_reaves[_element_class].node.DataClass);	// データをアセット化
						}

						// 整合性調和
						EditorGUI.indentLevel = _temporal_indent;	// インデントを補正前に戻す
					}
				}

				// クラス切替
				if (_throughed_reaves[_element_class].node.expanded && _throughed_reaves[_element_class].node.children.Count > _throughed_reaves[_element_class].draw_count)	// 描画したい子ノードがある
				{
					// 変数宣言
					var _child = _throughed_reaves[_element_class].node.children[_throughed_reaves[_element_class].draw_count];	// 次に描画する子
					Type _new_element_class = _child.DataClass;	// 次クラスの情報

					// 更新
					_throughed_reaves.Add(_new_element_class, (_child, 0, _element_class, _throughed_reaves[_element_class].depth + 1));	// 辞書に登録
					var _throughed_reaf = _throughed_reaves[_element_class];	// 構造体の取り出し(CS1612エラーの回避)
					_throughed_reaf.draw_count++;	// カウンタ進行
					_throughed_reaves[_element_class] = _throughed_reaf;	// 更新を反映
					_element_class = _new_element_class;

					// インデント管理
					EditorGUI.indentLevel++;	// 子クラスに移ったためインデントを増やす
				}
				else	// 描画したい子ノードが無い
				{
					// ループ値更新
					if(_element_class == typeof(RootType))	// 根に戻ってきた
					{
						break;	// ツリーの描画が完了
					}
					else	// 親がある
					{
						_element_class = _throughed_reaves[_element_class].parent;	// 親クラスに戻る
					}

					// インデント管理
					EditorGUI.indentLevel--;	// 親クラスに戻ったためインデントを減らす
				}
			}
		}


		/// <summary>
		/// <para>データアセット作成</para>
		/// </summary>
		/// <param name="data_class">作成対象</param>
		private void CreateAsset(Type data_class)
		{
			// 変数宣言
			BindingFlags _search_flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;	// 万能検索許可
			EditorWindow _project_browser_window = GetWindow(_PROJECT_BROWSER_TYPE);	// ファイルブラウザのウィンドウ
			var _putout_directory = (string)_PROJECT_BROWSER_TYPE.GetMethod("GetActiveFolderPath", _search_flag).Invoke(_project_browser_window, null);	// データを配置するフォルダのパスを取得
			string _output_path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(_putout_directory, data_class.Name) + _DATA_FILE_PATH_TAIL);	// 配置するデータのパス
			ScriptableObject _data_instance = CreateInstance(data_class);	// データのインスタンス
			AssetDatabase.CreateAsset(_data_instance, _output_path);	// アセット生成

			// エディタ操作
			Selection.activeObject = _data_instance;	// 作成したものを選択状態にする
		}
	}

	// 変数宣言
	ClassTree<CreatableData> _data_class_tree = new();	// データクラス木
	private Vector2 _scroll_pos;	// ウィンドウのスクロール位置


	/// <summary>
	/// <para>ウィンドウ展開</para>
	/// </summary>
	[MenuItem("Assets/Create/Data Asset")]
	public static void OpenWindow()
	{
		// ウィンドウ表示
		GetWindow<DataAssetCreator>();	// 自身のウィンドウがあれば最前面に表示、無ければ作成して表示
	}


	/// <summary>
	/// <para>コンパイル時に呼び出せる処理</para>
	/// </summary>
	private void OnEnable()
	{
		// 更新
		_data_class_tree.BuildTree();	// ツリー再構築
	}


	/// <summary>
	/// <para>GUI描画</para>
	/// </summary>
	private void OnGUI()
	{
		// 行並べ
		_scroll_pos = EditorGUILayout.BeginScrollView(_scroll_pos);	// スクロール配置
		_data_class_tree.DrawVerticalGUI();	// データクラス群の羅列
		EditorGUILayout.EndScrollView();	// スクロール領域終了
	}
}