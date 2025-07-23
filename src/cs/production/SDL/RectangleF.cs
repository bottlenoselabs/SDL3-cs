// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

#pragma warning disable CA1815

/// <summary>
///     Defines a rectangle with 32-bit float components.
/// </summary>
[PublicAPI]
public record struct RectangleF
{
    /// <summary>
    ///     The X coordinate of the rectangle.
    /// </summary>
    public float X;

    /// <summary>
    ///     The Y coordinate of the rectangle.
    /// </summary>
    public float Y;

    /// <summary>
    ///     The width of the rectangle.
    /// </summary>
    public float Width;

    /// <summary>
    ///     The height of the rectangle.
    /// </summary>
    public float Height;

    /// <summary>
    ///     Determines whether a specified <see cref="Vector2" /> position is inside the rectangle.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns><c>true</c> if <paramref name="position" /> is inside the rectangle; otherwise, <c>false</c>.</returns>
    public readonly bool Contains(Vector2 position)
    {
        if (position.X < X)
        {
            return false;
        }

        if (position.X > X + Width)
        {
            return false;
        }

        if (position.Y < Y)
        {
            return false;
        }

        if (position.Y > Y + Height)
        {
            return false;
        }

        return true;
    }
}
