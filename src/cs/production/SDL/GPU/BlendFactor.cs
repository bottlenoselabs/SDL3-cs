// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Defines the multiplication weights that can be applied to either source color/alpha (new fragment) or
///     destination color/alpha (existing render-target texel) during blending operations. Blending combines these
///     colors/alpha values to achieve effects like transparency, anti-aliasing, or compositing.
/// </summary>
/// <remarks>
///     <para>
///         The blending equation is
///         <c>Result = (SourceColor * SourceFactor) ◻ (DestinationColor * DestinationFactor)</c> where <c>◻</c> is a
///         blend operator (e.g., addition, subtraction). <c>SourceFactor</c> is the weight applied to the incoming
///         fragment's color. <c>DestinationFactor</c> is the weight applied to the existing render-target color.
///     </para>
/// </remarks>
[PublicAPI]
public enum BlendFactor
{
    /// <summary>
    ///     Invalid blend factor.
    /// </summary>
    Invalid = 0,

    /// <summary>
    ///     Blend factor of <c>0</c>.
    /// </summary>
    Zero = 1,

    /// <summary>
    ///     Blend factor of <c>one</c>.
    /// </summary>
    One = 2,

    /// <summary>
    ///     Blend factor of <c>SourceColor</c>.
    /// </summary>
    SourceColor = 3,

    /// <summary>
    ///     Blend factor of <c>1 - SourceColor</c>.
    /// </summary>
    OneMinusSourceColor = 4,

    /// <summary>
    ///     Blend factor of <c>DestinationColor</c>.
    /// </summary>
    DestinationColor = 5,

    /// <summary>
    ///     Blend factor of <c>1 - DestinationColor</c>.
    /// </summary>
    OneMinusDestinationColor = 6,

    /// <summary>
    ///     Blend factor of <c>SourceAlpha</c>.
    /// </summary>
    SourceAlpha = 7,

    /// <summary>
    ///     Blend factor of <c>1 - SourceAlpha</c>.
    /// </summary>
    OneMinusSourceAlpha = 8,

    /// <summary>
    ///     Blend factor of <c>DestinationAlpha</c>.
    /// </summary>
    DestinationAlpha = 9,

    /// <summary>
    ///     Blend factor of <c>1 - DestinationAlpha</c>.
    /// </summary>
    OneMinusDestinationAlpha = 10,

    /// <summary>
    ///     Blend factor of <c>ConstantColor</c>.
    /// </summary>
    ConstantColor = 11,

    /// <summary>
    ///     Blend factor of <c>1 - ConstantColor</c>.
    /// </summary>
    OneMinusConstantColor = 12,

    /// <summary>
    ///     Blend factor of <c>Minimum(SourceAlpha, 1 - DestinationAlpha)</c>.
    /// </summary>
    SourceAlphaSaturate = 13
}
