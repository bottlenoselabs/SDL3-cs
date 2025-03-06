// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Defines the vertex winding that determines a front-facing primitive during rasterization.
/// </summary>
[PublicAPI]
public enum GraphicsPipelineFrontFace
{
    /// <summary>
    ///     Primitives with counter-clockwise vertex winding will be considered as front-facing.
    /// </summary>
    CounterClockwise = 0,

    /// <summary>
    ///      Primitives with clockwise vertex winding will be considered as front-facing.
    /// </summary>
    Clockwise = 1
}
