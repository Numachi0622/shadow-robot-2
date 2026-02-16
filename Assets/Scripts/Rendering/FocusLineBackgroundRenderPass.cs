using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Rendering
{
    public class FocusLineBackgroundRenderPass : ScriptableRenderPass
    {
        private class PassData
        {
            public Material Material;
            public float Intensity;
        }
        
        private FocusLinesBackgroundRendererFeature.Context _context;
        
        public FocusLineBackgroundRenderPass(FocusLinesBackgroundRendererFeature.Context context)
        {
            _context = context;
            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            var colorTarget = resourceData.activeColorTexture;

            if (!_context.Material || !colorTarget.IsValid())
            {
                Debug.LogError("[FocusLineBackgroundRenderPass] Material or color target is not set.");
                return;
            }

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Focus Line Background Pass", out var passData))
            {
                passData.Material = _context.Material;
                passData.Intensity = _context.LineIntensity;

                builder.SetRenderAttachment(colorTarget, 0);
                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, new Vector4(1, 1, 0, 0), data.Material, 0);
                });
            }
        }
    }
}