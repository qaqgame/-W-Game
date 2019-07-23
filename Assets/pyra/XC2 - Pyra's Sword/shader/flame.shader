Shader "Unlit/flame"
{
    Properties
	{
		//噪音纹理流动速度
		_SpeedX("SpeedX", Range(-10,10)) = 1
		_SpeedY("SpeedY", Range(-10,10)) = 1
 
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_DistortTex("Distort Texture", 2D) = "white" {}
 
		//控制整体的alpha值
		_MainAlphaControl("MainAlphaControl", Range(1,20)) = 1
		_EdgeAlphaControl("EdgeAlphaControl", Range(1,50)) = 1
 
		//控制渐变纹理
		_Height("Height", Range(-4,10)) = 1
 
		//火焰边缘大小
		_Edge("Edge", Range(0.02,0.3)) = 0.1
 
		//控制扭曲强度
		_Distort("Distort", Range(0,1)) = 0.2
 

        _Texture("Texture",2D) = "white" {}
	}
		SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 100
		Zwrite Off Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		
 
		Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
 
		#include "UnityCG.cginc"
 
		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};
 
	struct v2f
	{
		float2 uv : TEXCOORD1;
		float2 uv2 : TEXCOORD2;
        float2 uv_t : TEXCOORD3;
		float4 vertex : SV_POSITION;
 
	};
 
	float _SpeedX, _SpeedY;
	sampler2D  _NoiseTex, _DistortTex;
	sampler2D _Texture;
	float4 _DistortTex_ST, _NoiseTex_ST, _Texture_ST;
	float _Height, _Edge, _Distort, _MainAlphaControl, _EdgeAlphaControl;
	
 
	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
		o.uv2 = TRANSFORM_TEX(v.uv, _DistortTex);
        o.uv_t = TRANSFORM_TEX(v.uv, _Texture);
		return o;
	}
 
	fixed4 frag(v2f i) : SV_Target
	{
		//可控制的渐变纹理
		float4 gradientBlend = lerp(float4(2,2,2,2), float4(0,0,0,0), (i.uv2.y + _Height));
 
		fixed4 distort = tex2D(_DistortTex, i.uv2) * _Distort;
		//使用扭曲纹理来取样噪音纹理
		fixed4 noise = tex2D(_NoiseTex,fixed2((i.uv.x + _Time.x * _SpeedX) + distort.g  ,(i.uv.y + _Time.x * _SpeedY) + distort.r));
 
		noise += gradientBlend;
		noise += gradientBlend;
 
        float4 fcolor = tex2D(_Texture,i.uv_t);

		float4 flame = float4(noise.rgb, saturate(noise.a * _MainAlphaControl));
		float4 flamecolor = flame * fcolor;
		float4 flameedge = saturate((flame + _Edge) * _EdgeAlphaControl) - flame;
		flameedge.a = 1 - flameedge.a;
		float4 edgecolor = flameedge * fcolor;
		float4 finalcolor = flamecolor + edgecolor;
		return finalcolor;
	}
		ENDCG
	   }
    }
}

