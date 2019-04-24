Shader "Custom/Planets/Shockwave" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Diffuse (RGB) Alpha (A)", 2D) = "white" {}
		_RampTex ("Atmosphere Ramp (B/W) (L->R)", 2D) = "white" {}
		_AlphaMult ("Alpha Multiplier", Range(0, 2)) = 1.0
		_AlphaContrast ("Alpha Contrast", Range(1, 10)) = 1.0
		_AlphaPadding ("Alpha Padding", Range(0,3)) = 1.1
		_GlareIntensity ("Glare Intensity", Range(0,2)) = 0.7
		_GlareFocus ("Glare Focus", Range(1,50)) = 4
		_RampThreshold("Ramp Threshold", Range(0,0.1)) = 0.05
	}

	SubShader{
		Tags { "RenderType" = "Opaque" "Queue"="Transparent"}
		
		Blend SrcAlpha OneMinusSrcAlpha 
		
		CGPROGRAM
			#pragma surface surf Lambert nolightmap
			#pragma target 3.0
			#pragma glsl

			struct Input
			{
				float2 uv_MainTex;
				float3 worldNormal;
				fixed3 viewDir;
				float3 Normal;
			};
			
			//struct EditorSurfaceOutput {
			//	half3 Albedo;
			//	half3 Normal;
			//	half3 Emission;
			//	half3 Gloss;
			//	half Specular;
			//	half Alpha;
			//};
			
			sampler2D _MainTex, _RampTex;
			float _AlphaMult;
			float _AlphaContrast;
			float _AlphaPadding;
			float _GlareIntensity;
			float _GlareFocus;
			float _RampThreshold;
			fixed4 _Color;
			float NdotL;
			
			inline fixed4 LightingTF3(SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
			{
				fixed4 c;
				float NdotL = min(_GlareIntensity, dot(s.Normal, lightDir));
				if(NdotL > 0)
				{
					NdotL = pow(NdotL, _GlareFocus);
				}
				c.rgb = NdotL;
				return c;
			}
			
			void surf (Input IN, inout SurfaceOutput o)
			{
				o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color.rgb;
				
				float dotResult = dot(normalize(IN.viewDir), o.Normal) / 2;
				//fixed3 ramp = tex2D(_RampTex, float2(1 - dotResult)).rgb;
				fixed3 ramp = tex2D(_RampTex, IN.uv_MainTex).rgb;
//				if(dotResult > _RampThreshold)
//				{
//					o.Alpha = dotResult;//pow((_AlphaPadding - dotResult), _AlphaContrast) * _AlphaMult * ramp;
//				}
//				else
//				{
//					o.Alpha = 0;
//				}
				o.Alpha = 0;
			}
		ENDCG
	}
	Fallback "Transparent/Cutout/Bumped Specular"
}
