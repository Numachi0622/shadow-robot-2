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
            public float Rotation = 0f;
            public float ChangeThreshold = 0f;
            public float LineSpeed = 0f;
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

        public void SetThresholdAndSpeed(float threshold, float speed)
        {
            if (_context == null)
            {
                Debug.LogError("[FocusLinesBackgroundRendererFeature] Context is null");
                return;
            }

            _context.ChangeThreshold = Mathf.Clamp01(threshold);
            _context.LineSpeed = Mathf.Clamp01(speed);
        }
    }
}