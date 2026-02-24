#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "PlayerProperties.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
};

struct Varyings
{
    float4 positionHCS : SV_POSITION;
};

Varyings Vert(Attributes input)
{
    Varyings output;

    // ワールド空間で法線方向に頂点を押し出してアウトラインを形成
    float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    positionWS += normalWS * _OutlineWidth * 0.001;

    output.positionHCS = TransformWorldToHClip(positionWS);
    return output;
}

half4 Frag(Varyings input) : SV_Target
{
    return _OutlineColor;
}
