Shader "ShadowRobot2/Character-Player"
{
    Properties
    {
        _MainTexture1("Main Texture 1", 2D) = "white" {}
        _MainTexture2("Main Texture 2", 2D) = "white" {}
        _LeftHandTexture("Left Hand Texture", 2D) = "white" {}
        _LeftHandMaskTexture("Left Hand Mask Texture(左手以外を黒で覆うようなマスクテクスチャ)", 2D) = "white" {}
        _RightHandTexture("Right Hand Texture", 2D) = "white" {}
        _RightHandMaskTexture("Right Hand Mask Texture(右手以外を黒で覆うようなマスクテクスチャ)", 2D) = "white" {}
        _FootTexture("Foot Texture", 2D) = "white" {}
        _FootMaskTexture("Foot Mask Texture(足以外を黒で覆うようなマスクテクスチャ)", 2D) = "white" {}
        _OutlineWidth("Outline Width", Float) = 2.0
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "Forward Lit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #pragma shader_feature_local _MAIN_TEXTURE1_ON
            #pragma shader_feature_local _MAIN_TEXTURE2_ON
            #pragma shader_feature_local _LEFT_HAND_TEXTURE_ON
            #pragma shader_feature_local _LEFT_HAND_MASK_TEXTURE_ON
            #pragma shader_feature_local _RIGHT_HAND_TEXTURE_ON
            #pragma shader_feature_local _RIGHT_HAND_MASK_TEXTURE_ON
            #pragma shader_feature_local _FOOT_TEXTURE_ON
            #pragma shader_feature_local _FOOT_MASK_TEXTURE_ON

            #include "PlayerForwardPass.hlsl"
            ENDHLSL
        }

//        Pass
//        {
//            Name "Forward Lit(Outline)"
//            Tags
//            {
//                "LightMode" = "SRPDefaultUnlit"
//            }
//
//            Cull Front
//            ZWrite On
//            ZTest LEqual
//
//            HLSLPROGRAM
//
//            #pragma vertex Vert
//            #pragma fragment Frag
//
//            #include "PlayerForwardOutlinePass.hlsl"
//
//            ENDHLSL
//        }

        Pass
        {
            Name "Depth Only"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            ZWrite On
            ZTest LEqual
            ColorMask 0

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "CharacterDepthOnlyPass.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "Shadow Caster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
            
            ZWrite On
            ZTest LEqual
            
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "CharacterShadowCasterPass.hlsl"
            
            ENDHLSL
        }
    }

    CustomEditor "CharacterPlayerShaderGUI"
}
