// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Defines how the graphics pipeline fills primitives of type
///     <see cref="GraphicsPipelineVertexPrimitiveType.TriangleList" /> or
///     <see cref="GraphicsPipelineVertexPrimitiveType.TriangleStrip" /> during rasterization.
/// </summary>
[PublicAPI]
public enum GraphicsPipelineFillMode
{
    /// <summary>
    ///     Rasterize <see cref="GraphicsPipelineVertexPrimitiveType.TriangleList" /> and
    ///     <see cref="GraphicsPipelineVertexPrimitiveType.TriangleStrip" /> primitives as filled triangles.
    /// </summary>
    Fill = 0,

    /// <summary>
    ///     Rasterize <see cref="GraphicsPipelineVertexPrimitiveType.TriangleList" /> and
    ///     <see cref="GraphicsPipelineVertexPrimitiveType.TriangleStrip" /> primitives as lines.
    /// </summary>
    Line = 1
}
