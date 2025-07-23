// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines the operation to a stored stencil value if a stencil test fails or passes.
/// </summary>
[PublicAPI]
public enum GpuStencilOp
{
    /// <summary>
    ///     Invalid.
    /// </summary>
    Invalid = 0,

    /// <summary>
    ///     Keeps the current value.
    /// </summary>
    Keep = 1,

    /// <summary>
    ///     Sets the value to <c>0</c>.
    /// </summary>
    Zero = 2,

    /// <summary>
    ///     Sets the value to the reference value.
    /// </summary>
    Replace = 3,

    /// <summary>
    ///     Increments the current value and clamps to the maximum value.
    /// </summary>
    IncrementAndClamp = 4,

    /// <summary>
    ///     Decrements the current value and clamps to <c>0</c>.
    /// </summary>
    DecrementAndClamp = 5,

    /// <summary>
    ///     Bitwise-inverts the current value.
    /// </summary>
    Invert = 6,

    /// <summary>
    ///     Increments the current value and wraps back to <c>0</c>.
    /// </summary>
    IncrementAndWrap = 7,

    /// <summary>
    ///     Decrements the current value and wraps to the maximum value.
    /// </summary>
    DecrementAndWrap = 8
}
