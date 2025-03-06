// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

#pragma warning disable CA1815

/// <summary>
///     TODO.
/// </summary>
[PublicAPI]
public struct RenderTargetInfoDepthStencil
{
    /// <summary>
    ///     TODO.
    /// </summary>
    public Texture? Texture;

    /// <summary>
    ///     TODO.
    /// </summary>
    public float ClearDepth;

    /// <summary>
    ///    TODO.
    /// </summary>
    public RenderTargetLoadOp LoadOp;

    /// <summary>
    ///     TODO.
    /// </summary>
    public RenderTargetStoreOp StoreOp;

    /// <summary>
    ///     TODO.
    /// </summary>
    public RenderTargetLoadOp StencilLoadOp;

    /// <summary>
    ///     TODO.
    /// </summary>
    public RenderTargetStoreOp StencilStoreOp;

    /// <summary>
    ///     TODO.
    /// </summary>
    public bool IsTextureCycled;

    /// <summary>
    ///     TODO.
    /// </summary>
    public byte ClearStencil;
}
