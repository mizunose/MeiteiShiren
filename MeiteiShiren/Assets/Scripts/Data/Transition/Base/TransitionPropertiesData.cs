/*
<TransitionPropertiesData.cs>

-author
	mizunose

-about
	トランジションのプロパティのデータを定義
*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>トランジションプロパティデータ</para>
/// </summary>
public abstract class TransitionPropertiesData : CreatableData
{
	// 変数宣言
	[SerializeField, Tooltip("対応シェーダー")] private Shader _supported_shader;

	// プロパティ定義

	/// <value><see cref="_supported_shader"/></value>
	public Shader SupportedShader => _supported_shader;
}