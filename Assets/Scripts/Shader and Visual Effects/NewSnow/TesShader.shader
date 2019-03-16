Shader "Custom/TesShader" {
        Properties {
            _Tess ("Tessellation", Range(1,32)) = 4
			_TessMin ("Minimum Tessellation Range", Float) = 10.0
			_TessMax ("Maximum Tesselation Range", Float) = 30.0
            _MainTex ("Base (RGB)", 2D) = "white" {}
			_Color("Color", color) = (1,1,1,0)
			_TrampledTex("Trampled Texture", 2D) = "white" {}
			_TrampleColor("Trampled Color", color) = (0.5, 0.5, 0.5, 1)
			_TrampleRange("Trample texture vertical fade range", Float) = 0.2
            _DispTex ("Disp Texture", 2D) = "gray" {}
            _NormalMap ("Normalmap", 2D) = "bump" {}
            _SpecColor ("Spec color", color) = (0.5,0.5,0.5,0.5)
			_Range("Camra Clipping Range", Float) = 5

        }
        SubShader {
            Tags { "RenderType"="Opaque" }
            LOD 300
            
            CGPROGRAM
            #pragma surface surf BlinnPhong addshadow fullforwardshadows vertex:disp tessellate:tessDistance nolightmap
            #pragma target 4.6
            #include "Tessellation.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            float _Tess;  
			float _TessMin;
			float _TessMax;
			float _Range;
			sampler2D _DispTex;

			//tesselation shader
            float4 tessDistance (appdata v0, appdata v1, appdata v2) {
				
				//Only do full tesselation on things that are displaced
				//should check midpoints between each vert too
				float d0 = tex2Dlod(_DispTex, float4(1- v0.texcoord.x, v0.texcoord.y,0,0)).r -0.001;
				float d1 = tex2Dlod(_DispTex, float4(1- v1.texcoord.x, v1.texcoord.y,0,0)).r -0.001;
				float d2 = tex2Dlod(_DispTex, float4(1- v2.texcoord.x, v2.texcoord.y,0,0)).r -0.001;

				float d01 = tex2Dlod(_DispTex, float4(1- (v0.texcoord.x + v1.texcoord.x)/2, (v0.texcoord.y + v1.texcoord.y)/2,  0,0)).r -0.001;
				float d12 = tex2Dlod(_DispTex, float4(1- (v1.texcoord.x + v2.texcoord.x)/2, (v1.texcoord.y + v2.texcoord.y)/2,  0,0)).r -0.001;
				float d20 = tex2Dlod(_DispTex, float4(1- (v2.texcoord.x + v0.texcoord.x)/2, (v2.texcoord.y + v0.texcoord.y)/2,  0,0)).r -0.001;


				if(	v0.vertex.y < -d0*_Range
					&& v1.vertex.y < -d1*_Range 
					&& v2.vertex.y < -d2* _Range

					&& (v0.vertex.y + v1.vertex.y)/2 < -d01* _Range
					&& (v1.vertex.y + v2.vertex.y)/2 < -d12* _Range
					&& (v2.vertex.y + v0.vertex.y)/2 < -d20* _Range){
					_Tess = 1;
				}

                return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _Tess);
            }


			//vertex shader
            void disp (inout appdata v)
            {
                float d = tex2Dlod(_DispTex, float4(1- v.texcoord.x, v.texcoord.y,0,0)).r;
                //v.vertex.xyz += v.normal * d;
				v.vertex.y = - (d*_Range); //TODO: y = min of initial height and displaced height rather than just displaced height
            }

            struct Input {
				float4 pos : SV_POSITION;
                float2 uv_MainTex;
				float2 uv_TrampleTex;
				float3 worldPos;
            };

            sampler2D _MainTex;
            sampler2D _NormalMap;
			sampler2D _TrampledTex;
            fixed4 _Color;
			fixed4 _TrampleColor;
			float _TrampleRange;

			//surface shader
            void surf (Input IN, inout SurfaceOutput o) {
                half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
				half4 trampleC = tex2D(_TrampledTex, IN.uv_MainTex) * _TrampleColor;
				float3 localPos = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
				float scale = clamp( (0 - localPos.y) / _TrampleRange, 0, 1);
				o.Albedo = lerp(c, trampleC, scale);
                o.Specular = 0.2;
                o.Gloss = 1.0;
                o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
            }
            ENDCG
        }
        FallBack "Diffuse"
    }
