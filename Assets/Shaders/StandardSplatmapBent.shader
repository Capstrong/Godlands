Shader "Custom/StandardSplatmapBent" 
{
	Properties 
	{
		//_Color ("Color", Color) = (1,1,1,1)
		//_MainTex ("Albedo (RGB)", 2D) = "white" {}
		
		_Splat0 ("Splat Texture 1 (RGB)", 2D) = "blackCol" {}
		_Splat1 ("Splat Texture 2 (RGB)", 2D) = "blackCol" {}
		_Splat2 ("Splat Texture 3 (RGB)", 2D) = "blackCol" {}
		_Splat3 ("Splat Texture 4 (RGB)", 2D) = "blackCol" {}
		
		_SplatMap ("Splat Map (RGBA)", 2D) = "white" {}
		
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert addshadow fullforwardshadows finalcolor:myfinal exclude_path:prepass
		#include "GodlandsShaderUtils.cginc"

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input 
		{
			float2 uv_Splat0 : TEXCOORD0;
			float2 uv_Splat1 : TEXCOORD1;
			float2 uv_Splat2 : TEXCOORD2;
			float2 uv_Splat3 : TEXCOORD3;
			float2 tc_SplatMap : TEXCOORD4; // not using uv allows for more texture interpolators
		};

		sampler2D _Splat0;
		sampler2D _Splat1;
		sampler2D _Splat2;
		sampler2D _Splat3;
		
		sampler2D _SplatMap;
		float4 _SplatMap_ST;		

		half _Glossiness;
		half _Metallic;
		
		fixed4 blackCol = float4( 0, 0, 0, 1);

		void vert ( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
		
			o.tc_SplatMap = TRANSFORM_TEX(v.texcoord, _SplatMap);
			v.vertex += CalculateWorldBendOffset( v );
		}

		void surf ( Input IN, inout SurfaceOutputStandard o ) 
		{
			// Albedo comes from a texture tinted by color
			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			//o.Albedo = c.rgb;
			
			fixed4 splatCol = tex2D( _SplatMap, IN.tc_SplatMap );
//			
//			fixed4 colA = lerp( blackCol, tex2D( _Splat0, IN.uv_Splat0 ), splatCol.r );
//			fixed4 colB = lerp( blackCol, tex2D( _Splat1, IN.uv_Splat1 ), splatCol.g );
//			fixed4 colC = lerp( blackCol, tex2D( _Splat2, IN.uv_Splat2 ), splatCol.b );
//			fixed4 colD = lerp( blackCol, tex2D( _Splat3, IN.uv_Splat3 ), splatCol.a );
//			
//			float4 finalCol = lerp( colD, colC, splatCol.b );
//			finalCol = lerp( finalCol, colB, splatCol.g );
//			finalCol = lerp( finalCol, colA, splatCol.r );
//			
//			o.Albedo = finalCol.rgb;
//			
			//splat_control = tex2D(_Control, IN.tc_Control);
			half weight = dot(splatCol, half4(1,1,1,1));

			

			#ifndef UNITY_PASS_DEFERRED
				// Normalize weights before lighting and restore weights in applyWeights function so that the overal
				// lighting result can be correctly weighted.
				// In G-Buffer pass we don't need to do it if Additive blending is enabled.
				// TODO: Normal blending in G-buffer pass...
				splatCol /= (weight + 1e-3f); // avoid NaNs in splat_control
			#endif
			
			clip(weight - 0.0039 /*1/255*/);

			float4 mixedDiffuse = 0.0f;
			mixedDiffuse += splatCol.r * tex2D(_Splat0, IN.uv_Splat0);
			mixedDiffuse += splatCol.g * tex2D(_Splat1, IN.uv_Splat1);
			mixedDiffuse += splatCol.b * tex2D(_Splat2, IN.uv_Splat2);
			mixedDiffuse += splatCol.a * tex2D(_Splat3, IN.uv_Splat3);
			
			o.Albedo = mixedDiffuse;
			
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			
			o.Alpha = float4(1, 1, 1, 1);
		}
		
		void myfinal(Input IN, SurfaceOutputStandard o, inout fixed4 color)
		{
			color.rgb *= color.a;
			color.a = 1.0f;
		}
		
		ENDCG
	} 
	FallBack "Diffuse"
}
