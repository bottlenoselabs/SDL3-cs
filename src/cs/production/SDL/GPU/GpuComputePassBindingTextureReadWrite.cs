// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

#pragma warning disable CA1815
#pragma warning disable SA1202

/// <summary>
///     TODO.
/// </summary>
public record struct GpuComputePassBindingTextureReadWrite
{
    /// <summary>
    ///     TODO.
    /// </summary>
    public nint Texture;

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

    /// <summary>
    ///     TODO.
    /// </summary>
    /// <param name="texture">The texture.</param>
    public unsafe void SetTexture(GpuTexture texture)
    {
        Texture = (IntPtr)texture.HandleTyped;
    }
}
