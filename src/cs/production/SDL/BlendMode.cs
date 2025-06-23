// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines the blend modes used for rendering options.
/// </summary>
[Flags]
public enum BlendMode
{
    /// <summary>
    ///     No blending.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Alpha blending.
    /// </summary>
    Alpha = 1 << 0,

    /// <summary>
    ///     Additive blending.
    /// </summary>
    Add = 1 << 1,

    /// <summary>
    ///     Color modulate.
    /// </summary>
    Modulate = 1 << 2,

    /// <summary>
    ///     Color multiply.
    /// </summary>
    Multiply = 1 << 3
}
