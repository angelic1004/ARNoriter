Shader "Custom/Ground_Layer" {
	Properties{
		_MainTex("Color", 2D) = "white" {}
		_MaskTex("RGB Mask", 2D) = "white" {}
		_RampTex("Ramp", 2D) = "gray" {}
		_Line("Line Thickness" , Float) = 0
		_LineC("Line Color", Color) = (0,0,0)
		_RColor("Rim Color", Color) = (0.3, 0.3, 0.3)
		_RimPower("Rim Power", Range(0.0,15)) = 15
		_C1("Color 1", Color) = (0.3, 0.3, 0.3)
		_C2("Color 2", Color) = (0.3, 0.3, 0.3)
		_C3("Color 3", Color) = (0.3, 0.3, 0.3)

	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }

		//pass1

		CGPROGRAM
		#pragma surface surf Ramp
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _RampTex;
		sampler2D _MaskTex;
		float3 _RColor;
		float _RimPower;
		half4 _C1, _C2, _C3;
		float4 _LineC;
		float _Line;

		half4 LightingRamp(SurfaceOutput s, half3 lightDir, half atten) {
		half NdotL = dot(s.Normal, lightDir);
		half diff = NdotL * 0.5 + 0.5;
		half3 ramp = tex2D(_RampTex, float2(diff, diff)).rgb;
		half4 c;
		c.rgb = s.Albedo * _LightColor0.rgb * ramp * atten;
		c.a = s.Alpha;
		return c;
		}

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};

		void surf(Input IN, inout SurfaceOutput o) {

			fixed4 base = tex2D(_MainTex, IN.uv_MainTex);
			half4 mask = tex2D(_MaskTex, IN.uv_MainTex);

			half rim = 1.0 - saturate(dot(IN.viewDir, o.Normal));

			half3 cr = base.rgb * _C1;
			half3 cg = base.rgb * _C2;
			half3 cb = base.rgb * _C3;
			half mr = mask.r;
			half mg = mask.g;
			half mb = mask.b;

			half minv = min(mr + mg + mb, 1);

			half3 cf = lerp(lerp(cr, cg, mg*(mr + mg)), cb, mb*(mr + mg + mb));
			half3 c = lerp(base.rgb, cf, minv);

			o.Emission = o.Emission = _RColor.rgb * pow(rim, _RimPower);
			o.Albedo = c.rgb;
			o.Alpha = base.a;
		}
		ENDCG

			//pass2

			cull front

			CGPROGRAM
		#pragma surface surf Lambert noambient vertex:vert
		#pragma target 3.0

			float _Line;
		sampler2D _MainTex;
		float4 _LineC;

		void vert(inout appdata_full v) {
			v.vertex.xyz = v.vertex.xyz + v.normal.xyz * _Line;
		}

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			o.Emission = _LineC.rgb;
			o.Alpha = _LineC.a;
		}
		ENDCG

		}
		FallBack "Diffuse"
}

