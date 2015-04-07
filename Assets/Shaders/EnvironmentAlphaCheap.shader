Shader "Custom/EnvironmentAlphaCheap" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color ("Color Overlay", Color) = (1, 1, 1, 1) // Need this to be named _Color or else shadows won't work WTF!!
	
		_EmissiveStrength ("Emissive Strength", Range(0, 2.3)) = 1
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}

	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 200
		
		Cull Off
		
		CGPROGRAM
		#pragma vertex vert
		#pragma surface surf Lambert alphatest:_Cutoff
		
		#include "GodlandsShaderUtils.cginc"

		sampler2D _MainTex;

		float _EmissiveStrength;
		fixed4 _Color;

		struct Input 
		{
			float2 uv_MainTex;
		};

		void vert ( inout appdata_full v )
		{
			v.vertex += CalculateWorldBendOffset(v);
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			float4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c * _Color;
			o.Emission = _EmissiveStrength * c * _Color;
			
			o.Alpha = c.a;
		}
		
		ENDCG
	}

Fallback "Transparent/Cutout"
}
