using InGame.System;
using Rendering;
using UnityEngine;

namespace InGame.Event
{
    public class CombineCutSceneController : MonoBehaviour
    {
        [SerializeField] private Material _outlineEdgeMaterial;
        [SerializeField] private Renderer _bodyRenderer;
        [SerializeField] private Renderer _leftArmRenderer;
        [SerializeField] private Renderer _rightArmRenderer;
        [SerializeField] private Renderer _footPartsRenderer;
        
        [SerializeField] private Renderer _beforeCombineBodyRenderer;
        [SerializeField] private Renderer _beforeCombineLeftPartRenderer;
        [SerializeField] private Renderer _beforeCombineRightPartRenderer;
        [SerializeField] private Renderer _beforeCombineFootRenderer;
        
        [Range(0f, 2f)] public float BackgroundRotation;
        [Range(0f, 1f)] public float ChangeThreshold;
        [Range(0f, 1f)] public float LineSpeed;
        [Range(0f, 1f)] public float EdgeEmissionIntensity;
        [Range(0f, 1f)] public float EyeEmissionIntensity;
        private Material _headMaterial;

        private void Update()
        {
            FocusLinesBackgroundRendererFeature.Instance?.SetRotation(BackgroundRotation);
            FocusLinesBackgroundRendererFeature.Instance?.SetThresholdAndSpeed(ChangeThreshold, LineSpeed);
            
            _outlineEdgeMaterial.SetFloat("_EdgeEmissionIntensity", EdgeEmissionIntensity);
            if (_headMaterial == null) _headMaterial = _bodyRenderer.materials[1];
            _headMaterial.SetFloat("_EyeEmissionIntensity", EyeEmissionIntensity);
        }

        public void SetTexture(ITextureContext context)
        {
            // 合体前
            var _material1 = _beforeCombineBodyRenderer.materials[1];
            var _material2 = _beforeCombineBodyRenderer.materials[0];
            _material1.SetTexture("_MainTexture1", context.Texture1);
            _material1.EnableKeyword("_MAIN_TEXTURE1_ON");
            _material2.SetTexture("_MainTexture2", context.Texture2);
            _material2.EnableKeyword("_MAIN_TEXTURE2_ON");
            _beforeCombineLeftPartRenderer.material.SetTexture("_MainTexture1", context.Texture1);
            _beforeCombineRightPartRenderer.material.SetTexture("_MainTexture1", context.Texture1);
                
            _material1.SetTexture("_LeftHandTexture", context.LeftArmTexture);
            _material1.EnableKeyword("_LEFT_HAND_TEXTURE_ON");
            _material1.SetTexture("_RightHandTexture", context.RightArmTexture);
            _material1.EnableKeyword("_RIGHT_HAND_TEXTURE_ON");
            
            _beforeCombineFootRenderer.material.SetTexture("_MainTexture2", context.FootPartsTexture);
            
            _material1 = _bodyRenderer.materials[1];
            _material2 = _bodyRenderer.materials[0];
            // 合体後
            _material1.SetTexture("_MainTexture1", context.Texture1);
            _material1.EnableKeyword("_MAIN_TEXTURE1_ON");
            _material2.SetTexture("_MainTexture2", context.Texture2);
            _material2.EnableKeyword("_MAIN_TEXTURE2_ON");
            
            _material1.SetTexture("_LeftHandTexture", context.LeftArmTexture);
            _material1.EnableKeyword("_LEFT_HAND_TEXTURE_ON");
            //_material1.SetTexture("_LeftHandMaskTexture", _maskTextures.LeftHandMaskTexture);
            _material1.EnableKeyword("_LEFT_HAND_MASK_TEXTURE_ON");
            _material1.SetTexture("_RightHandTexture", context.RightArmTexture);
            _material1.EnableKeyword("_RIGHT_HAND_TEXTURE_ON");
            //_material1.SetTexture("_RightHandMaskTexture", _maskTextures.RightHandMaskTexture);
            _material1.EnableKeyword("_RIGHT_HAND_MASK_TEXTURE_ON");
            _material2.SetTexture("_FootTexture", context.FootPartsTexture);
            _material2.EnableKeyword("_FOOT_TEXTURE_ON");
            //_material2.SetTexture("_FootMaskTexture", _maskTextures.FootMaskTexture);
            _material2.EnableKeyword("_FOOT_MASK_TEXTURE_ON");
            
            _leftArmRenderer.material.SetTexture("_MainTexture1", context.Texture1);
            _leftArmRenderer.material.EnableKeyword("_MAIN_TEXTURE1_ON");
            _rightArmRenderer.material.SetTexture("_MainTexture1", context.Texture1);
            _rightArmRenderer.material.EnableKeyword("_MAIN_TEXTURE1_ON");
            _footPartsRenderer.material.SetTexture("_MainTexture2", context.Texture2);
            _footPartsRenderer.material.EnableKeyword("_MAIN_TEXTURE2_ON");
        }
    }
}
