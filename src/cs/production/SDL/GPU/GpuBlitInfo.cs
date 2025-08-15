// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines parameters for a blit command.
/// </summary>
[PublicAPI]
public record struct GpuBlitInfo
{
    /// <summary>
    ///     The source texture region.
    /// </summary>
    public GpuBlitInfoTextureRegion Source;

    /// <summary>
    ///     The destination texture region.
    /// </summary>
    public GpuBlitInfoTextureRegion Destination;

    /// <summary>
    ///     The load operation used on the contents of the <see cref="Destination" /> region before the blit.
    /// </summary>
    public GpuRenderTargetLoadOperation LoadOperation;

    /// <summary>
    ///     The color to clear the contents of the <see cref="Destination" /> region before the blit.
    /// </summary>
    public Rgba32F ClearColor;

    /// <summary>
    ///     The flip mode used on the contents of the <see cref="Source" /> region.
    /// </summary>
    public FlipMode FlipMode;

    /// <summary>
    ///     The sampler filter mode used when performing the blit.
    /// </summary>
    public GpuSamplerFilterMode FilterMode;

    /// <summary>
    ///     Determines whether the <see cref="Destination" /> texture is cycled when it is already bound.
    /// </summary>
    public bool IsDestinationTextureCycled;
}
