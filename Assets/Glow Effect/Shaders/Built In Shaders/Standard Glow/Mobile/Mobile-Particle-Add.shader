// Simplified Additive Particle shader. Differences from regular Additive Particle one:
// - no Tint color
// - no Smooth particle support
// - no AlphaTest
// - no ColorMask

Shader "Mobile/Particles/Additive (Glow)" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
	_GlowTex ("Glow Texture", 2D) = "white" {}
	_GlowColor ("Glow Color", Color) = (1, 1, 1, 1)
	_GlowColorMult ("Glow Color Multiplier", Color) = (1, 1, 1, 1)
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="GlowTransparent" }
	Blend SrcAlpha One
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}
		}
	}
}
}