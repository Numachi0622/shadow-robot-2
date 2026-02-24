#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "PlayerProperties.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
    float2 uv : TEXCOORD0;
};

struct Varyings
{
    float4 positionHCS : SV_POSITION;
    float3 positionWS : TEXCOORD0;
    float3 normalWS : TEXCOORD1;
    float2 uv : TEXCOORD2;
};

Varyings Vert(Attributes input)
{
    Varyings output;
    output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
    output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
    output.normalWS = TransformObjectToWorldNormal(input.positionOS.xyz);
    output.uv = input.uv;
    return output;
}

half4 Frag(Varyings input) : SV_Target
{
    // テクスチャの決定
    half4 baseColor = 1;
    #if defined(_MAIN_TEXTURE1_ON)
                baseColor = SAMPLE_TEXTURE2D(_MainTexture1, sampler_MainTexture1, input.uv);
    #elif defined(_MAIN_TEXTURE2_ON)
                baseColor = SAMPLE_TEXTURE2D(_MainTexture2, sampler_MainTexture2, input.uv);
    #endif

    #if defined(_LEFT_HAND_TEXTURE_ON) && defined(_LEFT_HAND_MASK_TEXTURE_ON)
                half4 leftHandColor = SAMPLE_TEXTURE2D(_LeftHandTexture, sampler_LeftHandTexture, input.uv);
                uint leftHandMask = SAMPLE_TEXTURE2D(_LeftHandMaskTexture, sampler_LeftHandMaskTexture, input.uv).r;
                baseColor = lerp(baseColor, leftHandColor, leftHandMask);
    #endif

    #if defined(_RIGHT_HAND_TEXTURE_ON) && defined(_RIGHT_HAND_MASK_TEXTURE_ON)
                half4 rightHandColor = SAMPLE_TEXTURE2D(_RightHandTexture, sampler_RightHandTexture, input.uv);
                uint rightHandMask = SAMPLE_TEXTURE2D(_RightHandMaskTexture, sampler_RightHandMaskTexture, input.uv).r;
                baseColor = lerp(baseColor, rightHandColor, rightHandMask);
    #endif

    #if defined(_FOOT_TEXTURE_ON) && defined(_FOOT_MASK_TEXTURE_ON)
                half4 footColor = SAMPLE_TEXTURE2D(_FootTexture, sampler_FootTexture, input.uv);
                uint footMask = SAMPLE_TEXTURE2D(_FootMaskTexture, sampler_FootMaskTexture, input.uv).r;
                baseColor = lerp(baseColor, footColor, footMask);
    #endif

    // InputData
    InputData inputData = (InputData)0;
    inputData.positionWS = input.positionWS;
    inputData.normalWS = input.normalWS;
    inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
    inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
    inputData.bakedGI = SampleSH(inputData.normalWS);
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionHCS);

    // SurfaceData
    SurfaceData surfaceData = (SurfaceData)0;
    surfaceData.albedo = baseColor.rgb;
    surfaceData.metallic = 0;
    surfaceData.smoothness = 0;
    surfaceData.emission = 0;
    surfaceData.occlusion = 1;
    surfaceData.alpha = baseColor.a;

    return UniversalFragmentPBR(inputData, surfaceData);
}
