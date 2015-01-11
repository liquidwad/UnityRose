Shader "Custom/TerrainShader" {
	Properties {
		_BottomTex ("Bottom (RGBA)", 2D) = "white" {}
		_TopTex ("Top (RGBA)", 2D) = "black" {}
		_LightTex ("Light (RGBA)", 2D) = "white" {}
		_ColorTint ("Tint", Color) = (1.0, 0.6, 0.6, 1.0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert
		//#pragma surface surf Lambert finalcolor:mycolor

		sampler2D _BottomTex;
		sampler2D _TopTex;
		sampler2D _LightTex;
		fixed4 _ColorTint;

		struct Input {
			float2 uv_BottomTex;
			float2 uv_LightTex;
		};

		
      	//void mycolor (Input IN, SurfaceOutput o, inout fixed4 color)
      //	{
        //  	color *= _ColorTint;
      	//}
		void surf (Input IN, inout SurfaceOutput o) {
			half4 bottom =  tex2D (_BottomTex, IN.uv_BottomTex);
			half4 top = tex2D (_TopTex, IN.uv_BottomTex);
			half4 light = tex2D (_LightTex, IN.uv_LightTex;
			half4 tint = half4(_ColorTint);
			o.Albedo = bottom.rgb*(1.0f - top.a) + tint.rgb*0.0f + top.rgb*top.a;
			o.Albedo = o.Albedo.rgb*(1.0f - light.a) + light.rgb*light.a;
			o.Alpha = bottom.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
