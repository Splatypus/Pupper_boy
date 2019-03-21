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
			_SpecColor("Spec color", color) = (0.5,0.5,0.5,0.5)
			//Hiden attributes (set in SnowPhysics class)
			[HideInInspector]
			_Range("Camra Clipping Range", Float) = 5
			[HideInInspector]
			_WorldToPixel("World to pixel multiplier (set by SnowPhysics script)", Float) = 1
			[HideInInspector]
			_CameraLocationX("Snow Camera X", Float) = 0
			[HideInInspector]
			_CameraLocationZ("Smow Camera Z", Float) = 0
			[HideInInspector]
			_CameraWidth("Camera Pixel Width/Height", Float) = 512
			[HideInInspector]
			_TerrainHeightMap("Terrain height map", 2D) = "black"{}
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

            half _Tess;  
			float _TessMin;
			float _TessMax;
			float _Range;
			float _WorldToPixel;
			float _CameraLocationX;
			float _CameraLocationZ;
			float _CameraWidth;
			sampler2D _DispTex;
			sampler2D _TerrainHeightMap;

			

			//tesselation shader
            float4 tessDistance (appdata v0, appdata v1, appdata v2) {
				//pixel locations of the verts, converted to use tex2Dlod. See detailed explanation in vertex shader (disp)
				float3 p0 = ( float3(_CameraLocationX, 0, _CameraLocationZ) - mul(unity_ObjectToWorld, v0.vertex) ) * _WorldToPixel / _CameraWidth + 0.5;
				float3 p1 = ( float3(_CameraLocationX, 0, _CameraLocationZ) - mul(unity_ObjectToWorld, v1.vertex) ) * _WorldToPixel / _CameraWidth + 0.5;
				float3 p2 = ( float3(_CameraLocationX, 0, _CameraLocationZ) - mul(unity_ObjectToWorld, v2.vertex) ) * _WorldToPixel / _CameraWidth + 0.5;

				//Only do full tesselation on things that are displaced
				//should check midpoints between each vert too
				//Note: This is called before the vertex shader, so v.vertex.y represents its original position
				float d0 = tex2Dlod(_DispTex, float4(1- p0.x, p0.z,0,0)).r -0.001;
				float d1 = tex2Dlod(_DispTex, float4(1- p1.x, p1.z,0,0)).r -0.001;
				float d2 = tex2Dlod(_DispTex, float4(1- p2.x, p2.z,0,0)).r -0.001;

				float d01 = tex2Dlod(_DispTex, float4(1- lerp(p0.x, p1.x, 0.5), lerp(p0.z, p1.z, 0.5),  0,0)).r -0.001;
				float d12 = tex2Dlod(_DispTex, float4(1- lerp(p1.x, p2.x, 0.5), lerp(p1.z, p2.z, 0.5),  0,0)).r -0.001;
				float d20 = tex2Dlod(_DispTex, float4(1- lerp(p2.x, p0.x, 0.5), lerp(p2.z, p0.z, 0.5),  0,0)).r -0.001;


				if(	v0.vertex.y < -d0*_Range
					&& v1.vertex.y < -d1*_Range 
					&& v2.vertex.y < -d2* _Range

					&& lerp(v0.vertex.y, v1.vertex.y, 0.5) < -d01* _Range
					&& lerp(v1.vertex.y, v2.vertex.y, 0.5) < -d12* _Range
					&& lerp(v2.vertex.y, v0.vertex.y, 0.5) < -d20* _Range){
					_Tess = 1;
				}

                return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _Tess);
            }

			//vertex shader
            void disp (inout appdata v)
            {
				//camera location - worldpsace location : gets the vector from the vector location to the snow Camera
				// * _WorldToPixel : converts that vector to pixel distances
				// /_CameraWidth + 0.5 : adjusts the result for tex2Dlod
				float3 pixelLoc = (float3(_CameraLocationX, 0, _CameraLocationZ) - mul(unity_ObjectToWorld, v.vertex)) * _WorldToPixel;
                float d = tex2Dlod(_DispTex, float4(1-(pixelLoc.x / _CameraWidth + 0.5), (pixelLoc.z / _CameraWidth + 0.5), 0,0)).r;
				float halfWidth = _CameraWidth * 0.5;
				//if true, d = d, if false, d = 0
				//this is to stop artifacting that results from texture wrapping
				d *= (pixelLoc.x >= ( -halfWidth)) &&
					(pixelLoc.x <= ( halfWidth)) &&
					(pixelLoc.z >= ( -halfWidth)) &&
					(pixelLoc.z <= (halfWidth));
                //then find terrain height at this point
				float th = tex2Dlod(_TerrainHeightMap, float4(v.texcoord.x, v.texcoord.y, 0, 0)).a; //float4(mul(unity_ObjectToWorld, v.vertex).xz/1024, 0, 0)).r;
				v.vertex.y = -d*_Range;
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
