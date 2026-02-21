/*
<ColorOut.shader>

-author
	mizunose

-about
	固定色で画面全体を塗りつぶすトランジション用シェーダー
*/

// シェーダー
Shader "Transition/ColorOut"	// シェーダー名
{
	// プロパティ定義
	Properties
	{
		[Header(Status)]
		_MainTex ("Texture", 2D) = "white" {}
		curtain_color("CurtainnColor", Color) = (0, 0, 0, 1)
		draw_alpha("DrawAlpha", Range(0.0, 1.0)) = 0.0
	}

	// 実装部分
	SubShader
	{
		// 設定
 		Tags {"RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline"}	// 非透過表示
		Cull Off	// カリングしない
		ZWrite Off	// Zバッファに書き込まない
		ZTest Always	// Z比較しない
		Blend SrcAlpha OneMinusSrcAlpha	// 透過処理

		// レンダリング
		Pass	// 描画工程
		{
			HLSLPROGRAM	// HLSL文開始
			
			// インクルード部
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"	// ref: https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"	// ref: https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl

			// 関数の対応付け
			#pragma vertex vertex_shader	// 頂点シェーダー部の関数
			#pragma fragment fragment_shader	// ピクセルシェーダー部の関数

			// 構造体定義
			/// <summary>
			/// <para>頂点シェーダーの入力引数型</para>
			/// </summary>
			struct VertexInput
			{
				// 変数宣言
				float4 vertex : POSITION;	// 頂点位置
				float2 texcoord : TEXCOORD0;	// テクスチャ座標
			};
			/// <summary>
			/// <para>ピクセルシェーダーの入力引数型</para>
			/// </summary>
			struct FragmentInput
			{
				// 変数宣言
				float4 position : SV_POSITION;	// 頂点座標
				float2 texcoord : TEXCOORD0;	// テクスチャ座標
			};
			/// <summary>
			/// <para>シェーダー結果(ピクセルシェーダーの出力)用の型</para>
			/// </summary>
			struct Result
			{
				// 変数宣言
				float4 color : SV_TARGET;	// 表示色
			};

			// 変数宣言
			sampler2D _MainTex;
			float4	curtain_color;	// 塗りつぶし色
			float	draw_alpha;	// 描画不透明度


			/// <summary>
			/// <para>頂点シェーダー</para>
			/// </summary>
			/// <param name="input">頂点情報</param>
			/// <returns>次のシェーダー(ピクセルシェーダー)に渡すデータ</returns>
			FragmentInput vertex_shader(VertexInput input)	// 構造体を使用した入出力
			{
				// 変数宣言
				FragmentInput _result;	// 結果格納用

				// 初期化
				_result.position = TransformObjectToHClip(input.vertex.xyz);	// スクリーンまで座標変換
				_result.texcoord = input.texcoord;	// テクスチャ座標を設定

				// 提供
				return _result;	// 加工データ引き渡し
			}


			/// <summary>
			/// <para>ピクセルシェーダー</para>
			/// </summary>
			/// <param name="input">前のシェーダー(頂点シェーダー)から渡される情報</param>
			/// <returns>出力データ</returns>
			Result fragment_shader(FragmentInput input)
			{
				// 変数宣言
				Result _result;	// 結果格納用

				// 初期化
				_result.color.rgb = curtain_color;	// 固有色
				_result.color.a = draw_alpha;	// 不透明度

				// 提供
				return _result;	// 加工データ確定
			}

			ENDHLSL	// HLSL文終了
		}
	}
}