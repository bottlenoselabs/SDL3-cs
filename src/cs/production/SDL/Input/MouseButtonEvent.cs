// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines a mouse button event.
/// </summary>
[PublicAPI]
public readonly record struct MouseButtonEvent
{
    /// <summary>
    ///     The <see cref="bottlenoselabs.SDL.Window" /> associated with the event.
    /// </summary>
    public readonly Window Window;

    /// <summary>
    ///     The mouse button associated with the event.
    /// </summary>
    public readonly MouseButton MouseButton;

    /// <summary>
    ///     Determines whether the <see cref="MouseButton" /> associated with the event is pressed down.
    /// </summary>
    public readonly bool IsDown;

    /// <summary>
    ///     The number of button clicks. <c>1</c> for single-click, <c>2</c> for double-click, etc.
    /// </summary>
    public readonly int ClicksCount;

    /// <summary>
    ///     The position of the mouse relative to the <see cref="Window" /> at the time of the event.
    /// </summary>
    public readonly Vector2 Position;

    internal MouseButtonEvent(
        Window window, MouseButton button, bool isDown, int clicksCount, Vector2 position)
    {
        Window = window;
        MouseButton = button;
        IsDown = isDown;
        ClicksCount = clicksCount;
        Position = position;
    }
}
