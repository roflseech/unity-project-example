Shader "Custom/CloudComposite"
{
    Properties
    {
        _CloudMaskTexture("Cloud Mask", 2D) = "white" {}
        _CloudColor("Cloud Color", Color) = (1, 1, 1, 1)
        _EdgeColor("Edge Color", Color) = (1, 0.8, 0.6, 1)
        _EdgeIntensity("Edge Intensity", Float) = 2.0
        _EdgeWidth("Edge Width", Float) = 3.0
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue"="Transparent"
        }
        LOD 100

        Pass
        {
            Name "CloudComposite"
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest Always
            
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
            };

            TEXTURE2D(_CloudMaskTexture);
            SAMPLER(sampler_CloudMaskTexture);

            CBUFFER_START(UnityPerMaterial)
                float4 _CloudMaskTexture_ST;
                float4 _CloudMaskTexture_TexelSize;
                float4 _CloudColor;
                float4 _EdgeColor;
                float _EdgeIntensity;
                float _EdgeWidth;
            CBUFFER_END

            // Edge detection function using Sobel operator on cloud mask
            float detectEdges(float2 uv)
            {
                float2 texelSize = _CloudMaskTexture_TexelSize.xy * _EdgeWidth;
                
                // Sample cloud mask (white = cloud, black = empty)
                float center = SAMPLE_TEXTURE2D(_CloudMaskTexture, sampler_CloudMaskTexture, uv).r;
                
                // Sample 8 neighbors for edge detection
                float left   = SAMPLE_TEXTURE2D(_CloudMaskTexture, sampler_CloudMaskTexture, uv + float2(-texelSize.x, 0)).r;
                float right  = SAMPLE_TEXTURE2D(_CloudMaskTexture, sampler_CloudMaskTexture, uv + float2(texelSize.x, 0)).r;
                float up     = SAMPLE_TEXTURE2D(_CloudMaskTexture, sampler_CloudMaskTexture, uv + float2(0, texelSize.y)).r;
                float down   = SAMPLE_TEXTURE2D(_CloudMaskTexture, sampler_CloudMaskTexture, uv + float2(0, -texelSize.y)).r;
                
                float upLeft    = SAMPLE_TEXTURE2D(_CloudMaskTexture, sampler_CloudMaskTexture, uv + float2(-texelSize.x, texelSize.y)).r;
                float upRight   = SAMPLE_TEXTURE2D(_CloudMaskTexture, sampler_CloudMaskTexture, uv + float2(texelSize.x, texelSize.y)).r;
                float downLeft  = SAMPLE_TEXTURE2D(_CloudMaskTexture, sampler_CloudMaskTexture, uv + float2(-texelSize.x, -texelSize.y)).r;
                float downRight = SAMPLE_TEXTURE2D(_CloudMaskTexture, sampler_CloudMaskTexture, uv + float2(texelSize.x, -texelSize.y)).r;
                
                // Sobel edge detection
                float sobelX = (-1.0 * upLeft + 1.0 * upRight) +
                              (-2.0 * left + 2.0 * right) +
                              (-1.0 * downLeft + 1.0 * downRight);
                              
                float sobelY = (-1.0 * upLeft - 2.0 * up - 1.0 * upRight) +
                              (1.0 * downLeft + 2.0 * down + 1.0 * downRight);
                
                float edge = sqrt(sobelX * sobelX + sobelY * sobelY);
                
                // Only show edges where there's cloud content
                edge *= step(0.1, center);
                
                return saturate(edge * _EdgeIntensity);
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _CloudMaskTexture);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Sample the cloud mask (white = cloud, black = empty)
                float cloudMask = SAMPLE_TEXTURE2D(_CloudMaskTexture, sampler_CloudMaskTexture, input.uv).r;
                
                // Detect edges in the mask
                float edge = detectEdges(input.uv);
                
                // Create base cloud color
                half3 baseCloudColor = _CloudColor.rgb * cloudMask;
                
                // Create edge glow
                half3 edgeGlow = _EdgeColor.rgb * edge;
                
                // Combine base cloud with edge glow
                half3 finalColor = baseCloudColor + edgeGlow;
                
                // Calculate final alpha (cloud presence + edge presence)
                float finalAlpha = saturate(cloudMask + edge);
                
                return half4(finalColor, finalAlpha);
            }

            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}