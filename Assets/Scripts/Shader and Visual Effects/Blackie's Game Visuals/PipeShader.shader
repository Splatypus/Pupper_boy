Shader "Custom/PipeShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Cube("Cubemap", CUBE) = ""{}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
	SubShader
	{
		Tags { "Queue" = "Geometry" "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		struct Input
		{
			float3 viewDir;
		};

		fixed4 _Color;
		samplerCUBE _Cube;
		half _Glossiness;
		half _Metallic;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = texCUBE(_Cube, -IN.viewDir).rgb * _Color;
			o.Alpha = _Color.a;
			
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Emission = o.Albedo;
		}
		
		ENDCG
	
	}
		FallBack "Diffuse"
}
