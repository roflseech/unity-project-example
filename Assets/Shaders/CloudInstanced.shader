Shader "Custom/CloudInstanced"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,0.5)
        _EmissionColor("Emission Color", Color) = (0.8,0.9,1,1)
        _EmissionStrength("Emission Strength", Range(0,2)) = 0.3
        _Smoothness("Smoothness", Range(0,1)) = 0.8
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue"="Transparent"
        }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:setup
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : NORMAL;
                float3 positionWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _EmissionColor;
                float _EmissionStrength;
                float _Smoothness;
            CBUFFER_END

            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                StructuredBuffer<float4x4> _TransformBuffer;
            #endif

            void ConfigureProcedural()
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
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                    ConfigureProcedural();
                #endif

                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);

                output.positionHCS = positionInputs.positionCS;
                output.normalWS = normalInputs.normalWS;
                output.positionWS = positionInputs.positionWS;
                output.uv = input.uv;
                output.viewDir = normalize(_WorldSpaceCameraPos - positionInputs.positionWS);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Create soft, cloud-like appearance
                float2 centerUV = input.uv - 0.5;
                float distanceFromCenter = length(centerUV);
                
                // Create soft edge falloff
                float softEdge = 1.0 - smoothstep(0.3, 0.5, distanceFromCenter);
                
                // Add some noise-like variation
                float noise = sin(input.uv.x * 20.0) * sin(input.uv.y * 20.0) * 0.1 + 0.9;
                softEdge *= noise;
                
                // Basic lighting
                Light mainLight = GetMainLight();
                float3 lightDirection = normalize(mainLight.direction);
                float NdotL = saturate(dot(input.normalWS, lightDirection));
                
                // Rim lighting for cloud effect
                float rim = 1.0 - saturate(dot(input.normalWS, input.viewDir));
                rim = pow(rim, 2.0);
                
                float3 ambient = SampleSH(input.normalWS);
                float3 diffuse = mainLight.color * NdotL * 0.5;
                float3 emission = _EmissionColor.rgb * _EmissionStrength * rim;
                
                float3 finalColor = _Color.rgb * (ambient + diffuse) + emission;
                
                // Apply soft edge and alpha
                float alpha = _Color.a * softEdge;
                
                return half4(finalColor, alpha);
            }

            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}