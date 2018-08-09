// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/OutsideBox" {
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_GroundVariance("Ground Variance", Color) = (1, 1, 1, 1)
		_FadeMin("Fade minimal", Float) = 0
		_FadeMax("Fade maximal", Float) = 10
		_FadeColor("Fade Color", Color) = (1,1,1,1)
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
		// make fog work
#pragma multi_compile_fog

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float3 worldPos : TEXCOORD1;
		UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
	};

	sampler2D _MainTex;
	sampler2D _NoiseTex;
	float4 _MainTex_ST;
	fixed4 _Color;
	fixed4 _GroundVariance;
	fixed _FadeMin;
	fixed _FadeMax;
	fixed4 _FadeColor;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.worldPos = mul(unity_ObjectToWorld, v.vertex);
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 texColRaw = tex2D(_MainTex, i.uv);
		fixed alphaTex = texColRaw.a / 15;
		fixed4 texCol = fixed4(texColRaw.r, texColRaw.g, texColRaw.b, 1) * (1 + alphaTex);

		fixed backfaceValue = 1 - clamp((i.worldPos.y - _FadeMin) / (_FadeMax - _FadeMin) * (texColRaw.a * 4), 0, 1);
		fixed4 col = lerp(texCol * _Color, texCol * _FadeColor, 1 - backfaceValue);
		col = lerp(col, _GroundVariance, clamp(pow(texColRaw.a + 0.1, 20), 0, 1) * backfaceValue);
		return col;
	}
		ENDCG
	}
	}
}
