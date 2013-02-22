Shader "Spacefuck/No Depth/Transparent" 
{
	Properties 
	{
		_Color ("Color", Color) = (0,0,0,1)
	}
	SubShader 
	{
		Tags { "Queue" = "Transparent" }
		//LOD 200
			
		Pass
		{
			Name "Base"
			ZWrite Off	
			Blend SrcAlpha OneMinusSrcAlpha     // Alpha blending
			//Blend One One                       // Additive
			//Blend OneMinusDstColor One          // Soft Additive
			//Blend DstColor Zero                 // Multiplicative
			//Blend DstColor SrcColor             // 2x Multiplicative
			//Blend Zero One                       // ???
			
			Color [_Color]
		
//			SetTexture [_Color] 
//			{
//				ConstantColor [_Color]
//				Combine constant
//			}
		}
	} 
	FallBack "Diffuse"
}
