// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ToonShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Outline("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineSize("Outline Thickness", Float) = 0.1
		_MinDark("Minimum Darkness", Float) = 0.15
		_SunspotCutoff("Sunglow Cutoff", Float) = 0.95
		_Sunspot("Sunglow Intensity", Float) = 6


		//_Glossiness ("Smoothness", Range(0,1)) = 0.5
		//_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		//Outline Effect
		/*Pass{
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			half _OutlineSize;
			fixed4 _Outline;

			struct appdata_cel {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float4 color : COLOR;
			};

			//return position
			struct v2f {
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata_cel v) {
				v2f o;
				//move vertex position away from object origin. This should be replaced by moving them along their normal vectors, but hard edges are dumb
				v.vertex.xyz += normalize(v.normal.xyz) * _OutlineSize;
				//reverse normals
				v.normal *= -1;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			half4 frag(v2f i) : SV_Target
			{
				return _Outline;
			}
			ENDCG
		}
		
		Cull Back*/

		//surface shader (cel shading)

		CGPROGRAM
		#pragma surface surf CelShadingForward
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		float _MinDark;
		float _SunspotCutoff;
		float _Sunspot;
		half4 LightingCelShadingForward(SurfaceOutput s, half3 lightDir, half atten) { //atten is a range 0-1 where 0 is in darkness and 1 is in bright light
			half lightStr = dot(s.Normal, lightDir);
			lightStr -= (atten < 0.3); //if in shade, subtract 1 from light strength
			half NdotL = 1 + clamp(floor(lightStr), -1 + _MinDark, 0);
			half4 c;
			half sunglow = (lightStr > _SunspotCutoff)*_Sunspot + 1; //Apply the sunspot light strength multiplier if this spot is above the cutoff bringhtness 
			c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * 2 * sunglow );
			c.a = s.Alpha;
			return c;
		}

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		//half _Glossiness;
		//half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
