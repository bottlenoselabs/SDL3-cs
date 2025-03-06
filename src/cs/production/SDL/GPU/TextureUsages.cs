// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Defines the ways a <see cref="Texture" /> is to be used.
/// </summary>
[PublicAPI]
[Flags]
public enum TextureUsages
{
    /// <summary>
    ///     Invalid <see cref="Texture" /> usage.
    /// </summary>
    None = 0,

    /// <summary>
    ///     The <see cref="Texture" /> supports sampling.
    /// </summary>
    Sampler = 1 << 0,

    /// <summary>
    ///     The <see cref="Texture" /> supports usage as a color render-target.
    /// </summary>
    ColorRenderTarget = 1 << 1,

    /// <summary>
    ///     The <see cref="Texture" /> supports usage as a depth-stencil render-target.
    /// </summary>
    DepthStencilRenderTarget = 1 << 2,

    /// <summary>
    ///     The <see cref="Texture" /> supports usage of storage reads in vertex or fragment stages.
    /// </summary>
    GraphicsStorageRead = 1 << 3,

    /// <summary>
    ///     The <see cref="Texture" /> supports usage of storage reads in the compute stage.
    /// </summary>
    ComputeStorageRead = 1 << 4,

    /// <summary>
    ///     The <see cref="Texture" /> supports usage of storage writes in the compute stage.
    /// </summary>
    ComputeStorageWrite = 1 << 5,

    /// <summary>
    ///     The <see cref="Texture" /> supports usage of reads and writes in the same compute shader.
    ///     This is <b>not</b> equivalent to <see cref="ComputeStorageRead" /> or <see cref="ComputeStorageWrite" />.
    /// </summary>
    ComputeStorageSimultaneousReadWrite = 1 << 6
}
