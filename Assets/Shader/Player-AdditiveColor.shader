Shader "URP/SurfaceShader-Player-AdditiveColor_Shadow"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MetallicMap ("MetallicMap", 2D) = "white" {}
        _NormalMap ("NormalMap", 2D) = "white" {}
        _OcclusionMap ("OcclusionMap", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        LOD 200
        Pass
        {
                Name "ForwardLit"
                Tags { "LightMode"="UniversalForward" }

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma target 3.0

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

                struct Attributes
                {
                    float4 positionOS : POSITION;
                    float3 normalOS   : NORMAL;
                    float2 uv         : TEXCOORD0;
                };

                struct Varyings
                {
                    float2 uv         : TEXCOORD0;
                    float3 normalWS   : TEXCOORD1;
                    float4 positionHCS : SV_POSITION;
                    float3 viewDirWS   : TEXCOORD2;
                    float4 shadowCoord : TEXCOORD3;
                };

                TEXTURE2D(_MainTex);    SAMPLER(sampler_MainTex);
                TEXTURE2D(_MetallicMap);    SAMPLER(sampler_MetallicMap);
                TEXTURE2D(_NormalMap);      SAMPLER(sampler_NormalMap);
                TEXTURE2D(_OcclusionMap);   SAMPLER(sampler_OcclusionMap);

                float4 _Color;
                float _Glossiness;
                float _Metallic;
                float _Threshold;

                Varyings vert(Attributes IN)
                {
                    Varyings OUT;
                    OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                    OUT.uv = IN.uv;
                    OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                    float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                    OUT.viewDirWS = normalize(_WorldSpaceCameraPos - worldPos);
                    OUT.shadowCoord = TransformWorldToShadowCoord(worldPos);
                    return OUT;
                }

                float3 SimplePBR(float3 albedo, float metallic, float smoothness, float3 normal, float3 viewDir, float shadow)
                {
                    Light mainLight = GetMainLight();
                    float3 lightDir = normalize(mainLight.direction);
                    float3 halfVector = normalize(lightDir + viewDir);

                    float NdotL = max(dot(normal, lightDir), 0.0);
                    float3 diffuse = albedo * NdotL * mainLight.color * shadow;

                    float3 F0 = lerp(float3(0.04, 0.04, 0.04), albedo, metallic);
                    float3 fresnel = F0 + (1.0 - F0) * pow(1.0 - max(dot(halfVector, viewDir), 0.0), 5.0);

                    float NdotH = max(dot(normal, halfVector), 0.0);
                    float roughness = 1.0 - smoothness;
                    float alpha = roughness * roughness;
                    float alpha2 = alpha * alpha;
                    float denom = NdotH * NdotH * (alpha2 - 1.0) + 1.0;
                    float D = alpha2 / (PI * denom * denom);

                    float3 specular = D * fresnel * shadow;

                    return diffuse + specular;
                }

                half4 frag(Varyings IN) : SV_Target
                {
                    float3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb + _Color.rgb;

                    float3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.uv));
                    float3 normalWS = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, normalTS));

                    float metallic = SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, IN.uv).r;
                    float occulusion = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, IN.uv).r;
                    float smoothness = _Glossiness;

                    float shadow = MainLightRealtimeShadow(IN.shadowCoord);

                    float3 finalColor = SimplePBR(albedo, metallic, smoothness, normalWS, IN.viewDirWS, shadow * occulusion);
                    
                    return half4(finalColor, 1.0);
                }
                ENDHLSL
            }

            UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}