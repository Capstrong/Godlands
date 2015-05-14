Shader "Custom/BuddySkin2" 
{
	Properties 
	{
		_SkinTex ("Skin Tex (RGB)", 2D) = "white" {}
		_ClothesTex ("Clothes Tex (RGB)", 2D) = "white" {}
		_TintTex ("Tint Map (RGB)", 2D) = "white" {}
		
		
		_TintColor1 ("Tint Color 1", Color) = (0.2, 0.2, 0.2, 1)
		_TintColor2 ("Tint Color 2", Color) = (0,0,0,1)
		
		_AmbientColor ("Ambient Color", Color) = (0.23, 0.23, 0.23, 1)
		
		_Glossiness ("Smoothness", Range(0,1)) = 0.0
		_Metallic ("Metallic", Range(0,1)) = 0.0
		
		_RimColor ("Rim Color", Color) = ( 0.2, 0.2, 0.2, 1)
		_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		
		_HeightCutoff( "Height Cutoff", Range(-1,1) ) = 0.1
		
	}
	SubShader 
	{
		Tags {"Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200
		
		Cull Off
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert alpha alphatest:_Cutoff addshadow fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		#include "../GodlandsShaderUtils.cginc"

	
		sampler2D _SkinTex;
		sampler2D _TintTex;
		sampler2D _ClothesTex;

		struct Input 
		{
			float2 uv_SkinTex;
			float2 uv_TintTex;
			float2 uv_ClothesTex;
			float3 viewDir;
			float3 localPos;
		};

		half _Glossiness;
		half _Metallic;
		
		half _RimPower;
		half _HeightCutoff;
		
		fixed4 _AmbientColor;
		
		fixed4 _TintColor1;
		fixed4 _TintColor2;
		
		fixed4 _RimColor;

		void vert (inout appdata_full v, out Input o) 
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			v.vertex += WorldSpaceCalculateWorldBendOffset( v.vertex );
			o.localPos = v.vertex.xyz;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 blackCol = float4(1,1,1,1);
			fixed4 tint = tex2D ( _TintTex, IN.uv_TintTex );
					
			// Albedo comes from a texture tinted by color
			fixed4 c = float4(1,1,1,1);
			fixed4 skin = tex2D (_SkinTex, IN.uv_SkinTex);
			fixed4 cloth = tex2D (_ClothesTex, IN.uv_ClothesTex );
			
			c.rgb = lerp( cloth, skin, tint.r );
			c.rgb *= lerp( blackCol, _TintColor1, tint.g );
			c.rgb *= lerp( blackCol, _TintColor2, tint.b );
			
			if( IN.localPos.y < _HeightCutoff )
				discard;
			
			o.Albedo = c.rgb;			
			o.Alpha = c.a;
			
			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
          	o.Emission = c.rgb * pow (rim, _RimPower) * _RimColor + c.rgb * _AmbientColor;
			
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	}
	Fallback "Transparent/Cutout"
}
