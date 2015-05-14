Shader "Custom/PlanetShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_CloudTex ("Cloud Texture", 2D) = "white" {}
		_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower ("Rim Power", Range(0.5,10.0)) = 3.0
		_LightDir ("Light Direction", Vector) = (1.0, 0.0, 0.0, 1.0)
		_LightIntensity ("Light Intensity", Float) = 4.0
		_AlphaIntensity ("Alpha Intesity", Float) = 0.5
	}

	SubShader
	{
		Lighting Off
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }

		CGPROGRAM
		#pragma surface surf Standard alpha nofog

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _CloudTex;
		float4 _RimColor;
		float _RimPower;
		float4 _LightDir;
		float _LightIntensity;
		float _AlphaIntensity;
		
		half _Glossiness;
		half _Metallic;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_CloudTex;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
			o.Albedo += tex2D (_CloudTex, IN.uv_CloudTex).rgb;

			float3 L = normalize (-_LightDir.xyz);

			// TODO: Add a slight amount of ambient lighting to make
			// the planet more interesting at night.

			// Calculate diffuse light
			half NdotL = dot (o.Normal, L);
			o.Albedo *= saturate (max (NdotL, 1e-6) * _LightIntensity);

			// TODO: When the planet is completely dark (light vector is opposite view vector)
			// add a slight rim glow to dark side of moon.

			// TODO: Instead of making the dark side transparent, make
			// the color match the background, that way it blends in,
			// but still blocks out the sky box.

			// Calculate rim glow
			half rim = 1.0 - saturate (dot (normalize (IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow (rim, _RimPower) * o.Albedo;

			// Make dark spots fade away
			// effectively do the rim glow effect,
			// but from the light's perspective.
			// It's similar to using the diffuse calculation,
			// but we get more control over how the alpha fades.
			
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			
			half alpha_rim = 1.0 - saturate (NdotL);
			o.Alpha = 1.0 - (_AlphaIntensity * pow (alpha_rim, _RimPower));
		}
		ENDCG
	}
Fallback "Diffuse"
}
