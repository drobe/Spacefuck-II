Shader "Spacefuck/No Depth/No Depth"  
{
	Properties 
	{
		_Color ("Color", Color) = (0,0,0,1)
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass
		{
			Blend One Zero
			Color [_Color]
		}
	} 
	FallBack "Diffuse"
}
