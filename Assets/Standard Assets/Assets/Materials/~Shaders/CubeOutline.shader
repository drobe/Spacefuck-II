Shader "Spacefuck/Cube Outline" 
{
	Properties 
	{
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Float) = 0
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	
	struct appdata 
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};
	
	struct v2f 
	{
		float4 pos : POSITION;
		float4 color : COLOR;
	};
	
	uniform float _Outline;
	uniform float4 _OutlineColor;
	ENDCG

	SubShader 
	{
		Tags { "Queue" = "Transparent" }
		
		Pass 
		{
			Name "CubeBASE"
			Blend Zero One

			CGPROGRAM
			#pragma vertex vertCB
			#pragma fragment fragCB
			v2f vertCB(appdata v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}

	
			half4 fragCB(v2f i) :COLOR 
			{
				return (0,0,0,0);
			}
			ENDCG
		}
		
		Pass 
		{
			Name "CubeOUTLINE"
			Cull Front

			CGPROGRAM
			#pragma vertex vertCO
			#pragma fragment fragCO			
			v2f vertCO(appdata v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			
				float3 norm   = mul ((float3x3)UNITY_MATRIX_MV, v.vertex.xyz);
				float2 offset = TransformViewToProjection(norm.xy);
				
				o.pos.xy 		+= offset * _Outline;
				o.color = 		_OutlineColor;
				return o;
			}

			half4 fragCO(v2f i) :COLOR 
			{
				return i.color;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}