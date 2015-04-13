Shader "Custom/BendCubemap" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_Cube ("Reflection Cubemap", Cube) = "_Skybox" {}
		
		_Curvature ("Curvature", float) = -0.0017
		_MinCurveDistance ("Minimum Curve Distance", float) = 20
	}
	
	SubShader 
	{
		LOD 300
		Tags { "RenderType"="Opaque" }

		CGPROGRAM
		#pragma vertex vert
		#pragma surface surf BlinnPhong vertex:vert addshadow fullforwardshadows

		sampler2D _MainTex;
		samplerCUBE _Cube;

		fixed4 _Color;
		fixed4 _ReflectColor;
		half _Shininess;
		
		float _Curvature;
		float _MinCurveDistance;

		struct Input 
		{
			float2 uv_MainTex;
			float3 worldRefl;
		};

		void vert ( inout appdata_full v )
		{
			// Transform the vertex coordinates from model space into world space
		    float4 vv = mul( _Object2World, v.vertex );

		    // Now adjust the coordinates to be relative to the camera position
		    vv.xyz -= _WorldSpaceCameraPos.xyz;

			vv.z -= _MinCurveDistance * sign(vv.z);

		    // Reduce the y coordinate (i.e. lower the "height") of each vertex based
		    // on the square of the distance from the camera in the z axis, multiplied
		    // by the chosen curvature factor
		    
			vv = float4( 0.0f, (vv.z * vv.z) * - _Curvature * (sin(_Time.y/5) * 0.5 + 0.5), 0.0f, 0.0f );

		    // Now apply the offset back to the vertices in model space
		    v.vertex += mul(_World2Object, vv);
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c = tex * _Color;
			o.Albedo = c.rgb;
			o.Gloss = tex.a;
			o.Specular = _Shininess;
			
			fixed4 reflcol = texCUBE (_Cube, IN.worldRefl);
			reflcol *= tex.a;
			o.Emission = reflcol.rgb * _ReflectColor.rgb;
			o.Alpha = reflcol.a * _ReflectColor.a;
		}
		ENDCG
	}

	FallBack "Legacy Shaders/Reflective/VertexLit"
}
