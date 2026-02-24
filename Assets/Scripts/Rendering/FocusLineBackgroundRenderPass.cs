using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Rendering
{
    public class FocusLineBackgroundRenderPass : ScriptableRenderPass
    {
        private static readonly int RotationId = Shader.PropertyToID("_Rotation");

        private class PassData
        {
            public Material Material;
        }

        private readonly FocusLinesBackgroundRendererFeature.Context _context;

        public FocusLineBackgroundRenderPass(FocusLinesBackgroundRendererFeature.Context context)
        {
            _context = context;
            renderPassEvent = RenderPassEvent.BeforeRenderingPrePasses;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            var colorTarget = resourceData.activeColorTexture;

            if (!colorTarget.IsValid()) return;

            // マテリアルプロパティをメインスレッドで設定
            _context.Material.SetFloat(RotationId, _context.Rotation);

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Focus Line Background Pass", out var passData))
            {
                passData.Material = _context.Material;

                builder.SetRenderAttachment(colorTarget, 0);
                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, new Vector4(1, 1, 0, 0), data.Material, 0);
                });
            }
        }
    }
}