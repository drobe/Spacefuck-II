Shader "Spacefuck/Spherical Outline" 
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
			Name "SphereBASE"
			Blend Zero One

			CGPROGRAM
			#pragma vertex vertB
			#pragma fragment fragB
			v2f vertB(appdata v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			
				float3 norm   	= mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				float2 offset 	= TransformViewToProjection(norm.xy);
				
				o.pos.xy 		-= offset * _Outline;
				return o;
			}
	
			half4 fragB(v2f i) :COLOR 
			{
				return (0,0,0,0);
			}
			ENDCG
		}
		
		Pass 
		{
			Name "SphereOUTLINE"
			Cull Front

			CGPROGRAM
			#pragma vertex vertO
			#pragma fragment fragO
			v2f vertO(appdata v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
							
				o.color = _OutlineColor;
				return o;
			}

			half4 fragO(v2f i) :COLOR 
			{
				return i.color;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}