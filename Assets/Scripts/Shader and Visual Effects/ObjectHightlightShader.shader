Shader "Custom/ObjectHighlightShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_GlowColor("Glow Color", Color) = (1, 1, 1, 1)
		_GlowSpeed("Glow Speed", Float) = 7
		_GlowWidth("Glow Width", Float) = 2
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
			float vertexPos;
            float2 uv_MainTex;
        };

		void vert(inout appdata_full v, out Input o) {
			 UNITY_INITIALIZE_OUTPUT(Input, o);
			 float3 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz;
			 worldPos -= mul(unity_ObjectToWorld, float4(0,0,0, 1.0)).xyz;
			 float3 camRight = UNITY_MATRIX_V[0].xyz;
			 o.vertexPos = worldPos.y + dot(worldPos.xyz, camRight.xyz); // passes the vertex data to pixel shader, you can change it to a different axis or scale it (divide or multiply the value)
		 }

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		fixed4 _GlowColor;
		float _GlowSpeed;
		float _GlowWidth;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			//find highlight color
			float highlight_value = clamp(cos(_Time.y * _GlowSpeed - IN.vertexPos/_GlowWidth), 0, 1);

            o.Albedo = lerp(c, _GlowColor, highlight_value);
			o.Emission = lerp(fixed4(0,0,0,0), _GlowColor, highlight_value);
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
