// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

#pragma warning disable CA1815

/// <summary>
///     Defines the stencil operation state of the graphics pipeline.
/// </summary>
[PublicAPI]
public sealed class GraphicsPipelineStencilOpState
{
    /// <summary>
    ///     Gets or sets the operation performed on samples that fail the stencil-test.
    /// </summary>
    public StencilOp FailOp { get; set; }

    /// <summary>
    ///     Gets or sets the operation performed on samples that pass the depth-test and stencil-test.
    /// </summary>
    public StencilOp PassOp { get; set; }

    /// <summary>
    ///     Gets or sets the operation performed on samples that pass the stencil test and fail the depth test.
    /// </summary>
    public StencilOp DepthFailOp { get; set; }

    /// <summary>
    ///     Gets or sets the comparison operator used in the stencil test.
    /// </summary>
    public CompareOp CompareOp { get; set; }

    /// <summary>
    ///     Resets the stencil operation state to default values.
    /// </summary>
    public void Reset()
    {
        FailOp = StencilOp.Invalid;
        PassOp = StencilOp.Invalid;
        DepthFailOp = StencilOp.Invalid;
        CompareOp = CompareOp.Invalid;
    }
}
