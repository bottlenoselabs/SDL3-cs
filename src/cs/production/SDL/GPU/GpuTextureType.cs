// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL.GPU;

/// <summary>
///     Defines the different types of a <see cref="GpuTexture" />.
/// </summary>
[PublicAPI]
public enum GpuTextureType
{
    /// <summary>
    ///     The <see cref="GpuTexture" /> has one sub-texture with dimensions: width, height.
    /// </summary>
    TwoDimensional = 0,

    /// <summary>
    ///     The <see cref="GpuTexture" /> has one or more <see cref="GpuTextureType.TwoDimensional" /> sub-textures.
    /// </summary>
    TwoDimensionalArray = 1,

    /// <summary>
    ///     The <see cref="GpuTexture" /> has one 3D sub-texture with dimensions: width, height, depth.
    /// </summary>
    ThreeDimensional = 2,

    /// <summary>
    ///     The <see cref="GpuTexture" /> has six <see cref="GpuTextureType.TwoDimensional" /> sub-textures, one for
    ///     each face of a cube.
    /// </summary>
    Cube = 3,

    /// <summary>
    ///     The <see cref="GpuTexture" /> has one or more <see cref="GpuTextureType.Cube" /> sub-textures.
    /// </summary>
    CubeArray = 4
}
