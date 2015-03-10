Shader "Custom/IllumDiffuseHueShift" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_Illum ("Illumin (A)", 2D) = "white" {}
		_ShiftAmount("Shift Amount", Range(0, 1)) = 0
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
			
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _Illum;
		fixed4 _Color;
		float _ShiftAmount;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_Illum;
		};

		// GOT THESE ONLINE
		float3 RGBToHSV(float3 c)
		{
		    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
		    float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
		    float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

		    float d = q.x - min(q.w, q.y);
		    float e = 1.0e-10;
		    return float4(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x, 1);
		}

		float3 HSVToRGB(float3 c)
		{
		    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
		    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
		    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			
			fixed4 c = tex * _Color;
			c.xyz = RGBToHSV( c.xyz );
			c[0] += _ShiftAmount;
			c.xyz = HSVToRGB( c.xyz );
			
			o.Albedo = c.rgb;
			o.Emission = c.rgb * tex2D(_Illum, IN.uv_Illum).a;
			o.Alpha = c.a;
		}
		ENDCG
	} 
FallBack "Self-Illumin/VertexLit"
}
