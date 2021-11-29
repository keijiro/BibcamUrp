using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Bibcam.Common;
using Bibcam.Decoder;

namespace Bibcam {

sealed class BackgroundRenderPass : ScriptableRenderPass
{
    public BibcamMetadataDecoder Decoder { get; set; }
    public BibcamTextureDemuxer Demuxer { get; set; }
    public Material Material { get; set; }

    public override void Execute
      (ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (Decoder == null || Demuxer == null) return;
        if (Demuxer.ColorTexture == null) return;

        // Camera parameters
        var meta = Decoder.Metadata;
        var ray = BibcamRenderUtils.RayParams(meta);
        var iview = BibcamRenderUtils.InverseView(meta);

        // Material property update
        Material.SetVector(ShaderID.RayParams, ray);
        Material.SetMatrix(ShaderID.InverseView, iview);
        Material.SetVector(ShaderID.DepthRange, meta.DepthRange);
        Material.SetTexture(ShaderID.ColorTexture, Demuxer.ColorTexture);
        Material.SetTexture(ShaderID.DepthTexture, Demuxer.DepthTexture);

        // Draw call
        var cmd = CommandBufferPool.Get();
        CoreUtils.DrawFullScreen(cmd, Material);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}

public sealed class BibcamBackgroundPassFeature : ScriptableRendererFeature
{
    #region Public properties/methods

    public void SetSource
      (BibcamMetadataDecoder decoder, BibcamTextureDemuxer demuxer)
      => (_pass.Decoder, _pass.Demuxer) = (decoder, demuxer);

    public float DepthOffset
      { set => Material.SetFloat(ShaderID.DepthOffset, value); }

    public Color DepthColor
      { set => Material.SetColor(ShaderID.DepthColor, value); }

    public Color StencilColor
      { set => Material.SetColor(ShaderID.StencilColor, value); }

    #endregion

    #region Project asset references

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Private members

    Material _material;
    BackgroundRenderPass _pass;

    Material Material
      => _material != null ? _material :
           _material = CoreUtils.CreateEngineMaterial(_shader);

    #endregion

    #region ScriptableRendererFeature overrides

    public override void Create()
    {
        _pass = new BackgroundRenderPass { Material = this.Material };
        _pass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public override void AddRenderPasses
      (ScriptableRenderer renderer, ref RenderingData renderingData)
      => renderer.EnqueuePass(_pass);

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(_material);
        _material = null;
    }

    #endregion
}

} // namespace Bibcam
