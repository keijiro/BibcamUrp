using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Bibcam.Decoder {

sealed class BibcamBackgroundRenderPass : ScriptableRenderPass
{
    public override void Execute
      (ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // Play mode is not yet supported.
        if (!UnityEngine.Application.isPlaying) return;

        // BibcamBackground component reference
        var camera = renderingData.cameraData.camera;
        var bg = camera.GetComponent<BibcamBackground>();
        if (bg == null) return;

        // Command buffer
        var cmd = CommandBufferPool.Get();
        bg.PushDrawCommand(cmd);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}

public sealed class BibcamBackgroundPassFeature : ScriptableRendererFeature
{
    BibcamBackgroundRenderPass _pass;

    public override void Create()
      => _pass = new BibcamBackgroundRenderPass
           { renderPassEvent = RenderPassEvent.AfterRenderingOpaques };

    public override void AddRenderPasses
      (ScriptableRenderer renderer, ref RenderingData renderingData)
      => renderer.EnqueuePass(_pass);
}

} // namespace Bibcam.Decoder
