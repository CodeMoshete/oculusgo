// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Shockwave"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Alpha("Alpha", Range(0,1)) = 1
		_Color("Color", Color) = (1,1,1,1)
	}
		SubShader
		{
			Tags { "Queue" = "Transparent" "RenderType" = "Opaque" }
			LOD 100

			Blend One One
			Lighting Off
			//Cull Off
			//ZTest Always
			//ZWrite Off
			Fog { Mode Off }

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float3 normal: NORMAL;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
					float3 normal : NORMAL;

				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _Color;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.normal = normalize(v.normal);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture
					float3 camPos = _WorldSpaceCameraPos;
					float3 vertPos = mul(unity_ObjectToWorld, i.vertex);
					float3 camVec = normalize(vertPos - camPos);
					float3 viewDir = normalize(UNITY_MATRIX_IT_MV[2].xyz);
					float3 test = float3(0, 1, 0);
					float rampCoords = dot(float3(vertPos), test);
					fixed4 col = tex2D(_MainTex, (0, 0.5));
					col.rgb *= rampCoords;//_Color.a;
					// apply fog
					return col;
				}
				ENDCG
			}
		}
}
