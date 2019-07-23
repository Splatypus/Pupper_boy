// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ToonShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_DarknessColor("Darkness color", Color) = (0.4,0.4,0.4,1)
		_ShadowTint("Shadow Tint", Color) = (1,1,1,1)
		_ShadowCutoff("Shadow Tint Cutoff", Float) = 0.95
		_ShadowMult("Shadow Tint Intensity", Float) = 0.85
		_SunspotCutoff("Sunglow Cutoff", Float) = 0.95
		_Sunspot("Sunglow Intensity", Float) = 1
		_EdgeGlowCutoff("Edge Glow Cutoff", Float) = 0.0
		_EdgeGlowIntensity("Edgeglow Intensity", Float) = 1.5
	}
	SubShader {
		
		Tags { "RenderType"="Opaque"} // "LightMode" = "ForwardBase"
		LOD 200

		//surface shader (cel shading)
		CGPROGRAM
		#pragma surface surf CelShadingForward addshadow
		#pragma target 3.0
		

		float _SunspotCutoff;
		float _Sunspot;
		float4 _ShadowTint;
		float _ShadowCutoff;
		float4 _DarknessColor;
		float _ShadowMult;
		float _EdgeGlowCutoff;
		float _EdgeGlowIntensity;
		
		half4 LightingCelShadingForward(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) { //atten is a range 0-1 where 0 is in darkness and 1 is in bright light
			half lightStr = dot(s.Normal, lightDir);
			lightStr -= step(atten, 0.5); //if in shade, subtract 1 from light strength
			//half clampedShade = 1 + clamp(floor(lightStr), -1 + _MinDark, 0); //clamps lightStr rounded down between minDark-1 and 0. Basically, sets it to either 1 or mindark
			half4 c;
			half sunglow = (lightStr > _SunspotCutoff)*_Sunspot + 1; //Apply the sunspot light strength multiplier if this spot is above the cutoff bringhtness 
			c.rgb = s.Albedo * _LightColor0.rgb * (2 * sunglow );
			
			//Apply tint to faces that are within the cutoff range for facing away from light source
			_ShadowTint *= _ShadowMult; //multiply shadow strength by its mult. This is so backlit stuff can be lighter than shadows
			_ShadowTint -= 1;
			_ShadowTint *= ( dot(s.Normal, lightDir) < -(_ShadowCutoff) ); //multiply by 0 if it is not below the cutoff dot product
			_ShadowTint += 1; //add 1 back so colors arent warped. The reason for this is so that shadowtint of 0,0,0 from the above line goes to 1,1,1, which doesnt change color. 
			
			_DarknessColor -= 1;
			_DarknessColor *= (lightStr < 0); //in shade
			_DarknessColor += 1;
			
			//light the edges of the character based on the camera direction
			half edgeLighting = 1;
			//unity_4LightPosX0[0]
			edgeLighting += _EdgeGlowIntensity * (dot(s.Normal, viewDir) < _EdgeGlowCutoff && dot(s.Normal, viewDir) > -_EdgeGlowCutoff);

			c.rgb *= _DarknessColor * _ShadowTint.rgb * _ShadowTint.a * edgeLighting;
			c.a = s.Alpha;
			return c;
		}

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color *_Color.a;
			o.Albedo = c.rgba;
			o.Alpha = c.a;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
