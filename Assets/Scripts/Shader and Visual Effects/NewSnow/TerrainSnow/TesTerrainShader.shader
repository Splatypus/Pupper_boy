Shader "Custom/TesTerrainShader"
{
	Properties
	{
		//base
		_Color("Color", color) = (1,1,1,0)
		_SnowDepth("Snow Depth", Float) = 1
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
		#pragma surface surf Standard addshadow fullforwardshadows vertex:disp tessellate:tessDistance nolightmap
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
		half _SnowDepth;
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

			//find depth values at verts and between verts
			float d0 = tex2Dlod(_DispTex, float4(1 - p0.x, p0.z, 0, 0)).r;
			float d1 = tex2Dlod(_DispTex, float4(1 - p1.x, p1.z, 0, 0)).r;
			float d2 = tex2Dlod(_DispTex, float4(1 - p2.x, p2.z, 0, 0)).r;

			float d01 = tex2Dlod(_DispTex, float4(1 - lerp(p0.x, p1.x, 0.5), lerp(p0.z, p1.z, 0.5), 0, 0)).r;
			float d12 = tex2Dlod(_DispTex, float4(1 - lerp(p1.x, p2.x, 0.5), lerp(p1.z, p2.z, 0.5), 0, 0)).r;
			float d20 = tex2Dlod(_DispTex, float4(1 - lerp(p2.x, p0.x, 0.5), lerp(p2.z, p0.z, 0.5), 0, 0)).r;

			//Do not perform full tesselation on things that have a higher designated height than their current (pre-vertex shader) height
			//desired height is the snowcam location, +1 (range starts 1 unit above camera) + camera range - displacement*range
			//default height is just the worldspace location of the vert
			if (	(_CameraLocation.y + 1 + _Range - _Range * d0) > (mul(unity_ObjectToWorld, v0.vertex).y + _SnowDepth) &&
					(_CameraLocation.y + 1 + _Range - _Range * d1) > (mul(unity_ObjectToWorld, v1.vertex).y + _SnowDepth) &&
					(_CameraLocation.y + 1 + _Range - _Range * d2) > (mul(unity_ObjectToWorld, v2.vertex).y + _SnowDepth) &&

					(_CameraLocation.y + 1 + _Range - _Range * d01) > (mul(unity_ObjectToWorld, lerp(v0.vertex, v1.vertex, 0.5)).y + _SnowDepth) &&
					(_CameraLocation.y + 1 + _Range - _Range * d12) > (mul(unity_ObjectToWorld, lerp(v1.vertex, v2.vertex, 0.5)).y + _SnowDepth) &&
					(_CameraLocation.y + 1 + _Range - _Range * d20) > (mul(unity_ObjectToWorld, lerp(v2.vertex, v0.vertex, 0.5)).y + _SnowDepth)
				
					) {
				_Tess = 1;
			}

			return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _Tess);
		}

		/**
		* vertex shader
		*/
		void disp(inout appdata v)
		{
			//camera location - worldpsace location : gets the vector from the vector location to the snow Camera
			// * _WorldToPixel : converts that vector to pixel distances
			// /_CameraWidth + 0.5 : adjusts the result for tex2Dlod
			float3 pixelLoc = (_CameraLocation - mul(unity_ObjectToWorld, v.vertex)) * _WorldToPixel;
			float d = tex2Dlod(_DispTex, float4(1 - (pixelLoc.x / _CameraWidth + 0.5), (pixelLoc.z / _CameraWidth + 0.5), 0, 0)).r;

			//y position is the lower of the worldspace position of displaced snow and the snow max height
			v.vertex.y = min(	_CameraLocation.y + 1 + _Range - _Range*d,	mul(unity_ObjectToWorld, v.vertex).y + _SnowDepth	) + 150;
		
		}

		/**
		* Surface shader
		*/
		sampler2D _Control;
		uniform sampler2D _Splat0, _Splat1, _Splat2, _Splat3;
		fixed4 _TrampleColor;
		float _TrampleRange;
		sampler2D _TrampledTex;

		struct Input{
			float2 uv_Control : TEXCOORD0;
			float2 uv_Splat0 : TEXCOORD1;
			float2 uv_Splat1 : TEXCOORD2;
			float2 uv_Splat2 : TEXCOORD3;
			float2 uv_Splat3 : TEXCOORD4;
			float3 worldPos;
		};

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			//Find the combined splatmap main color of the terrain at this texel
            fixed4 c = tex2D (_Control, IN.uv_Control);
			fixed4 s0 = tex2D(_Splat0, IN.uv_Splat0);
			fixed4 s1 = tex2D(_Splat1, IN.uv_Splat1);
			fixed4 s2 = tex2D(_Splat2, IN.uv_Splat2);
			fixed4 s3 = tex2D(_Splat3, IN.uv_Splat3);
			fixed4 splatFinal = s0 * c.r + s1 * c.g + s2 * c.b + s3 * c.a;

			//find the trampled color (uv-ed the same as splat0
			half4 trampleC = tex2D(_TrampledTex, IN.uv_Splat0) * _TrampleColor;
			float3 localPos = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz; //local position is the difference between world position and the terrain's origin in world position
			float scale = clamp((0 - localPos.y) / _TrampleRange, 0, 1); //how far down its trampled / trample distance

			o.Albedo = lerp(splatFinal, trampleC, scale);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
