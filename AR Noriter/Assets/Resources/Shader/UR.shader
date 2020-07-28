Shader "Custom/Unlit_Rim" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_RimColor("Rim Color", Color) = (0.1, 0.3, 0.2)
		_RimPower("Rim Power", Range(0.0, 10.0)) = 5
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }

		CGPROGRAM
		#pragma surface surf Lambert noambient
		#pragma target 3.0

		sampler2D _MainTex;
		float4 _RimColor;
		float _RimPower;
		struct Input {
		float2 uv_MainTex;
		float3 viewDir;
		};

		void surf(Input IN, inout SurfaceOutput o) {
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
		half rim = 1.0 - saturate(dot(IN.viewDir, o.Normal));
		o.Emission = ((tex2D(_MainTex, IN.uv_MainTex).rgb) - 0.1) + (_RimColor.rgb * pow(rim, _RimPower));
		}
		ENDCG
		}
		FallBack "Unlit"
}