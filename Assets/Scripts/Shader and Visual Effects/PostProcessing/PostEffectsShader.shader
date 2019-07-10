Shader "Custom/PostEffectsShader"
{
    Properties{
		_MainTex("", 2D) = "white" {} //this texture will have the rendered image before post-processing
		_FadeColor("Fade Color", Color) = (0, 0, 0, 1)
	}

	SubShader{
		Tags{ "RenderType" = "Opaque" }

		Pass{
			CGPROGRAM
			#pragma fragment frag
			#include "UnityCG.cginc"

			float _FadePercent;
			float _GameTime;
			fixed4 _FadeColor;


			struct v2f {
				float4 scrPos : TEXCOORD1;
			};

			sampler2D _MainTex;

			//Fragment Shader
			half4 frag(v2f i) : COLOR{
				//save the original color of each frag
				fixed4 orgColor = tex2Dproj(_MainTex, i.scrPos);

				return lerp(orgColor, _FadeColor, _FadePercent/100);
			}
		ENDCG
		}
	}
	FallBack "Diffuse"
}
