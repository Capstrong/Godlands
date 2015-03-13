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
	}

	SubShader
	{
		Lighting Off
		Tags { "RenderType" = "Opaque" }
		Fog { Mode Off }

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _CloudTex;
		float4 _RimColor;
		float _RimPower;
		float4 _LightDir;
		float _LightIntensity;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_CloudTex;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
			o.Albedo += tex2D (_CloudTex, IN.uv_CloudTex).rgb;

			// Calculate diffuse light
			half NdotL = dot (o.Normal, normalize (-_LightDir.xyz));
			o.Albedo *= max (NdotL, 1e-6) * _LightIntensity;

			// Calculate rim glow
			half rim = 1.0 - saturate (dot (normalize (IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow (rim, _RimPower) * o.Albedo;
		}
		ENDCG
	}
Fallback "Diffuse"
}
