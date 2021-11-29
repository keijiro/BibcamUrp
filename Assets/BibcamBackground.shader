Shader "Hidden/Bibcam/BibcamBackground"
{
    HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/jp.keijiro.bibcam/Decoder/Shaders/Utils.hlsl"

sampler2D _ColorTexture;
sampler2D _DepthTexture;

float4 _RayParams;
float4x4 _InverseView;
float2 _DepthRange;
float _DepthOffset;

float4 _DepthColor;
float4 _StencilColor;

void Vertex(uint vid : SV_VertexID,
            out float4 position : SV_Position,
            out float2 texCoord : TEXCOORD0)
{
    const float far = UNITY_RAW_FAR_CLIP_VALUE;
    position = GetFullScreenTriangleVertexPosition(vid, far);
    texCoord = GetFullScreenTriangleTexCoord(vid);
}

void Fragment(float4 position : SV_Position,
              float2 texCoord : TEXCOORD0,
              out float4 outColor : SV_Target,
              out float outDepth : SV_Depth)
{
    // Color/depth samples
    float4 color = tex2D(_ColorTexture, texCoord);
    float depth = tex2D(_DepthTexture, texCoord).x;

    // World space position
    float3 wpos = DistanceToWorldPosition
      (texCoord, depth, _RayParams, _InverseView);

    // Depth range mask
    float d_near = 1 - smoothstep(0.0, 0.1, depth - _DepthRange.x);
    float d_far = smoothstep(-0.1, 0, depth - _DepthRange.y);
    float d_safe = 1 - max(d_near, d_far);

    // Zebra pattern
    float zebra = frac(dot(texCoord, 20)) < 0.25;

    // 3-axis grid lines
    float3 wpc = wpos * 5;
    wpc = min(frac(1 - wpc), frac(wpc)) / fwidth(wpc);
    wpc = 1 - saturate(wpc * 0.5);
    float grid = max(max(wpc.x, wpc.y), wpc.z);

    // Depth overlay
    float d_ovr = d_safe * grid;
    d_ovr = max(d_ovr, d_near * zebra);
    d_ovr = max(d_ovr, d_far * zebra);

    // Stencil edge lines
    float s_edge = color.a * 2 - 1;
    s_edge = saturate(1 - 0.2 * abs(s_edge / fwidth(s_edge)));

    // Blending
    float3 rgb = color.rgb;
    rgb = lerp(rgb, _DepthColor.rgb, _DepthColor.a * d_ovr);
    rgb = lerp(rgb, _StencilColor.rgb, _StencilColor.a * s_edge);

    // Output
    outColor = float4(rgb, 1);
    outDepth = DistanceToDepth(depth + _DepthOffset);
}

    ENDHLSL

    SubShader
    {
        Pass
        {
            Cull Off ZWrite On ZTest LEqual
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDHLSL
        }
    }
}
