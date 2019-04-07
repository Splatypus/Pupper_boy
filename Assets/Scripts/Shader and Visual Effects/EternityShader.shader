Shader "Custom/EternityShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Cube("Cubemap", CUBE) = ""{}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        
        #pragma surface surf NoLighting
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
			float3 worldRefl;
			float3 viewDir;
        };

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		sampler2D _MainTex;
		samplerCUBE _Cube;

        void surf (Input IN, inout SurfaceOutput o)
        {
			o.Albedo = texCUBE(_Cube, -IN.viewDir).rgb;  //IN.worldRefl
        }

		

        ENDCG
    }
    FallBack "Diffuse"
}
