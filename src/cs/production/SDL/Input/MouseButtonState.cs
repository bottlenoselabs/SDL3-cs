// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines the state of the buttons on a mouse input device.
/// </summary>
[Flags]
public enum MouseButtonState
{
    /// <summary>
    ///     No mouse button.
    /// </summary>
    None = 0,

    /// <summary>
    ///     The left mouse button.
    /// </summary>
    Left = 1 << 0,

    /// <summary>
    ///     The middle mouse button.
    /// </summary>
    Middle = 1 << 1,

    /// <summary>
    ///     The right mouse button.
    /// </summary>
    Right = 1 << 2,

    /// <summary>
    ///     The X1 mouse button.
    /// </summary>
    X1 = 1 << 3,

    /// <summary>
    ///     The X1 mouse button.
    /// </summary>
    X2 = 1 << 4,
}
