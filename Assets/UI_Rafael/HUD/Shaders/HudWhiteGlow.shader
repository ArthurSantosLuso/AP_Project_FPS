Shader "Rafael/HUD/White Glow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite sheet", 2D) = "white" {}
        _Color ("Glow tint", Color) = (0.85,0.93,1,0.7)
        _SpriteUVRect ("Sprite UV bounds", Vector) = (0,0,1,1)
        _BlurStep ("Blur step in sprite UV", Vector) = (0.01,0.01,0,0)
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Stencil { Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] }
        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha One
        ColorMask [_ColorMask]
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            struct Input { float4 vertex:POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
            struct Output { float4 vertex:SV_POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; float4 local:TEXCOORD1; };
            sampler2D _MainTex;
            float4 _SpriteUVRect, _BlurStep, _ClipRect;
            fixed4 _Color;
            Output vert(Input v)
            {
                Output o; o.local=v.vertex; o.vertex=UnityObjectToClipPos(v.vertex);
                o.uv=v.uv; o.color=v.color*_Color; return o;
            }
            float whiteMask(float2 uv)
            {
                if (any(uv < 0) || any(uv > 1)) return 0;
                fixed4 source=tex2D(_MainTex,lerp(_SpriteUVRect.xy,_SpriteUVRect.zw,uv));
                float white=smoothstep(0.55,0.92,min(source.r,min(source.g,source.b)));
                return source.a*white;
            }
            fixed4 frag(Output i):SV_Target
            {
                float blur=0;
                const float weights[5]={1,4,6,4,1};
                [unroll] for (int x=-2;x<=2;x++)
                    [unroll] for (int y=-2;y<=2;y++)
                        blur+=whiteMask(i.uv+float2(x,y)*_BlurStep.xy)*weights[x+2]*weights[y+2]/256.0;
                fixed4 result=fixed4(i.color.rgb,blur*i.color.a);
                #ifdef UNITY_UI_CLIP_RECT
                result.a*=UnityGet2DClipping(i.local.xy,_ClipRect);
                #endif
                return result;
            }
            ENDCG
        }
    }
}
