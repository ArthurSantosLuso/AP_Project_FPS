Shader "Custom/FogShader"
{
    Properties
    {
        fogColor ("Color", Color) = (1, 1, 1, 1)
        [HDR] lightContribution ("Light contribution", Color) = (1, 1, 1, 1)

        maxDistance("Max distance", float) = 100
        stepSize ("Step size", Range(0.1, 20)) = 1
        densityMult ("Density multiplier", Range(0, 10)) = 1
        
        [NoScaleOffset] fogNoise ("3D Fog noise", 3D) = "white"{}
        densityThreshold ("Density treshold", Range(0, 1)) = 0.1
        noiseTiling ("Noise tiling", Range(0, 10)) = 1
        
        blueNoise ("2D Blue noise", 2D) = "gray"{}

        fogWind ("Fog wind", Vector) = (1, 0, 0, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            float4 fogColor;
            float4 lightContribution;
            float maxDistance;
            float stepSize;
            float densityMult;
            TEXTURE3D (fogNoise);
            float densityThreshold;
            float noiseTiling;
            TEXTURE2D (blueNoise);
            float4 blueNoise_TexelSize;
            float4 fogWind;

            float get_density(float3 worldPos)
            {
                float3 wind = (worldPos - fogWind.xyz * fogWind.w * _Time.y) * 0.01 * noiseTiling;
                float4 noise = fogNoise.SampleLevel(sampler_TrilinearRepeat, wind, 0);
                float cloudShape = noise.r;
                float cloudDetail = dot(noise.gba, float3(0.625, 0.25, 0.125));
                
                float density = saturate(cloudShape - densityThreshold);
                density = saturate(density - (1.0 - cloudDetail) * 0.3) * densityMult;
                
                return density;
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
                uint2 noiseSize = (uint2)blueNoise_TexelSize.zw;
                uint2 noiseCoords = (uint2)pixelCoords % noiseSize;
                float bNoise = LOAD_TEXTURE2D(blueNoise, noiseCoords).r;
                
                float disLimit = min(viewLenght, maxDistance);
                float disTravelled = bNoise * stepSize;
                
                float transmittance = 1;
                float4 fogColorHDR = fogColor;

                while(disTravelled < disLimit)
                {
                    float3 rayPos = startPoint + rayDir * disTravelled;
                    float density = get_density(rayPos);

                    if (density > 0)
                    {
                        Light mainLight = GetMainLight(TransformWorldToShadowCoord(rayPos));
                        fogColorHDR.rgb += mainLight.color.rgb * lightContribution.rgb * density * mainLight.shadowAttenuation * stepSize;
                        transmittance *= exp(-density * stepSize);
                    }

                    disTravelled += stepSize;
                }
                
                return lerp (col, fogColorHDR, 1.0 - saturate(transmittance));
            }

            ENDHLSL
        }
    }
}
