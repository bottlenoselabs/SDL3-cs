// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

#pragma warning disable CA1815

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines a point with 32-bit float components.
/// </summary>
[PublicAPI]
public record struct PointF
{
    /// <summary>
    ///     The X coordinate of the rectangle.
    /// </summary>
    public float X;

    /// <summary>
    ///     The Y coordinate of the rectangle.
    /// </summary>
    public float Y;
}
