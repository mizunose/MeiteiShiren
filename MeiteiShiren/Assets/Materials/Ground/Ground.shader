/*=====
<Ground.shader>

-author
・mizunose

-about
・uv座標を表示するグラデーションシェーダー

-remarks
・地面に使用する想定なのでキャストシャドウは付けていません
=====*/

// シェーダー
Shader "Custom/Ground"	// シェーダー名
{
	Properties	// プロパティ定義
	{
		[Header(Shadow)]
		_shadow_texture("Texture", 2D) = "white" {}
		_shadow_brightness("Brightness", Range(0.0, 1.0)) = 0.1
	}
	SubShader	// 実装部分
	{
		// 設定
 		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }	// 非透過表示

		// レンダリング
		Pass	// 描画工程
		{
			HLSLPROGRAM	// HLSL文開始
			
			// インクルード部
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"	// シャドウバッファ

			// 関数の対応付け
			#pragma vertex vertex_shader	// 頂点シェーダー部の関数
			#pragma fragment fragment_shader Standard fullforwardshadows alpha:fade	// ピクセルシェーダー部の関数

			// コンパイル分岐
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN	// レシーブシャドウ用

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
				float4 vertex : SV_POSITION;	// 頂点位置
				float2 texcoord : TEXCOORD0;	// テクスチャ座標
				float4 shadow_coordinate : TEXCOORD3;	// シャドウマップ上での位置
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
			sampler2D _shadow_texture;	// 落ち影に映すテクスチャ
			float4 _shadow_texture_ST;	// 落ち影に映すテクスチャのオフセット・タイリング情報
			float _shadow_brightness;	// 影の明るさ
			

			/// <summary>
			/// <para>頂点シェーダー</para>
			/// </summary>
			/// <param name="input">頂点情報</param>
			/// <returns>次のシェーダー(ピクセルシェーダー)に渡すデータ</returns>
			FragmentInput vertex_shader(VertexInput input) // 構造体を使用した入出力
			{
				// 変数宣言
				FragmentInput _result;	// 結果格納用

				// 初期化
				_result.vertex = TransformObjectToHClip(input.vertex.xyz);	// スクリーンまで座標変換
				_result.texcoord = input.texcoord;	// テクスチャ座標を設定
				// シャドウマップ上での座標取得
					VertexPositionInputs _positions = GetVertexPositionInputs(input.vertex.xyz);	// 各空間座標系における頂点位置取得
					_result.shadow_coordinate = GetShadowCoord(_positions);	// 頂点位置をシャドウマップ上に変換し設定

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
				_result.color = float4(input.texcoord.x, input.texcoord.y, 1.0f, 1.0f);	// uv座標に基づいた色を設定
				
				// 影の演算
				float _shadow = MainLightRealtimeShadow(input.shadow_coordinate);	// 落ち影のマスク情報
				float4 _shadow_texcolor = tex2D(_shadow_texture, input.shadow_coordinate.xy * _shadow_texture_ST.xy + _shadow_texture_ST.zw);	// 影のテクスチャ
				_shadow_texcolor.a *= (1.0f - _shadow_brightness);	// 暗いほどテクスチャが映りこむようにテクスチャの透明度を補間

				// ライティング
				_result.color.rgb = _shadow * _result.color.rgb	// 光ならそのまま
					+ (1.0f - _shadow) * (_shadow_texcolor.rgb * _shadow_texcolor.a + _result.color.rgb * (1.0f - _shadow_texcolor.a));	// 影ならそのテクスチャをブレンド
				_result.color.rgb *= _shadow + (1.0f - _shadow) * _shadow_brightness;	// 影の明度を調整

				// 提供
				return _result;	// 加工データ確定
			}

			ENDHLSL	// HLSL文終了
		}
	}
}