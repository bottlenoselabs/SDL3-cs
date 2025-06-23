// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL.GPU;

/// <summary>
///     Defines the rate for fetching vertex attributes from a vertex buffer into the per-vertex processing stage of the
///     graphics pipeline. Use <see cref="PerInstance" /> to enable instancing and <see cref="PerVertex" /> to disable
///     instancing.
/// </summary>
[PublicAPI]
public enum GpuGraphicsPipelineVertexInputRate
{
    /// <summary>
    ///     Vertex attribute fetching is per vertex index.
    /// </summary>
    PerVertex = 0,

    /// <summary>
    ///     Vertex attribute fetching is per instance index.
    /// </summary>
    PerInstance
}
