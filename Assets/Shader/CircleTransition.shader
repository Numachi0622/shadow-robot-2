Shader "URP/UI/CircleMask"
{
    Properties
    {
        _Color("Color", Color) = (0,0,0,1)
        _Radius("Radius", Range(0,1)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Name "CircleMaskPass"
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float _Radius;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float aspect = (_ScreenParams.x / _ScreenParams.y);
                float2 center = float2(0.5, 0.5);
                float2 delta = float2((i.uv.x - center.x) * aspect, i.uv.y - center.y);
                float dist = length(delta);
                float maxRadius = length(float2(0.5 * aspect, 0.5));
                float scaledRadius = _Radius * maxRadius;
                if (dist > scaledRadius)
                    return _Color;
                else
                    return half4(0, 0, 0, 0); // 中央は透明
            }
            ENDHLSL
        }
    }
}
