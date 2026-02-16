using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Rendering
{
    public class FocusLinesBackgroundRendererFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Context
        {
            public Material Material;
            [Range(0, 10)] public float LineIntensity = 1f;
        }

        [SerializeField] private Context _context;
        
        public override void Create()
        {
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!_context.Material) return;

            var pass = new FocusLineBackgroundRenderPass(_context);
            renderer.EnqueuePass(pass);
        }
    }
}