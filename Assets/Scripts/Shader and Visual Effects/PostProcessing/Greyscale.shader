Shader "Custom/Greyscale" {

	Properties{
		_MainTex("", 2D) = "white" {} //this texture will have the rendered image before post-processing
		_RingWidth("ring width", Float) = 0.01
		_RingColor("ring color", Color) = (0,0,0,1)
	}

	SubShader{
		Tags{ "RenderType" = "Opaque" }

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _CameraDepthTexture;
			float _StartingTime;
			uniform float _RingPassTimeLength;
			uniform float _RingWidth;
			uniform float _RingMaxDistance;
			uniform float4 _RingColor;
			float _RunRingPass = 0;
			float3 _DoggoPosition;
			float4 _CameraPosition;
			float4x4 _ViewFrustum;
			float _GameTime;


			struct v2f {
				float4 pos : SV_POSITION;
				float4 scrPos : TEXCOORD1;
				float4 interpolatedRay : TEXCOORD2;
			};

			//Vertex Shader
			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.scrPos = ComputeScreenPos(o.pos);

				//set each vertex to have a ray casting thru it. Since the ray is marked as texcoords, it will be interpolated to the fragments
				float4 top = lerp(float4(_ViewFrustum[1].xyz, 0), float4(_ViewFrustum[2].xyz, 0), (o.pos.x + 1.0)/2.0);
				float4 bottom = lerp(float4(_ViewFrustum[0].xyz, 0), float4(_ViewFrustum[3].xyz, 0), (o.pos.x + 1.0) / 2.0);
				o.interpolatedRay = lerp(bottom, top, (o.pos.y + 1.0) / 2.0);
				
				return o;
			}

			sampler2D _MainTex;

			//Fragment Shader
			half4 frag(v2f i) : COLOR{
				//find a depth value based on depth textures, then use that and the interpolated ray to find the location in world space of each fragment
				float depthValue = Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)).r);
				float3 worldSpaceLocation = _CameraPosition + (depthValue * i.interpolatedRay.xyz);
				depthValue = distance(worldSpaceLocation, _DoggoPosition.xyz)/_RingMaxDistance;
				//save the original color of each frag
				fixed4 orgColor = tex2Dproj(_MainTex, i.scrPos);
				float4 newColor = orgColor;

				//the percentage we are through the animation
				float t = clamp( (_GameTime - _StartingTime) / _RingPassTimeLength , 0, 1);
				t = (_RunRingPass == 2 ? 1 - t : t);
				t = pow(t, 2.7);
				float ringW = t * _RingWidth;
				
				//runningpass will be set to 1 or 2 to trigger this event
				if (_RunRingPass >= 1) {
					//find the shaded color. (greyscale effect)
					newColor.rgb = (orgColor.r * 0.2989) + (orgColor.g * 0.5870) + (orgColor.b * 0.1140);
					newColor.a = 1;

					newColor =	newColor * (depthValue < (t - ringW) ) +	//greyscale when inside ring
								orgColor * (depthValue > t && depthValue < 1- ringW) +	//original when outside ring and inside max ring distance
								lerp(orgColor, newColor, t) * (depthValue > 1 - ringW) + //lerp when past max ring distance
								clamp(lerp(newColor, _RingColor, (depthValue - (t - ringW)) / ringW), 0, 1) * (depthValue < t && depthValue > t - ringW && depthValue < 1 - ringW); //ringColor when on the ring
								//lerp(newColor, _RingColor, (depthValue-(t-ringW))/ringW)
					return newColor;
				}
				else {
					return orgColor;
				}
			}
		ENDCG
		}
	}
	FallBack "Diffuse"
}