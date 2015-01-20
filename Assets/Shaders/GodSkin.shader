Shader "Custom/GodSkin" 
{
	Properties 
	{
		_NoiseTex ("Noise", 2D) = "white" {}
		_Brightness ("Brightness", Range(0.0, 3.0)) = 1.0
      	_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
      	_RimBrightness ("Rim Brightness", Range(0.0, 2.0)) = 1.0
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		
		struct Input 
		{
			float4 color : COLOR;
			float2 uv_BumpMap;
			float2 uv_NoiseTex;
			float3 viewDir;
		};
		
      	float _RimPower;
      	float _Brightness;
      	float _RimBrightness;
		sampler2D _BumpMap;
		sampler2D _NoiseTex;
		
		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color;
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			half4 n = tex2D(_NoiseTex, IN.uv_NoiseTex) * 2;
			half4 c = IN.color * 1/normalize(n);
			
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
		    half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
		    o.Emission = c.rgb * pow (rim, _RimPower) * _RimBrightness;

			o.Albedo = c.rgb * _Brightness;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
