﻿Shader "Custom/BendStandardNormal" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		
		_BumpMap("Normal Map", 2D) = "bump" {}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		
		#pragma shader_feature _NORMALMAP
		
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert addshadow fullforwardshadows
	
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		#include "GodlandsShaderUtils.cginc"

		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void vert ( inout appdata_full v )
		{
			v.vertex += CalculateWorldBendOffset(v);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D( _BumpMap, IN.uv_BumpMap ));
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
