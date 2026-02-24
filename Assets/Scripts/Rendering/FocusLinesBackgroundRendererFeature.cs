using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Rendering
{
    public class FocusLinesBackgroundRendererFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Context
        {
            public Material Material;
            [FormerlySerializedAs("RotateValue")] public float Rotation = 0f;
        }

        [SerializeField] private Context _context;

        private const float MaxRotateRate = 2f;
        
        public static FocusLinesBackgroundRendererFeature Instance { get; private set; }
        
        public override void Create()
        {
            Instance = this;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!_context.Material) return;

            var cameraData = renderingData.cameraData;
            if (cameraData.isSceneViewCamera || cameraData.isPreviewCamera) return;

            renderer.EnqueuePass(new FocusLineBackgroundRenderPass(_context));
        }

        public void SetRotation(float value)
        {
            if (_context == null)
            {
                Debug.LogError("[FocusLinesBackgroundRendererFeature] Context is null");
                return;
            }
            _context.Rotation = Mathf.Clamp(value, 0f, Mathf.PI * MaxRotateRate);
        }
    }
}