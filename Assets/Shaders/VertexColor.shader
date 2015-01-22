Shader "Custom/VertexColor" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex: vert

		sampler2D _MainTex;

		struct Input 
		{
			float4 color : COLOR;
			float2 uv_MainTex;
		};

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color;
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			half4 c = IN.color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
