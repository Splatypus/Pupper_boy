// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/MeshDeformShader" {
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_PlayerLoc("Player Location", Vector) = (0,0,0,0)
		_MoveScale("Push Strength", float) = 2
		_DisFall("Distance Falloff Exponent", float) = 1.4
		_HeightEffect("Height Exponent", float) = 1.5
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		//#pragma surface surf Lambert vertex:vert
		#pragma surface surf BlinnPhong vertex:vert addshadow
		struct Input {
			float2 uv_MainTex;
		};
		
		//transform the verts of this object away from the player
		float4 _PlayerLoc;
		float _MoveScale;
		float _DisFall;
		float _HeightEffect;
		void vert (inout appdata_full v) {
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			float3 fromPlayer = (worldPos.xyz - _PlayerLoc.xyz);
			v.vertex.x += (sin(_Time.y) * 0.06) * pow(v.vertex.y, _HeightEffect); //wind
			//move verts away from player
			v.vertex.xz += fromPlayer.xz * pow(v.vertex.y, _HeightEffect) * _MoveScale / max(pow(dot(fromPlayer, fromPlayer), _DisFall), 1); //dot(a,a) gets the square magnitude of a
		}

		fixed4 _Color;
		sampler2D _MainTex;
		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}

		ENDCG
    } 
    Fallback "Diffuse"
  }
