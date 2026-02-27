using InGame.System;
using Rendering;
using UnityEngine;
using Utility;

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
            var material1 = _beforeCombineBodyRenderer.materials[1];
            var material2 = _beforeCombineBodyRenderer.materials[0];
            material1.SetTexture(GameConst.ShaderMainTexture1, context.Texture1);
            material1.EnableKeyword(GameConst.ShaderKeywordMainTexture1On);
            material2.SetTexture(GameConst.ShaderMainTexture2, context.Texture2);
            material2.EnableKeyword(GameConst.ShaderKeywordMainTexture2On);
            material1.SetTexture(GameConst.ShaderLeftHandTexture, context.LeftArmTexture);
            material1.EnableKeyword(GameConst.ShaderKeywordLeftHandTextureOn);
            material1.SetTexture(GameConst.ShaderRightHandTexture, context.RightArmTexture);
            material1.EnableKeyword(GameConst.ShaderKeywordRightHandTextureOn);
            _beforeCombineLeftPartRenderer.material.SetTexture(GameConst.ShaderMainTexture1, context.Texture1);
            _beforeCombineRightPartRenderer.material.SetTexture(GameConst.ShaderMainTexture1, context.Texture1);
            _beforeCombineFootRenderer.material.SetTexture(GameConst.ShaderMainTexture2, context.FootPartsTexture);

            // 合体後
            material1 = _bodyRenderer.materials[1];
            material2 = _bodyRenderer.materials[0];
            material1.SetTexture(GameConst.ShaderMainTexture1, context.Texture1);
            material1.EnableKeyword(GameConst.ShaderKeywordMainTexture1On);
            material2.SetTexture(GameConst.ShaderMainTexture2, context.Texture2);
            material2.EnableKeyword(GameConst.ShaderKeywordMainTexture2On);
            material1.SetTexture(GameConst.ShaderLeftHandTexture, context.LeftArmTexture);
            material1.EnableKeyword(GameConst.ShaderKeywordLeftHandTextureOn);
            material1.SetTexture(GameConst.ShaderRightHandTexture, context.RightArmTexture);
            material1.EnableKeyword(GameConst.ShaderKeywordRightHandTextureOn);
            material2.SetTexture(GameConst.ShaderFootTexture, context.FootPartsTexture);
            material2.EnableKeyword(GameConst.ShaderKeywordFootTextureOn);
            _leftArmRenderer.material.SetTexture(GameConst.ShaderMainTexture1, context.Texture1);
            _leftArmRenderer.material.EnableKeyword(GameConst.ShaderKeywordMainTexture1On);
            _rightArmRenderer.material.SetTexture(GameConst.ShaderMainTexture1, context.Texture1);
            _rightArmRenderer.material.EnableKeyword(GameConst.ShaderKeywordMainTexture1On);
            _footPartsRenderer.material.SetTexture(GameConst.ShaderMainTexture2, context.Texture2);
            _footPartsRenderer.material.EnableKeyword(GameConst.ShaderKeywordMainTexture2On);
        }

        private void OnDestroy()
        {
            FocusLinesBackgroundRendererFeature.Instance?.SetRotation(0f);
            FocusLinesBackgroundRendererFeature.Instance?.SetThresholdAndSpeed(0f, 1f);
        }
    }
}
