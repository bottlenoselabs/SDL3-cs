// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

#pragma warning disable CA1815

/// <summary>
///     TODO.
/// </summary>
[PublicAPI]
public struct GpuRenderTargetInfoDepthStencil
{
    /// <summary>
    ///     TODO.
    /// </summary>
    public GpuTexture? Texture;

    /// <summary>
    ///     TODO.
    /// </summary>
    public float ClearDepth;

    /// <summary>
    ///    TODO.
    /// </summary>
    public GpuRenderTargetLoadOp LoadOp;

    /// <summary>
    ///     TODO.
    /// </summary>
    public GpuRenderTargetStoreOp StoreOp;

    /// <summary>
    ///     TODO.
    /// </summary>
    public GpuRenderTargetLoadOp StencilLoadOp;

    /// <summary>
    ///     TODO.
    /// </summary>
    public GpuRenderTargetStoreOp StencilStoreOp;

    /// <summary>
    ///     TODO.
    /// </summary>
    public bool IsTextureCycled;

    /// <summary>
    ///     TODO.
    /// </summary>
    public byte ClearStencil;
}
