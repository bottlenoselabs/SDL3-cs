// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

#pragma warning disable CA1815

/// <summary>
///     Defines the configuration for the rasterization stage of the graphics pipeline.
/// </summary>
/// <remarks>
///     <para>
///         The rasterizer stage converts vertex primitives (points, lines, triangles) from the per-vertex stage into a
///         raster image by performing scan-line conversion. During rasterization, vertices are transformed into the
///         homogenous clip-space and clipped. As output, per-vertex attributes may be interpolated across the primitive
///         (such as color) and made ready for the next stage of the graphics pipeline: depth-test stage, stencil-test
///         stage, or per-fragment shading stage.
///     </para>
/// </remarks>
[PublicAPI]
public sealed class GpuGraphicsPipelineRasterizerState
{
    /// <summary>
    ///     Gets or sets the way primitives of type
    ///     <see cref="GpuGraphicsPipelineVertexPrimitiveType.TriangleList" /> or
    ///     <see cref="GpuGraphicsPipelineVertexPrimitiveType.TriangleStrip" /> are filled.
    /// </summary>
    public GpuGraphicsPipelineFillMode FillMode { get; set; }

    /// <summary>
    ///     Gets or sets the facing direction in which primitives will be culled.
    /// </summary>
    public GpuGraphicsPipelineCullMode CullMode { get; set; }

    /// <summary>
    ///     Gets or sets the vertex winding that determines a front-facing primitive.
    /// </summary>
    public GpuGraphicsPipelineFrontFace FrontFace { get; set; }

    /// <summary>
    ///     Gets or sets the scalar factor controlling the depth value added to each fragment.
    /// </summary>
    public float DepthBiasConstantFactor { get; set; }

    /// <summary>
    ///     Gets or sets the maximum depth bias of a fragment.
    /// </summary>
    public float DepthBiasClamp { get; set; }

    /// <summary>
    ///     Gets or sets the scalar factor applied to a fragment's slope in depth calculations.
    /// </summary>
    public float DepthBiasSlopeFactor { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether fragment depth value bias is enabled.
    /// </summary>
    public bool IsEnabledDepthBias { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether fragment depth clamping is enabled.
    /// </summary>
    public bool IsEnabledDepthClamp { get; set; }

    /// <summary>
    ///     Resets the rasterizer state to default values.
    /// </summary>
    public void Reset()
    {
        FillMode = GpuGraphicsPipelineFillMode.Fill;
        CullMode = GpuGraphicsPipelineCullMode.None;
        FrontFace = GpuGraphicsPipelineFrontFace.CounterClockwise;
        DepthBiasConstantFactor = 0;
        DepthBiasClamp = 0;
        DepthBiasSlopeFactor = 0;
        IsEnabledDepthBias = false;
        IsEnabledDepthClamp = false;
    }
}
