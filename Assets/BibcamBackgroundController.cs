using UnityEngine;
using Bibcam.Decoder;

namespace Bibcam {

public sealed class BibcamBackgroundController : MonoBehaviour
{
    #region Project asset references

    [SerializeField] BibcamBackgroundPassFeature _target = null;

    #endregion

    #region Scene object references

    [SerializeField] BibcamMetadataDecoder _decoder = null;
    [SerializeField] BibcamTextureDemuxer _demux = null;

    #endregion

    #region Editable attributes

    [SerializeField] float _depthOffset = 0;
    [SerializeField] Color _depthColor = Color.white;
    [SerializeField] Color _stencilColor = Color.red;

    #endregion

    #region

    void Update()
    {
        _target.SetSource(_decoder, _demux);
        _target.DepthOffset = _depthOffset;
        _target.DepthColor = _depthColor;
        _target.StencilColor = _stencilColor;
    }

    #endregion

}

} // namespace Bibcam
