using Rendering;
using UnityEngine;

namespace InGame.Event
{
    public class CombineCutSceneController : MonoBehaviour
    {
        [SerializeField] private Material _outlineEdgeMaterial;
        [SerializeField] private Renderer _headRenderer;
        
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
            if (_headMaterial == null) _headMaterial = _headRenderer.materials[1];
            _headMaterial.SetFloat("_EyeEmissionIntensity", EyeEmissionIntensity);
        }
    }
}
