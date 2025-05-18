// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

// ReSharper disable InconsistentNaming

/// <summary>
///     Defines the texture format and colorspace of the swapchain textures.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="GpuShaderFormats" /> is blittable to the C `SDL_GPUShaderFormat` enum found in `SDL_gpu`.
///     </para>
/// </remarks>
[PublicAPI]
public enum GpuSwapchainComposition
{
    /// <summary>
    ///     TODO.
    /// </summary>
    SDR = 0,

    /// <summary>
    ///     TODO.
    /// </summary>
    SDRLinear = 1,

    /// <summary>
    ///     TODO.
    /// </summary>
    HDRExtendedLinear = 2,

    /// <summary>
    ///     TODO.
    /// </summary>
    HDR10ST2084 = 3
}
