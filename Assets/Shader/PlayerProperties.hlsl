CBUFFER_START(UnityPerMaterial)
half4 _EyeEmissionColor;
float _EyeEmissionIntensity;
CBUFFER_END

#if defined(_MAIN_TEXTURE1_ON)
TEXTURE2D(_MainTexture1);
SAMPLER(sampler_MainTexture1);
#endif

#if defined(_MAIN_TEXTURE2_ON)
TEXTURE2D(_MainTexture2);
SAMPLER(sampler_MainTexture2);
#endif

#if defined(_LEFT_HAND_TEXTURE_ON)
TEXTURE2D(_LeftHandTexture);
SAMPLER(sampler_LeftHandTexture);
#endif

#if defined(_LEFT_HAND_MASK_TEXTURE_ON)
TEXTURE2D(_LeftHandMaskTexture);
SAMPLER(sampler_LeftHandMaskTexture);
#endif

#if defined(_RIGHT_HAND_TEXTURE_ON)
TEXTURE2D(_RightHandTexture);
SAMPLER(sampler_RightHandTexture);
#endif

#if defined(_RIGHT_HAND_MASK_TEXTURE_ON)
TEXTURE2D(_RightHandMaskTexture);
SAMPLER(sampler_RightHandMaskTexture);
#endif

#if defined(_FOOT_TEXTURE_ON)
TEXTURE2D(_FootTexture);
SAMPLER(sampler_FootTexture);
#endif

#if defined(_FOOT_MASK_TEXTURE_ON)
TEXTURE2D(_FootMaskTexture);
SAMPLER(sampler_FootMaskTexture);
#endif

TEXTURE2D(_EyeEmissionMaskTexture);
SAMPLER(sampler_EyeEmissionMaskTexture);