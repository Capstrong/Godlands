Shader "Custom/CloudWave" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ScaleSpeed ("Scale Speed", float) = 5
		_ScaleForce ("Scale Force", float) = 10
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;

		float _ScaleSpeed;
		float _ScaleForce;

		struct Input 
		{
			float2 uv_MainTex;
		};

		void vert( inout appdata_full v ,out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			v.vertex.xyz += v.normal * sin(_Time.y * _ScaleSpeed) * _ScaleForce;
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Transparent/Diffuse"
}
