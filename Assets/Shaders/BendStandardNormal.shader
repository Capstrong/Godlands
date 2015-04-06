Shader "Custom/BendStandardNormal" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		
		_Curvature ("Curvature", float) = -0.0017
		_MinCurveDistance ("Minimum Curve Distance", float) = 20
		_BumpMap("Normal Map", 2D) = "bump" {}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		
		#pragma shader_feature _NORMALMAP
		
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert addshadow fullforwardshadows
	
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		

		float _Curvature;
		float _MinCurveDistance;

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

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D( _BumpMap, IN.uv_BumpMap ));
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
