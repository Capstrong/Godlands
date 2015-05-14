Shader "Custom/CloudWave"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ScaleSpeed ("Scale Speed", float) = 1
		_MinScale ("Min Scale", float) = 1
		_MaxScale ("Max Scale", float) = 5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;

		float _ScaleSpeed;
		float _MinScale;
		float _MaxScale;

		struct Input
		{
			float2 uv_MainTex;
		};

		void vert( inout appdata_full v ,out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			float amplitude = _MaxScale - _MinScale;
			float wave = ( sin(_Time.y * _ScaleSpeed) * 0.5f + 0.5f ); // normalize sin from 0 to 1
			wave = wave * amplitude + _MinScale;                       // scale and shift by amplitude and minimum value
			v.vertex.xyz += v.normal * wave;
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
