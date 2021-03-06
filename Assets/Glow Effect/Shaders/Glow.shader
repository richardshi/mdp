// A simple unlit shader. _Glow* properties are not used in this shader but are used by the replacement shader.
Shader "Glow Effect/Glow"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_GlowTex ("Glow Texture", 2D) = "white" {}
		_GlowColor ("Glow Color", Color) = (1, 1, 1, 1)
		_GlowColorMult ("Glow Color Multiplier", Color) = (1, 1, 1, 1)
	}
    
    SubShader
	{
        Tags { "RenderType" = "Glow" "Queue" = "Geometry" }
        
        Pass {
        
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform half4 _GlowColor;
			uniform half4 _GlowColorMult;
			
			half4 frag(v2f_img i) : COLOR
			{
				return tex2D(_MainTex,i.uv);
			}
			
			ENDCG
        } 
    } 
	
	Fallback "Diffuse"
	CustomEditor "GlowMaterialInspector"
}