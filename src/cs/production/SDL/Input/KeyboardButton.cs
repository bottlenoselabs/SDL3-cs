// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines the buttons on a keyboard based on the QWERTY (EN-US) keyboard layout.
/// </summary>
/// <remarks>
///     <para>
///         Use <see cref="KeyboardButton" /> for when you want the physical location of a key independent of the
///         layout. For example, comparing the physical layout of a French keyboard and an English keyboard, the
///         location of the keys "WASD" on the English keyboard would be same location for the "ZQSD" keys on the French
///         keyboard.
///     </para>
/// </remarks>
[PublicAPI]
public enum KeyboardButton
{
    /// <summary>
    ///     Unknown keyboard button.
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     The up arrow key.
    /// </summary>
    Up,

    /// <summary>
    ///     The down arrow key.
    /// </summary>
    Down,

    /// <summary>
    ///     The left arrow key.
    /// </summary>
    Left,

    /// <summary>
    ///     The right arrow key.
    /// </summary>
    Right,

    /// <summary>
    ///     The A key.
    /// </summary>
    A,

    /// <summary>
    ///     The B key.
    /// </summary>
    B,

    /// <summary>
    ///     The C key.
    /// </summary>
    C,

    /// <summary>
    ///     The D key.
    /// </summary>
    D,

    /// <summary>
    ///     The E key.
    /// </summary>
    E,

    /// <summary>
    ///     The F key.
    /// </summary>
    F,

    /// <summary>
    ///     The G key.
    /// </summary>
    G,

    /// <summary>
    ///     The H key.
    /// </summary>
    H,

    /// <summary>
    ///     The I key.
    /// </summary>
    I,

    /// <summary>
    ///     The J key.
    /// </summary>
    J,

    /// <summary>
    ///     The K key.
    /// </summary>
    K,

    /// <summary>
    ///     The L key.
    /// </summary>
    L,

    /// <summary>
    ///     The M key.
    /// </summary>
    M,

    /// <summary>
    ///     The N key.
    /// </summary>
    N,

    /// <summary>
    ///     The O key.
    /// </summary>
    O,

    /// <summary>
    ///     The P key.
    /// </summary>
    P,

    /// <summary>
    ///     The Q key.
    /// </summary>
    Q,

    /// <summary>
    ///     The R key.
    /// </summary>
    R,

    /// <summary>
    ///     The S key.
    /// </summary>
    S,

    /// <summary>
    ///     The T key.
    /// </summary>
    T,

    /// <summary>
    ///     The U key.
    /// </summary>
    U,

    /// <summary>
    ///     The V key.
    /// </summary>
    V,

    /// <summary>
    ///     The W key.
    /// </summary>
    W,

    /// <summary>
    ///     The X key.
    /// </summary>
    X,

    /// <summary>
    ///     The Y key.
    /// </summary>
    Y,

    /// <summary>
    ///     The Z key.
    /// </summary>
    Z,

    /// <summary>
    ///     The number 1 key.
    /// </summary>
    Number1,

    /// <summary>
    ///     The number 2 key.
    /// </summary>
    Number2,

    /// <summary>
    ///     The number 3 key.
    /// </summary>
    Number3,

    /// <summary>
    ///     The number 4 key.
    /// </summary>
    Number4,

    /// <summary>
    ///     The number 5 key.
    /// </summary>
    Number5,

    /// <summary>
    ///     The number 6 key.
    /// </summary>
    Number6,

    /// <summary>
    ///     The number 7 key.
    /// </summary>
    Number7,

    /// <summary>
    ///     The number 8 key.
    /// </summary>
    Number8,

    /// <summary>
    ///     The number 9 key.
    /// </summary>
    Number9,

    /// <summary>
    ///     The number 0 key.
    /// </summary>
    Number0
}
