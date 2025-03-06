// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Defines how texture coordinates (UV coordinates or UVW coordinates) outside the [0, 1) range are handled when
///     sampling.
/// </summary>
public enum SamplerAddressMode
{
    /// <summary>
    ///     TODO.
    /// </summary>
    Repeat = 0,

    /// <summary>
    ///     TODO.
    /// </summary>
    MirroredRepeat = 1,

    /// <summary>
    ///     TODO.
    /// </summary>
    ClampToEdge = 2
}
