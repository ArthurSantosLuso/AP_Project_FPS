Shader "Custom/Cascata"
{
    // Lâmina de água para a malha Cascata.fbx.
    // UV: U = largura, V = 1 no topo e 0 em baixo (usar "Inverter V" se a malha for ao contrário).
    // Cor de vértice alpha = fade nas laterais e em baixo.
    Properties
    {
        [Header(Cor)]
        _ColorDeep ("Cor funda", Color) = (0.03, 0.18, 0.2, 1)
        _ColorShallow ("Cor clara", Color) = (0.3, 0.62, 0.64, 1)
        _FoamColor ("Cor da espuma", Color) = (0.92, 0.97, 0.97, 1)
        _Opacity ("Opacidade do corpo", Range(0, 1)) = 0.45

        [Header(Fluxo)]
        _FlowSpeed ("Velocidade", Float) = 1.1
        _Acceleration ("Aceleração da queda", Range(0, 1)) = 0.6
        _StreakDensity ("Densidade dos fios", Float) = 38
        _StreakStretch ("Comprimento dos fios", Float) = 10
        _StreakContrast ("Contraste dos fios", Range(0.5, 8)) = 2.2
        [Toggle] _FlipV ("Inverter V", Float) = 0

        [Header(Espuma)]
        _TopFoam ("Espuma no topo", Range(0, 0.5)) = 0.06
        _BottomFoam ("Espuma em baixo", Range(0, 1)) = 0.35
        _FoamStreaks ("Fios brancos", Range(0, 1)) = 0.8
        _EdgeBreakup ("Recorte das bordas", Range(0, 1)) = 0.45

        [Header(Luz)]
        _NormalStrength ("Relevo dos fios", Range(0, 4)) = 1.2
        _Smoothness ("Suavidade", Range(0, 1)) = 0.92
        _SpecularStrength ("Brilho especular", Range(0, 4)) = 1.5
        _FresnelPower ("Fresnel", Range(0.5, 8)) = 3
        _ReflectionStrength ("Reflexo do céu", Range(0, 1)) = 0.5

        [Header(Refracao)]
        [Toggle(_REFRACTION)] _Refraction ("Refração (Opaque Texture)", Float) = 1
        _RefractionStrength ("Força da refração", Range(0, 0.1)) = 0.025

        [Header(Ondulacao)]
        _WobbleAmount ("Ondulação da malha", Range(0, 0.05)) = 0.012
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "Cascata"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma shader_feature_local _REFRACTION

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _ColorDeep;
                half4 _ColorShallow;
                half4 _FoamColor;
                half _Opacity;
                float _FlowSpeed;
                half _Acceleration;
                float _StreakDensity;
                float _StreakStretch;
                half _StreakContrast;
                half _FlipV;
                half _TopFoam;
                half _BottomFoam;
                half _FoamStreaks;
                half _EdgeBreakup;
                half _NormalStrength;
                half _Smoothness;
                half _SpecularStrength;
                half _FresnelPower;
                half _ReflectionStrength;
                half _RefractionStrength;
                half _WobbleAmount;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0; // x = largura, y = distância ao longo da queda (0 topo, 1 fundo)
                float3 positionWS : TEXCOORD1;
                half3 normalWS : TEXCOORD2;
                half4 tangentWS : TEXCOORD3;
                float4 screenPos : TEXCOORD4;
                half4 color : COLOR;
                half fogFactor : TEXCOORD5;
            };

            float Hash(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            // Value noise que repete em x a cada "period" células (para a costura em U não se notar)
            float Noise(float2 p, float period)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);

                float2 i0 = float2(fmod(i.x, period), i.y);
                float2 i1 = float2(fmod(i.x + 1.0, period), i.y);

                float a = Hash(i0);
                float b = Hash(i1);
                float c = Hash(i0 + float2(0, 1));
                float d = Hash(i1 + float2(0, 1));
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            // Coordenada ao longo da queda: a água acelera, por isso os fios esticam ao descer
            float FallCoord(float s)
            {
                return lerp(s, sqrt(saturate(s)) * 1.5, _Acceleration);
            }

            // Duas camadas de fios a velocidades diferentes; devolve 0..1
            float Streaks(float u, float s, float t)
            {
                float fall = FallCoord(s);
                float density = max(_StreakDensity, 1.0);
                float stretch = max(_StreakStretch, 0.1);

                float2 p1 = float2(u * density, (fall - t * _FlowSpeed) * density / stretch);
                float2 p2 = float2(u * density * 2.0 + 17.3, (fall - t * _FlowSpeed * 1.35) * density * 2.0 / stretch);

                float n = Noise(p1, density) * 0.65 + Noise(p2, density * 2.0) * 0.35;
                return n;
            }

            Varyings Vert(Attributes input)
            {
                Varyings output;

                float s = lerp(1.0 - input.uv.y, input.uv.y, _FlipV);

                // Ondulação leve, mais forte em baixo
                float wobble = Noise(float2(input.uv.x * 4.0, FallCoord(s) * 3.0 - _Time.y * _FlowSpeed), 4.0) - 0.5;
                input.positionOS.xyz += input.normalOS * wobble * _WobbleAmount * (0.3 + s);

                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.normalWS = normInputs.normalWS;
                output.tangentWS = half4(normInputs.tangentWS, input.tangentOS.w * GetOddNegativeScale());
                output.screenPos = ComputeScreenPos(posInputs.positionCS);
                output.uv = float2(input.uv.x, s);
                output.color = input.color;
                output.fogFactor = ComputeFogFactor(posInputs.positionCS.z);
                return output;
            }

            half4 Frag(Varyings input, bool isFrontFace : SV_IsFrontFace) : SV_Target
            {
                float t = _Time.y;
                float u = input.uv.x;
                float s = input.uv.y;

                // --- Fios de água ---
                float n = Streaks(u, s, t);
                float streak = pow(saturate(n), _StreakContrast);

                // Gradiente dos fios na largura e na queda (relevo para a luz)
                float eps = 0.5 / max(_StreakDensity, 1.0);
                float nU = Streaks(u + eps, s, t);
                float nS = Streaks(u, s + eps, t);
                float2 grad = float2(nU - n, nS - n) / eps;

                // --- Normal ---
                half3 normalWS = normalize(input.normalWS);
                half3 tangentWS = normalize(input.tangentWS.xyz);
                half3 bitangentWS = cross(normalWS, tangentWS) * input.tangentWS.w;
                if (!isFrontFace)
                    normalWS = -normalWS;
                half3 bumpTS = half3(-grad.x * 0.04 * _NormalStrength, -grad.y * 0.01 * _NormalStrength, 1.0);
                normalWS = normalize(bumpTS.x * tangentWS + bumpTS.y * bitangentWS + bumpTS.z * normalWS);

                float3 viewDirWS = normalize(GetWorldSpaceViewDir(input.positionWS));

                // --- Espuma ---
                float foamNoise = Noise(float2(u * 30.0, (FallCoord(s) - t * _FlowSpeed * 1.1) * 12.0), 30.0);
                float topFoam = (1.0 - smoothstep(0.0, _TopFoam + 0.0001, s)) * (0.6 + 0.4 * foamNoise);
                float bottomFoam = smoothstep(1.0 - _BottomFoam, 1.0, s) * smoothstep(0.35, 0.75, foamNoise + s * 0.3);
                float whiteStreaks = smoothstep(0.62, 0.85, n) * _FoamStreaks * saturate(0.4 + s);
                float foam = saturate(topFoam + bottomFoam + whiteStreaks);

                // --- Cor base ---
                half3 waterColor = lerp(_ColorDeep.rgb, _ColorShallow.rgb, streak);

                // --- Luz ---
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(input.positionWS));
                half shadow = lerp(0.5, 1.0, mainLight.shadowAttenuation);
                half3 lightCol = mainLight.color * shadow;

                half NdotL = saturate(dot(normalWS, mainLight.direction));
                half3 diffuse = waterColor * (lightCol * (0.35 + 0.65 * NdotL) + SampleSH(normalWS));

                half3 halfDir = normalize(mainLight.direction + viewDirWS);
                half specPower = exp2(_Smoothness * 10.0 + 1.0);
                half3 specular = lightCol * pow(saturate(dot(normalWS, halfDir)), specPower) * _SpecularStrength;

                half fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), _FresnelPower);
                half3 reflectDir = reflect(-viewDirWS, normalWS);
                half3 reflection = GlossyEnvironmentReflection(reflectDir, 1.0 - _Smoothness, 1.0) * fresnel * _ReflectionStrength;

                half3 foamLit = _FoamColor.rgb * (lightCol * (0.6 + 0.4 * NdotL) + SampleSH(normalWS));

                half3 surface = diffuse + reflection;
                half bodyAlpha = saturate(_Opacity + streak * 0.35 + fresnel * 0.3);

                // --- Refração: o que está atrás vê-se distorcido e tingido ---
                #if defined(_REFRACTION)
                    float2 screenUV = input.screenPos.xy / input.screenPos.w;
                    float2 offset = (grad * 0.02 + normalWS.xy * 0.5) * _RefractionStrength;
                    half3 behind = SampleSceneColor(screenUV + offset) * lerp(half3(1, 1, 1), _ColorShallow.rgb * 1.4, 0.6);
                    half3 color = lerp(behind, surface, bodyAlpha);
                    half alpha = 1.0;
                #else
                    half3 color = surface;
                    half alpha = bodyAlpha;
                #endif

                color = lerp(color, foamLit, foam) + specular * (1.0 - foam);
                alpha = saturate(max(alpha, foam));

                // --- Recorte das bordas (cor de vértice) ---
                half edge = input.color.a;
                half edgeNoise = Noise(float2(u * 12.0, (FallCoord(s) - t * _FlowSpeed) * 6.0), 12.0);
                edge = saturate((edge - edgeNoise * _EdgeBreakup) / max(1.0 - _EdgeBreakup, 0.05));
                alpha *= edge;

                color = MixFog(color, input.fogFactor);
                return half4(color, alpha);
            }

            ENDHLSL
        }
    }

    FallBack Off
}
