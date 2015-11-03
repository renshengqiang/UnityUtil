// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "MoreFun/UI-DefaultAlpha" {
Properties {
    [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
	_Color ("Tint", Color) = (1,1,1,1)
	
	_StencilComp ("Stencil Comparison", Float) = 8
	_Stencil ("Stencil ID", Float) = 0
	_StencilOp ("Stencil Operation", Float) = 0
	_StencilWriteMask ("Stencil Write Mask", Float) = 255
	_StencilReadMask ("Stencil Read Mask", Float) = 255

	_ColorMask ("Color Mask", Float) = 15
}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			fixed4 _Color;
			
			struct a2v {
                float4 vertex 	: POSITION;
                float2 texcoord : TEXCOORD0;
                float2 texcoord1: TEXCOORD1;
                float4 color 	: COLOR;
            };

            struct v2f {
                float4 pos 		: SV_POSITION;
                half2 uv0 		: TEXCOORD0;
                half2 uv1 		: TEXCOORD1;
                float4 color 	: COLOR;
            };
            
            v2f vert (a2v v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv0 = v.texcoord;
                o.uv1 = v.texcoord1;
#ifdef UNITY_HALF_TEXEL_OFFSET
				o.pos.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
                o.color = v.color * _Color;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
				fixed4 col_i = i.color;
                fixed4 col = tex2D(_MainTex, i.uv0) * col_i;
                fixed4 col2 = tex2D(_MainTex, i.uv1);
                
               return fixed4(col.r, col.g, col.b, (col2.r + col2.g + col2.b) * col.a);
            }
		ENDCG
		}
	}
}
