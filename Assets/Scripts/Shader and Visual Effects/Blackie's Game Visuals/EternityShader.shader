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
        
        #pragma surface surf Standard
        #pragma target 3.0


        struct Input
        {
            float2 uv_MainTex;
			float3 worldRefl;
        };

		sampler2D _MainTex;
		samplerCUBE _Cube;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * 0.5;
			o.Emission = texCUBE(_Cube, IN.worldRefl).rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
