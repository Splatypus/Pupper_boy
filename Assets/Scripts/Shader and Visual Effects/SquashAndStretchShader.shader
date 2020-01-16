Shader "Custom/SquashAndStretchShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_Amount("Squish Amount", Range(0,1)) = 0
		_Speed("Speed Multiplier", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

		float _Amount;
		float _Speed;
		void vert(inout appdata_full v) 
		{
			//Height deforms with Sin of time
			v.vertex.y += v.vertex.y * _Amount * sin(_Time.y * _Speed);
			//Horizontal dimensions deform with Cos of time
			v.vertex.x += v.vertex.x * _Amount * -sin(_Time.y * _Speed);
			v.vertex.z += v.vertex.z * _Amount * -sin(_Time.y * _Speed);
			//v.vertex.xyz += v.normal * _Amount;
		}

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
