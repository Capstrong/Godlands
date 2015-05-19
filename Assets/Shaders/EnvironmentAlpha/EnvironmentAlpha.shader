Shader "Custom/EnvironmentAlpha" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		
		_ColorMask ("Color Mask", 2D) = "white" {}
		_ColorOverlayA ("Color Overlay White", Color) = (1, 1, 1, 1)
		_Color ("Color Overlay Black", Color) = (1, 1, 1, 1) // Need this to be named _Color or else shadows won't work WTF!!
		
		_EmissiveStrength ("Emissive Strength", Range(0, 2.3)) = 1
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}

	SubShader 
	{
		Tags {"Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 200
		
		Cull Off
		
		CGPROGRAM
		#pragma vertex vert
		#pragma surface surf Lambert alphatest:_Cutoff addshadow fullforwardshadows
		#pragma target 3.0
		
		#include "../GodlandsShaderUtils.cginc"

		sampler2D _MainTex;
		sampler2D _ColorMask;
		
		fixed _EmissiveStrength;
		fixed4 _Color;
		fixed4 _ColorOverlayA;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_ColorMask;
			float2 uv_NormalMap;
			float3 viewDir;
			float3 normal;
		}; 

		void vert ( inout appdata_full v, out Input o )
		{
			v.vertex += WorldSpaceCalculateWorldBendOffset( v.vertex );
			o.normal = v.normal;
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			fixed3 mask = tex2D (_ColorMask, IN.uv_ColorMask).rgb;
			o.Albedo = c * lerp(_Color, _ColorOverlayA, mask.r);
			
			o.Normal = IN.normal;
			o.Emission = _EmissiveStrength * o.Albedo;
			
			fixed vDotN = dot(IN.viewDir, o.Normal);
			o.Normal *= vDotN;
			o.Alpha = c.a;
		}
		
		ENDCG
	}

	Fallback "Transparent/Cutout"
}
