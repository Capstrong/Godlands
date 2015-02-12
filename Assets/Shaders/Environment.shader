Shader "Custom/Environment" 
{
	Properties 
	{
		_MainTex ("Diffuse Texture", 2D) = "white" {}
		_NormalMap ("Normal Map", 2D) = "bump" {}

		_ColorMask ("Color Mask", 2D) = "white" {}
		_ColorOverlayA ("Color Overlay White", Color) = (1, 1, 1, 1)
		_ColorOverlayB ("Color Overlay Black", Color) = (1, 1, 1, 1)
		
		
		_EmissiveStrength ("Emissive Strength", Range(0, 2.3)) = 1
	}
	SubShader 
	{
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf Lambert alphatest:_Cutoff

		sampler2D _MainTex;
		sampler2D _NormalMap;
		sampler2D _ColorMask;

		float _EmissiveStrength;
		fixed4 _ColorOverlayA;
		fixed4 _ColorOverlayB;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_NormalMap;
			float2 uv_ColorMask;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			float4 c = tex2D (_MainTex, IN.uv_MainTex);
			float3 mask = tex2D (_ColorMask, IN.uv_ColorMask).rgb;
			o.Albedo = c * lerp(_ColorOverlayB, _ColorOverlayA, (mask.r + mask.g + mask.b)/3);
			o.Normal = UnpackNormal (tex2D (_NormalMap, IN.uv_NormalMap));			
			o.Emission = _EmissiveStrength * o.Albedo;
		}
		
		ENDCG
	} 
Fallback "Diffuse"
}