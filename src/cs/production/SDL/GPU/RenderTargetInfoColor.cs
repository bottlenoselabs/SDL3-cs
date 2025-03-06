// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

#pragma warning disable CA1815

/// <summary>
///     TODO.
/// </summary>
[PublicAPI]
public struct RenderTargetInfoColor
{
    /// <summary>
    ///     TODO.
    /// </summary>
    public Texture? Texture;

    /// <summary>
    ///     TODO.
    /// </summary>
    public int MipMapLevel;

    /// <summary>
    ///     TODO.
    /// </summary>
    public int LayerOrDepthPlane;

    /// <summary>
    ///     TODO.
    /// </summary>
    public Rgba32F ClearColor;

    /// <summary>
    ///     TODO.
    /// </summary>
    public RenderTargetLoadOp LoadOp;

    /// <summary>
    ///     TODO.
    /// </summary>
    public RenderTargetStoreOp StoreOp;

    /// <summary>
    ///     TODO.
    /// </summary>
    public Texture? ResolveTexture;

    /// <summary>
    ///     TODO.
    /// </summary>
    public int ResolveMipMapLevel;

    /// <summary>
    ///     TODO.
    /// </summary>
    public int ResolveLayer;

    /// <summary>
    ///     TODO.
    /// </summary>
    public bool IsTextureCycled;

    /// <summary>
    ///     TODO.
    /// </summary>
    public bool IsResolveTextureCycled;
}
