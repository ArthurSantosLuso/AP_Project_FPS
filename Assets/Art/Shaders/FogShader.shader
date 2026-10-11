Shader "Custom/FogShader"
{
    Properties
    {
        fogColor ("Color", Color) = (1, 1, 1, 1)
        maxDistance("Max distance", float) = 100
        stepSize ("Step size", Range(0.1, 20)) = 1
        densityMult ("Density multiplier", Range(0, 10)) = 1
        noiseOffset ("Noise offset", float) = 0 
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            float4 fogColor;
            float maxDistance;
            float stepSize;
            float densityMult;
            float noiseOffset;

            float get_density()
            {
                return densityMult;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, IN.texcoord);
                float depth = SampleSceneDepth(IN.texcoord);
                float3 worldPos = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP);
                
                float3 startPoint = _WorldSpaceCameraPos;
                
                float3 viewDir = worldPos - _WorldSpaceCameraPos;
                float viewLenght = length(viewDir);
                float3 rayDir = normalize(viewDir);

                float2 pixelCoords = IN.texcoord * _BlitTexture_TexelSize.zw; 
                
                float disLimit = min(viewLenght, maxDistance);
                float disTravelled = InterleavedGradientNoise(pixelCoords, (int)(_Time.y / max(HALF_EPS, unity_DeltaTime.x)) * noiseOffset);
                float transmittance = 1;

                while(disTravelled < disLimit)
                {
                    float density = get_density();

                    if (density > 0)
                    {
                        transmittance *= exp(-density * stepSize);
                    }

                    disTravelled += stepSize;
                }
                
                return lerp (col, fogColor, 1.0 - saturate(transmittance));
            }
            ENDHLSL
        }
    }
}
