// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines a keyboard event.
/// </summary>
public readonly record struct KeyboardEvent
{
    /// <summary>
    ///     The <see cref="bottlenoselabs.SDL.Window" /> associated with the event.
    /// </summary>
    public readonly Window Window;

    /// <summary>
    ///     The <see cref="bottlenoselabs.SDL.KeyboardButton" /> associated with the event.
    /// </summary>
    public readonly KeyboardButton Key;

    /// <summary>
    ///     Determines whether the <see cref="Key" /> associated with the event is pressed down.
    /// </summary>
    public readonly bool IsDown;

    /// <summary>
    ///     Determines whether the <see cref="Key" /> associated with the event has been pressed down continuously.
    /// </summary>
    public readonly bool IsRepeat;

    internal KeyboardEvent(Window window, KeyboardButton key, bool isDown, bool isRepeat)
    {
        Window = window;
        Key = key;
        IsDown = isDown;
        IsRepeat = isRepeat;
    }
}
