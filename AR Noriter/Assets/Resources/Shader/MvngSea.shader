Shader "Custom/MvngSea" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_FoamTex ("Foam", 2D) = "black" {}
		_Threshold("Shoreline Distance", Float) = 1
		_FoamInten ("Foam Intensuty", Range(0,1)) = 0
		_WSpeed ("Foam Speed", Float) = 0
		_Speed ("Wave Speed", Float) = 0
		_Height ("Wave Height", Float) = 0
		_Amount("Wave Amount", Float) = 0
		_RimColor("Rim Color", Color) = (0.1, 0.3, 0.2)
		_RimPower("Rim Power", Range(0.0, 10.0)) = 5
	}
	SubShader {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		
		Zwrite Off

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert noambient
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _FoamTex;
		sampler2D _CameraDepthTexture;
		float _Threshold;
		fixed _WSpeed;
		fixed _Speed;
		fixed _Height;
		fixed _Amount;
		fixed4 _RimColor;
		fixed _RimPower;
		fixed _FoamInten;


		struct Input {
			float2 uv_MainTex;
			float2 uv_FoamTex;
			float3 viewDir;
			float3 worldPos;
			float4 screenPos;
		};

		void vert (inout appdata_full v) {
			v.vertex.y += sin(_Time.z * _Speed + (v.vertex.x * v.vertex.z * _Amount)) * _Height;
		}


		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D (_MainTex, float2(IN.uv_MainTex.x + _Time.y * _WSpeed, IN.uv_MainTex.y));
			fixed3 f = tex2D(_FoamTex, float2(IN.uv_FoamTex.y + _Time.y * _WSpeed, IN.uv_FoamTex.x));

			half depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos));
			depth = LinearEyeDepth(depth * _Threshold);
			half4 edgeBlend = abs(1.0 - clamp((depth - IN.screenPos.w), 0.0, 1.0)) * 1;

			half rim = 1.0 - saturate(dot(IN.viewDir, o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower) + (o.Albedo) *  c.rgb + (f.rgb * _FoamInten) + (edgeBlend * c.rgb);
			o.Albedo = c.rgb + (f.rgb * _FoamInten) + (edgeBlend * c.rgb);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
