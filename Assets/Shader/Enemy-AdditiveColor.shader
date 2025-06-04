Shader "URP/SurfaceShader-AdditiveColor_Shadow"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _DisolveTex ("Dissolve Tex", 2D) = "white" {}
        _Threshold ("Threshold", Range(0,1)) = 0.0
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
                float2 uv           : TEXCOORD0;
                float3 normalWS     : TEXCOORD1;
                float3 viewDirWS    : TEXCOORD2;
                float4 positionHCS  : SV_POSITION;
                float4 shadowCoord  : TEXCOORD3;
            };

            TEXTURE2D(_MainTex);       SAMPLER(sampler_MainTex);
            TEXTURE2D(_DisolveTex);    SAMPLER(sampler_DisolveTex);

            float4 _Color;
            float _Glossiness;
            float _Metallic;
            float _Threshold;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(positionWS);
                OUT.uv = IN.uv;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = normalize(_WorldSpaceCameraPos - positionWS);
                OUT.shadowCoord = TransformWorldToShadowCoord(positionWS);
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
                float dissolveValue = SAMPLE_TEXTURE2D(_DisolveTex, sampler_DisolveTex, IN.uv).r;
                clip(dissolveValue - _Threshold);

                float3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb + _Color.rgb;

                float shadow = MainLightRealtimeShadow(IN.shadowCoord);

                // 正しい空間でのノーマル使用（すでにWSに変換済み）
                float3 finalColor = SimplePBR(albedo, _Metallic, _Glossiness, normalize(IN.normalWS), IN.viewDirWS, shadow);

                float edge = smoothstep(_Threshold, _Threshold + 0.1, dissolveValue);
                float3 glow = lerp(0, float3(1, 0.2, 0), (1 - edge));
                finalColor += glow;

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }

        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
