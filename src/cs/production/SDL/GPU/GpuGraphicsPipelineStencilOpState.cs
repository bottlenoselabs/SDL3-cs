// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL.GPU;

#pragma warning disable CA1815

/// <summary>
///     Defines the stencil operation state of the graphics pipeline.
/// </summary>
[PublicAPI]
public sealed class GpuGraphicsPipelineStencilOpState
{
    /// <summary>
    ///     Gets or sets the operation performed on samples that fail the stencil-test.
    /// </summary>
    public GpuStencilOp FailOp { get; set; }

    /// <summary>
    ///     Gets or sets the operation performed on samples that pass the depth-test and stencil-test.
    /// </summary>
    public GpuStencilOp PassOp { get; set; }

    /// <summary>
    ///     Gets or sets the operation performed on samples that pass the stencil test and fail the depth test.
    /// </summary>
    public GpuStencilOp DepthFailOp { get; set; }

    /// <summary>
    ///     Gets or sets the comparison operator used in the stencil test.
    /// </summary>
    public GpuCompareOp CompareOp { get; set; }

    /// <summary>
    ///     Resets the stencil operation state to default values.
    /// </summary>
    public void Reset()
    {
        FailOp = GpuStencilOp.Invalid;
        PassOp = GpuStencilOp.Invalid;
        DepthFailOp = GpuStencilOp.Invalid;
        CompareOp = GpuCompareOp.Invalid;
    }
}
