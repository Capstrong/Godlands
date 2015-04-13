Shader "Custom/EnvironmentAlpha" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_NormalMap ("Normal Map", 2D) = "bump" {}
		
		_ColorMask ("Color Mask", 2D) = "white" {}
		_ColorOverlayA ("Color Overlay White", Color) = (1, 1, 1, 1)
		_Color ("Color Overlay Black", Color) = (1, 1, 1, 1) // Need this to be named _Color or else shadows won't work WTF!!
		
		
		_EmissiveStrength ("Emissive Strength", Range(0, 2.3)) = 1
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		
		_Curvature ("Curvature", float) = -0.0017
		_MinCurveDistance ("Minimum Curve Distance", float) = 20
	}

	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 200
		
		Cull Off
		
		CGPROGRAM
		#pragma vertex vert
		#pragma surface surf Lambert alphatest:_Cutoff addshadow fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _ColorMask;
		sampler2D _NormalMap;

		float _EmissiveStrength;
		fixed4 _Color;
		fixed4 _ColorOverlayA;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_ColorMask;
			float2 uv_NormalMap;
			float3 viewDir;
		};

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

		void surf (Input IN, inout SurfaceOutput o) 
		{
			float4 c = tex2D (_MainTex, IN.uv_MainTex);
			float3 mask = tex2D (_ColorMask, IN.uv_ColorMask).rgb;
			o.Albedo = c * lerp(_Color, _ColorOverlayA, (mask.r + mask.g + mask.b)/3);
			o.Normal = UnpackNormal (tex2D (_NormalMap, IN.uv_NormalMap));
			
			o.Emission = _EmissiveStrength * o.Albedo;
			
			if(dot(IN.viewDir, o.Normal) < 0)
			{
				o.Normal *= -1;
				o.Emission += o.Albedo * 0.1;
			}
			
			o.Alpha = c.a;
		}
		
		ENDCG
	}

Fallback "Transparent/Cutout"
}
