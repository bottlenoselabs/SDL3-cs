// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     TODO.
/// </summary>
[PublicAPI]
public record struct GpuComputePassBindingTextureReadWrite
{
    /// <summary>
    ///     TODO.
    /// </summary>
    public GpuTexture Texture;

    /// <summary>
    ///     TODO.
    /// </summary>
    public int MipMapLevelIndex;

    /// <summary>
    ///     TODO.
    /// </summary>
    public int LayerOrDepthIndex;

    /// <summary>
    ///     TODO.
    /// </summary>
    public bool IsCycled;
}
