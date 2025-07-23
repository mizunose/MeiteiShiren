/*=====
<UVTest.cs>

-author
	mizunose

-about
	uv座標を表示するグラデーションシェーダー
=====*/

// シェーダー
Shader "Custom/UVTest"	// シェーダー名
{
	SubShader	// 実装部分
	{
		// 設定
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}	// 透過表示

		// 描画
		Pass	// 1工程
		{
			HLSLPROGRAM	// HLSL文開始
			
			// インクルード部
			#include "UnityCG.cginc"

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
				float4 vertex : SV_POSITION;	// 頂点位置
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
				_result.vertex = UnityObjectToClipPos(input.vertex);	// スクリーンまで座標変換
				_result.texcoord = input.texcoord;	// テクスチャ座標を設定

				// 提供
				return _result;	// 加工データ引き渡し
			}
			
			/// <summary>
			/// <para>ピクセルシェーダー</para>
			/// </summary>
			Result fragment_shader(FragmentInput input)
			{
				// 変数宣言
				Result _result;	// 結果格納用

				// 初期化
				_result.color = float4(input.texcoord.x, input.texcoord.y, 1.0f, 1.0f);	// uv座標に基づいた色を設定

				// 提供
				return _result;	// 加工データ確定
			}

			ENDHLSL	// HLSL文終了
		}
	}
}