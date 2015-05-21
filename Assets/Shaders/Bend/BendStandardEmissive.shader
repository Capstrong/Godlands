﻿Shader "Custom/BendStandardEmissive" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_EmissionTex ("Emissive (RGB)", 2D) = "white" {}
		_EmissionColor ("Emissive Color (RGB)", Color) = (1, 1, 1, 1)
		
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert addshadow fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		#include "../GodlandsShaderUtils.cginc"

		sampler2D _MainTex;
		sampler2D _EmissionTex;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_EmissionTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _EmissionColor;

		void vert ( inout appdata_full v )
		{
			v.vertex += WorldSpaceCalculateWorldBendOffset( v.vertex );
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
			o.Emission = tex2D(_EmissionTex, IN.uv_EmissionTex) * _EmissionColor;
		}
		ENDCG
	}
	
	FallBack "Diffuse"
}