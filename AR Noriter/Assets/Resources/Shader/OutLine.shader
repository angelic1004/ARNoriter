Shader "Custom/OutLine" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Line ("Line Thickness" , Float) = 0
		_LineC ("Line Color", Color) = (0,0,0)
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }

		//pass1

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

		void surf (Input IN, inout SurfaceOutput o) {
			o.Emission = _LineC.rgb;
			o.Alpha = _LineC.a;
		}
		ENDCG

		//pass2

			cull back

			CGPROGRAM
		#pragma surface surf Lambert noambient
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Emission = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG

		}
		FallBack "Unlit"
}
