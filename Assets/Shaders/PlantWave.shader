Shader "Custom/PlantWave" 
{
	Properties 
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		
		_WindTex ("Wind Map (RGB)", 2D) = "white" {}
		_WindDirection ("Wind Direction", Vector) = (1, 1, 1, 1)
		_WindSpeed ("Wind Speed", float) = 1
		_WindForce ("Wind Force", float) = 1
		
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		
		_Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.0
	}
	SubShader 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 200
		
		//Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha vertex:vert alphatest:_Cutoff addshadow fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _WindTex;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_WindTex;
		};

		half _Glossiness;
		half _Metallic;
		
		fixed4 _Color;
		fixed4 _WindDirection;
		
		float _WindSpeed;
		float _WindForce;

		#ifndef __noise_hlsl_
		#define __noise_hlsl_
		 
		// hash based 3d value noise
		// function taken from [url]https://www.shadertoy.com/view/XslGRr[/url]
		// Created by inigo quilez - iq/2013
		// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
		 
		// ported from GLSL to HLSL
		 
		float hash( float n )
		{
		    return frac(sin(n)*43758.5453);
		}
		 
		float pnoise( float3 x )
		{
		    // The noise function returns a value in the range -1.0f -> 1.0f
		 
		    float3 p = floor(x);
		    float3 f = frac(x);
		 
		    f       = f*f*(3.0-2.0*f);
		    float n = p.x + p.y*57.0 + 113.0*p.z;
		 
		    return lerp(lerp(lerp( hash(n+0.0), hash(n+1.0),f.x),
		                   lerp( hash(n+57.0), hash(n+58.0),f.x),f.y),
		               lerp(lerp( hash(n+113.0), hash(n+114.0),f.x),
		                   lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
		}
		#endif

		void vert ( inout appdata_full v )
		{
			float4 worldPos = normalize( mul (UNITY_MATRIX_MVP, v.vertex) );
			float worldSeed = (worldPos.x + worldPos.y + worldPos.z)/3;
		
			float4 moveVec = sin( _Time.y * _WindSpeed + pnoise(worldSeed)) * _WindDirection;
			float moveAmount = tex2Dlod( _WindTex, float4( v.texcoord.xy, 0, 0 )).r * _WindForce;
			//float moveAmount = v.color.r * _WindForce;
			v.vertex += moveVec * moveAmount;
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
		}
		ENDCG
	} 
	Fallback "Transparent/Cutout"
}
