Shader "Custom/TesTerrainShader"
{
    Properties
    {
		//base
		_Color("Color", color) = (1,1,1,0)
		//trample texture
		_TrampledTex("Trampled Texture", 2D) = "white" {}
		_TrampleColor("Trampled Color", color) = (0.5, 0.5, 0.5, 1)
		_TrampleRange("Trample texture vertical fade range", Float) = 0.2
		//tesselation
		_Tess("Tessellation", Range(1,32)) = 8
		_TessMin("Minimum Tessellation Range", Float) = 10.0
		_TessMax("Maximum Tesselation Range", Float) = 40.0
		_DispTex("Disp Texture", 2D) = "gray" {}
				
		//texture positioning details
		[HideInInspector] _WorldToPixel("World to pixel multiplier (set by SnowPhysics script)", Float) = 1
		[HideInInspector] _CameraLocation("Snow Camera Location", Vector) = (0,0,0,0)
		[HideInInspector] _CameraWidth("Camera Pixel Width/Height", Float) = 512
		[HideInInspector] _Range("Camra Clipping Range", Float) = 5

		//Splatmap control texture
		[HideInInspector] _Control("Control (RGBA)", 2D) = "red"{}
		//textures
		[HideInInspector] _Splat0("Layer 0 (R)", 2D) = "white"{}
		[HideInInspector] _Splat1("Layer 1 (G)", 2D) = "white"{}
		[HideInInspector] _Splat2("Layer 2 (B)", 2D) = "white"{}
		[HideInInspector] _Splat3("Layer 3 (A)", 2D) = "white"{}
		
		//normal Maps
		[HideInInspector] _Normal0("Normal 0 (R)", 2D) = "bump" {}
		[HideInInspector] _Normal1("Normal 1 (G)", 2D) = "bump" {}
		[HideInInspector] _Normal2("Normal 2 (B)", 2D) = "bump" {}
		[HideInInspector] _Normal3("Normal 3 (A)", 2D) = "bump" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "SplatCount"="4" "Queue"="Geometry-100"}
        LOD 300

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:disp tessellate:tessDistance nolightmap
        #pragma target 4.6
		#include "Tessellation.cginc"

		/**
		* Tessellation shader
		*/
		struct appdata {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
		};
		half _Tess;
		float _TessMin;
		float _TessMax;
		float3 _CameraLocation;
		float _Range;
		float _CameraWidth;
		float _WorldToPixel;
		sampler2D _DispTex;

		float4 tessDistance(appdata v0, appdata v1, appdata v2) {
			//pixel locations of the verts, converted to use tex2Dlod. See detailed explanation in vertex shader (disp)
			float3 p0 = (_CameraLocation - mul(unity_ObjectToWorld, v0.vertex)) * _WorldToPixel / _CameraWidth + 0.5;
			float3 p1 = (_CameraLocation - mul(unity_ObjectToWorld, v1.vertex)) * _WorldToPixel / _CameraWidth + 0.5;
			float3 p2 = (_CameraLocation - mul(unity_ObjectToWorld, v2.vertex)) * _WorldToPixel / _CameraWidth + 0.5;

			//find displacement at verts and between verts
			float d0 = tex2Dlod(_DispTex, float4(1 - p0.x, p0.z, 0, 0)).r - 0.001;
			float d1 = tex2Dlod(_DispTex, float4(1 - p1.x, p1.z, 0, 0)).r - 0.001;
			float d2 = tex2Dlod(_DispTex, float4(1 - p2.x, p2.z, 0, 0)).r - 0.001;

			float d01 = tex2Dlod(_DispTex, float4(1 - lerp(p0.x, p1.x, 0.5), lerp(p0.z, p1.z, 0.5), 0, 0)).r - 0.001;
			float d12 = tex2Dlod(_DispTex, float4(1 - lerp(p1.x, p2.x, 0.5), lerp(p1.z, p2.z, 0.5), 0, 0)).r - 0.001;
			float d20 = tex2Dlod(_DispTex, float4(1 - lerp(p2.x, p0.x, 0.5), lerp(p2.z, p0.z, 0.5), 0, 0)).r - 0.001;

			//Only do full tesselation on things that are displaced
			if (v0.vertex.y < -d0 * _Range
				&& v1.vertex.y < -d1 * _Range
				&& v2.vertex.y < -d2 * _Range

				&& lerp(v0.vertex.y, v1.vertex.y, 0.5) < -d01 * _Range
				&& lerp(v1.vertex.y, v2.vertex.y, 0.5) < -d12 * _Range
				&& lerp(v2.vertex.y, v0.vertex.y, 0.5) < -d20 * _Range) {
				_Tess = 1;
			}

			return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _Tess);
		}

		/**
		* vertex shader
		*/
		void disp(inout appdata v)
		{

		}

		/**
		* Surface shader
		*/
		sampler2D _Control;
		uniform sampler2D _Splat0, _Splat1, _Splat2, _Splat3;
		struct Input
		{
			float2 uv_Control : TEXCOORD0;
			float2 uv_Splat0 : TEXCOORD1;
			float2 uv_Splat1 : TEXCOORD2;
			float2 uv_Splat2 : TEXCOORD3;
			float2 uv_Splat3 : TEXCOORD4;
		};
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_Control, IN.uv_Control);
			fixed4 s0 = tex2D(_Splat0, IN.uv_Splat0);
			fixed4 s1 = tex2D(_Splat1, IN.uv_Splat1);
			fixed4 s2 = tex2D(_Splat2, IN.uv_Splat2);
			fixed4 s3 = tex2D(_Splat3, IN.uv_Splat3);

			o.Albedo = s0 * c.r + s1 * c.g + s2 * c.b + s3 * c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
