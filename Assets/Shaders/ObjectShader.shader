Shader "Custom/ObjectShader" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_LightTex ("Light (RGB)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 200
	
CGPROGRAM
#pragma surface surf Lambert alphatest:_Cutoff

sampler2D _MainTex;
sampler2D _LightTex;
fixed4 _Color;

struct Input {
	float2 uv_MainTex;
	float2 uv2_LightTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	half4 l = tex2D(_LightTex, IN.uv2_LightTex);
	o.Albedo = c.rgb;
	o.Alpha = c.a;
	o.Emission = o.Albedo*l*2.0;
}
ENDCG
}

Fallback "Transparent/Cutout/VertexLit"
}
