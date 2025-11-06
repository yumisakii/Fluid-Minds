Shader "VertexSurface" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (1,1,1,1)
		_Shininess ("Shininess", Range(0.01,1)) = .5
		_MainTex ("Main Texture", 2D) = "White" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 300
		
		CGPROGRAM
		#pragma surface surf BlinnPhong

		fixed4 _Color;
		half _Shininess;
		sampler2D _MainTex;
									
		struct Input {
			float4 color : COLOR;
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			
			o.Albedo = IN.color * tex.rgb * _Color;
			o.Gloss = tex.a;
			o.Specular = _Shininess;
		}
		ENDCG
	}
	
	FallBack "Diffuse"
}