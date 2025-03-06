// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Defines the different types of a <see cref="Texture" />.
/// </summary>
[PublicAPI]
public enum TextureType
{
    /// <summary>
    ///     The <see cref="Texture" /> has one sub-texture with dimensions: width, height.
    /// </summary>
    TwoDimensional = 0,

    /// <summary>
    ///     The <see cref="Texture" /> has one or more <see cref="TextureType.TwoDimensional" /> sub-textures.
    /// </summary>
    TwoDimensionalArray = 1,

    /// <summary>
    ///     The <see cref="Texture" /> has one 3D sub-texture with dimensions: width, height, depth.
    /// </summary>
    ThreeDimensional = 2,

    /// <summary>
    ///     The <see cref="Texture" /> has six <see cref="TextureType.TwoDimensional" /> sub-textures, one for
    ///     each face of a cube.
    /// </summary>
    Cube = 3,

    /// <summary>
    ///     The <see cref="Texture" /> has one or more <see cref="TextureType.Cube" /> sub-textures.
    /// </summary>
    CubeArray = 4
}
