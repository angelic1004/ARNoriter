Shader "Custom/VertMove" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Cutoff("Cut out", Range(0,1)) = 0
		_Wind("Wind On", Range(0,1)) = 0
		_Speed("Speed", Range(0,0.1)) = 0
	}
		SubShader{
			Tags {"RenderType"="TransparentCutout" "Queue" = "AlphaTest"}

			cull off

			CGPROGRAM
			#pragma surface surf Lambert vertex:vert alphatest:_Cutoff addshadow
			#pragma target 3.0
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _Wind;
			float _Speed;

			struct appdata {
				float4 vertex : POSITION;
			};

			void vert (inout appdata_full v) {
				v.vertex.y += sin(_Time.y * _Wind) * _Speed * v.color.r;
			}

			struct Input {
				float2 uv_MainTex;
				float4 color:COLOR;
			};

			void surf(Input IN, inout SurfaceOutput o) {
				float4 c = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = c.rgb * IN.color.rgb;
				o.Alpha = c.a;
			}

			ENDCG
			
		}
			FallBack "Diffuse"
}