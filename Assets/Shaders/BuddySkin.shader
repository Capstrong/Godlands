Shader "Custom/BuddySkin" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_SkinColor ("Skin Color", Color) = (1,1,1,1)
		
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ColorMask ("Color Mask", 2D) = "white" {}

      	_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
      	_RimBrightness ("Rim Brightness", Range(0.0, 2.0)) = 0.1
      	
      	_Curvature ("Curvature", float) = -0.0017
      	_MinCurveDistance ("Minimum Curve Distance", float) = 20
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert addshadow fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _ColorMask;
		sampler2D _BumpMap;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_ColorMask;
			float3 viewDir;
		};
		
		float _RimPower;
		float _Brightness;
      	float _RimBrightness;

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _SkinColor;
		
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
			fixed4 skinColor = _SkinColor;
			fixed4 bodyColor = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 mask = tex2D (_ColorMask, IN.uv_ColorMask);
			
			if( mask.r > 0.5 )
			{
				o.Albedo = skinColor.rgb;
			}
			else
			{
				o.Albedo = bodyColor.rgb;
			}
			
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
		    half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
		    o.Emission = o.Albedo * pow (rim, _RimPower) * _RimBrightness;
			
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
