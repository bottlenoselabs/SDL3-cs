// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines how the graphics pipeline fills primitives of type
///     <see cref="GpuGraphicsPipelineVertexPrimitiveType.TriangleList" /> or
///     <see cref="GpuGraphicsPipelineVertexPrimitiveType.TriangleStrip" /> during rasterization.
/// </summary>
[PublicAPI]
public enum GpuGraphicsPipelineFillMode
{
    /// <summary>
    ///     Rasterize <see cref="GpuGraphicsPipelineVertexPrimitiveType.TriangleList" /> and
    ///     <see cref="GpuGraphicsPipelineVertexPrimitiveType.TriangleStrip" /> primitives as filled triangles.
    /// </summary>
    Fill = 0,

    /// <summary>
    ///     Rasterize <see cref="GpuGraphicsPipelineVertexPrimitiveType.TriangleList" /> and
    ///     <see cref="GpuGraphicsPipelineVertexPrimitiveType.TriangleStrip" /> primitives as lines.
    /// </summary>
    Line = 1
}
