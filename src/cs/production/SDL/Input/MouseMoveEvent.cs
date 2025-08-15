// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines a mouse move event.
/// </summary>
public readonly record struct MouseMoveEvent
{
    /// <summary>
    ///     The <see cref="bottlenoselabs.SDL.Window" /> associated with the event.
    /// </summary>
    public readonly Window Window;

    /// <summary>
    ///     The position of the mouse relative to the <see cref="Window" /> at the time of the event.
    /// </summary>
    public readonly Vector2 Position;

    /// <summary>
    ///     The change in position of the mouse at the time of the event.
    /// </summary>
    public readonly Vector2 PositionDelta;

    /// <summary>
    ///     The button state of the mouse at the time of the event.
    /// </summary>
    public readonly MouseButtonState ButtonState;

    internal MouseMoveEvent(
        Window window, Vector2 position, Vector2 positionDelta, MouseButtonState buttonState)
    {
        Window = window;
        Position = position;
        PositionDelta = positionDelta;
        ButtonState = buttonState;
    }
}
