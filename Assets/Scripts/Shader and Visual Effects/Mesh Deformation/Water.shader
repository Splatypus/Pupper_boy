Shader "Custom/Water" {
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_MoveScale("Wave Height", float) = 2
		_Speed("Wave Speed", float) = 5
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		//#pragma surface surf Lambert vertex:vert
		#pragma surface surf BlinnPhong vertex:vert addshadow
		struct Input {
			float2 uv_MainTex;
		};
		
		//transform the verts of this object to form waves based off the x,y values and time
		float _MoveScale;
		float _Speed;
		void vert (inout appdata_full v) {
			//make wave effect related to world position rather than local
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			v.vertex.y += sin(_Time.y * _Speed + worldPos.x + worldPos.z) * _MoveScale; //waves
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
