Shader "Example/Rim" 
{
    Properties 
    {
		_Color ("Color", Color) = (1,1,1,1)
		_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0

		_Glossiness ("Smoothness", Range(0,1)) = 0.5
    }
    
    SubShader 
    {
      Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
      
      CGPROGRAM
      #pragma surface surf Standard alpha
      #pragma target 3.0
      
      struct Input 
      {
          float2 uv_MainTex;
          float2 uv_BumpMap;
          float3 viewDir;
      };
      
      sampler2D _MainTex;
      sampler2D _BumpMap;
      
      fixed4 _Color;
      
      float4 _RimColor;
      float _RimPower;
      
      float _Glossiness;
      float _Metallic;
      
      void surf (Input IN, inout SurfaceOutputStandard o) 
      {
          o.Albedo = _Color.rgb;
          o.Alpha = _Color.a;
 
          half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
          o.Emission = _Color.rgb * pow (rim, _RimPower);
          
          o.Smoothness = _Glossiness;
          o.Metallic = _Metallic;
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }