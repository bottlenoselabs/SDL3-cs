// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines a texture region used in a blit operation.
/// </summary>
[PublicAPI]
public record struct GpuBlitInfoTextureRegion
{
    /// <summary>
    ///     The texture.
    /// </summary>
    public GpuTexture Texture;

    /// <summary>
    ///     The mip map level index of the texture.
    /// </summary>
    public int MipMapLevel;

    /// <summary>
    ///     The layer index or depth plane index of the texture.
    /// </summary>
    public int LayerOrDepthPlane;

    /// <summary>
    ///     The bounds of the texture region.
    /// </summary>
    public Rectangle Bounds;
}
