Shader "Custom/CloudNoise"
{
    Properties
    {
        _MainTex("Cloud Mask", 2D) = "white" {}
        _NoiseScale("Noise Scale", Float) = 1.0
        _NoiseSpeed("Noise Speed", Float) = 0.5
        _CloudDensity("Cloud Density", Range(0,1)) = 0.7
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Pass
        {
            Name "CloudNoise"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _NoiseScale;
                float _NoiseSpeed;
                float _CloudDensity;
            CBUFFER_END

            // Simple noise function
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                
                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));
                
                float2 u = f * f * (3.0 - 2.0 * f);
                
                return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }

            // Fractal noise with multiple octaves
            float fbm(float2 p)
            {
                float value = 0.0;
                float amplitude = 0.5;
                float frequency = 1.0;
                
                // Add 4 octaves of noise
                for (int i = 0; i < 4; i++)
                {
                    value += amplitude * noise(p * frequency);
                    amplitude *= 0.5;
                    frequency *= 2.0;
                }
                
                return value;
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionHCS = positionInputs.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.worldPos = positionInputs.positionWS;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Sample the cloud mask
                half4 mask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                if (mask.r < 0.01)
                {
                    // No cloud here, return transparent
                    return half4(0, 0, 0, 0);
                }
                
                // Generate noise based on world position for spatial coherence
                float2 noiseUV = input.worldPos.xz * _NoiseScale * 0.01;
                
                // Animate the noise using Unity's built-in _Time
                float time = _Time.y * _NoiseSpeed;
                float2 animatedUV1 = noiseUV + float2(time * 0.1, time * 0.05);
                float2 animatedUV2 = noiseUV * 2.0 + float2(time * -0.08, time * 0.12);
                
                // Generate fractal noise
                float noise1 = fbm(animatedUV1);
                float noise2 = fbm(animatedUV2) * 0.5;
                
                // Combine noise layers
                float finalNoise = (noise1 + noise2) * 0.75;
                
                // Apply cloud density threshold
                finalNoise = smoothstep(1.0 - _CloudDensity, 1.0, finalNoise);
                
                // Combine with original mask
                float cloudValue = mask.r * finalNoise;
                
                // Create cloud color (white with some variation)
                float3 cloudColor = float3(0.95, 0.97, 1.0) * (0.8 + finalNoise * 0.4);
                
                return half4(cloudColor, cloudValue);
            }

            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}