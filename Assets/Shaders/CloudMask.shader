Shader "Custom/CloudMask"
{
    Properties
    {
        _Color("Cloud Color", Color) = (1,1,1,1)
        _EdgeColor("Edge Color", Color) = (1,0.8,0.6,1)
        _EdgeWidth("Edge Width", Range(0.0, 0.5)) = 0.1
        _EdgeIntensity("Edge Intensity", Range(0.0, 5.0)) = 2.0
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
            Name "CloudMask"
            
            // Alpha blending for soft cloud overlays
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:setup

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _EdgeColor;
                float _EdgeWidth;
                float _EdgeIntensity;
            CBUFFER_END

            StructuredBuffer<float4x4> _TransformBuffer;

            void setup()
            {
            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                float4x4 data = _TransformBuffer[unity_InstanceID];
                unity_ObjectToWorld = data;
                unity_WorldToObject = unity_ObjectToWorld;
                unity_WorldToObject._14_24_34 *= -1;
                unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
            #endif
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                setup();
            #endif

                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionHCS = positionInputs.positionCS;
                output.uv = input.uv;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                
                // Create soft cloud particles with edge glow that blends properly
                float2 centerUV = input.uv - 0.5;
                float distanceFromCenter = length(centerUV);
                
                // Soft circular falloff
                float radius = 0.5;
                float softness = 0.3;
                float alpha = 1.0 - smoothstep(radius - softness, radius, distanceFromCenter);
                
                // Discard completely transparent pixels
                if (alpha <= 0.01)
                    discard;
                
                // Edge effect - stronger near particle edges but soft
                float edgeStrength = smoothstep(0.2, 0.5, distanceFromCenter) * _EdgeIntensity;
                
                // Base cloud color
                half3 baseColor = _Color.rgb;
                half3 edgeColor = _EdgeColor.rgb * edgeStrength;
                
                // Additive approach - edges add to the base color
                half3 finalColor = baseColor + edgeColor * alpha;
                
                return half4(finalColor, alpha);
            }

            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}