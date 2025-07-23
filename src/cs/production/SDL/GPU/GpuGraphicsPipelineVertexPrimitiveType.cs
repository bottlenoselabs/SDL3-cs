// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines the vertex topology, how vertices are grouped and interpreted, for rasterization by the graphics
///     pipeline.
/// </summary>
[PublicAPI]
public enum GpuGraphicsPipelineVertexPrimitiveType
{
    /// <summary>
    ///     Every three vertices form a separate triangle.
    /// </summary>
    TriangleList = 0,

    /// <summary>
    ///     Vertices form a strip of connected triangles where each vertex (after the first two vertices) creates a new
    ///     triangle with the previous two vertices.
    /// </summary>
    TriangleStrip = 1,

    /// <summary>
    ///     Every two vertices form a separate line.
    /// </summary>
    LineList = 2,

    /// <summary>
    ///     Vertices form a strip of connected lines where each vertex (after the first vertex) creates a new line with
    ///     the previous vertex.
    /// </summary>
    LineStrip = 3,

    /// <summary>
    ///     Each vertex is a single point.
    /// </summary>
    PointList = 4
}
