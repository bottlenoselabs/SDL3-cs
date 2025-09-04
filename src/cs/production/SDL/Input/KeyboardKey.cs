// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

#pragma warning disable CA1028

/// <summary>
///     Defines the buttons on a keyboard independent of any keyboard layout or any modifiers.
/// </summary>
/// <remarks>
///     <para>
///         Use <see cref="KeyboardKey" /> for when you don't care about the physical location of a key on a
///         keyboard nor the modifiers.
///     </para>
/// </remarks>
[PublicAPI]
public enum KeyboardKey : uint
{
    /// <summary>
    ///     Unknown virtual key.
    /// </summary>
    Unknown = 0x00000000u,

    /// <summary>
    ///     The '\r' virtual key.
    /// </summary>
    Return = 0x0000000du,

    /// <summary>
    ///     The '\x1B' virtual key.
    /// </summary>
    Escape = 0x0000001bu,

    /// <summary>
    ///     The '\b' virtual key.
    /// </summary>
    Backspace = 0x00000008u,

    /// <summary>
    ///     The '\t' virtual key.
    /// </summary>
    Tab = 0x00000009u,

    /// <summary>
    ///     The ' ' virtual key.
    /// </summary>
    Space = 0x00000020u,

    /// <summary>
    ///     The '!' virtual key.
    /// </summary>
    Exclaim = 0x00000021u,

    /// <summary>
    ///     The '"' virtual key.
    /// </summary>
    DoubleApostrophe = 0x00000022u,

    /// <summary>
    ///     The '#' virtual key.
    /// </summary>
    Hash = 0x00000023u,

    /// <summary>
    ///     The '$' virtual key.
    /// </summary>
    Dollar = 0x00000024u,

    /// <summary>
    ///     The '%' virtual key.
    /// </summary>
    Percent = 0x00000025u,

    /// <summary>
    ///     The '&amp;' virtual key.
    /// </summary>
    Ampersand = 0x00000026u,

    /// <summary>
    ///     The '\'' virtual key.
    /// </summary>
    Apostrophe = 0x00000027u,

    /// <summary>
    ///     The '(' virtual key.
    /// </summary>
    LeftParenthesis = 0x00000028u,

    /// <summary>
    ///     The ')' virtual key.
    /// </summary>
    RightParenthesis = 0x00000029u,

    /// <summary>
    ///     The '*' virtual key.
    /// </summary>
    Asterisk = 0x0000002au,

    /// <summary>
    ///     The '+' virtual key.
    /// </summary>
    Plus = 0x0000002bu,

    /// <summary>
    ///     The ',' virtual key.
    /// </summary>
    Comma = 0x0000002cu,

    /// <summary>
    ///     The '-' virtual key.
    /// </summary>
    Minus = 0x0000002du,

    /// <summary>
    ///     The '.' virtual key.
    /// </summary>
    Period = 0x0000002eu,

    /// <summary>
    ///     The '/' virtual key.
    /// </summary>
    Slash = 0x0000002fu,

    /// <summary>
    ///     The '0' virtual key.
    /// </summary>
    Number0 = 0x00000030u,

    /// <summary>
    ///     The '1' virtual key.
    /// </summary>
    Number1 = 0x00000031u,

    /// <summary>
    ///     The '2' virtual key.
    /// </summary>
    Number2 = 0x00000032u,

    /// <summary>
    ///     The '3' virtual key.
    /// </summary>
    Number3 = 0x00000033u,

    /// <summary>
    ///     The '4' virtual key.
    /// </summary>
    Number4 = 0x00000034u,

    /// <summary>
    ///     The '5' virtual key.
    /// </summary>
    Number5 = 0x00000035u,

    /// <summary>
    ///     The '6' virtual key.
    /// </summary>
    Number6 = 0x00000036u,

    /// <summary>
    ///     The '7' virtual key.
    /// </summary>
    Number7 = 0x00000037u,

    /// <summary>
    ///     The '8' virtual key.
    /// </summary>
    Number8 = 0x00000038u,

    /// <summary>
    ///     The '9' virtual key.
    /// </summary>
    Number9 = 0x00000039u,

    /// <summary>
    ///     The ':' virtual key.
    /// </summary>
    Colon = 0x0000003au,

    /// <summary>
    ///     The ';' virtual key.
    /// </summary>
    SemiColon = 0x0000003bu,

    /// <summary>
    ///     The '&lt;' virtual key.
    /// </summary>
    Less = 0x0000003cu,

    /// <summary>
    ///     The '=' virtual key.
    /// </summary>
    Equals = 0x0000003du,

    /// <summary>
    ///     The '&gt;' virtual key.
    /// </summary>
    Greater = 0x0000003eu,

    /// <summary>
    ///     The '?' virtual key.
    /// </summary>
    Question = 0x0000003fu,

    /// <summary>
    ///     The '@' virtual key.
    /// </summary>
    At = 0x00000040u,

    /// <summary>
    ///     The '[' virtual key.
    /// </summary>
    LeftBracket = 0x0000005bu,

    /// <summary>
    ///     The '\\' virtual key.
    /// </summary>
    Backslash = 0x0000005cu,

    /// <summary>
    ///     The ']' virtual key.
    /// </summary>
    RightBracket = 0x0000005du,

    /// <summary>
    ///     The '^' virtual key.
    /// </summary>
    Caret = 0x0000005eu,

    /// <summary>
    ///     The '_' virtual key.
    /// </summary>
    Underscore = 0x0000005fu,

    /// <summary>
    ///     The '`' virtual key.
    /// </summary>
    Grave = 0x00000060u,

    /// <summary>
    ///     The 'a' virtual key.
    /// </summary>
    A = 0x00000061u,

    /// <summary>
    ///     The 'b' virtual key.
    /// </summary>
    B = 0x00000062u,

    /// <summary>
    ///     The 'c' virtual key.
    /// </summary>
    C = 0x00000063u,

    /// <summary>
    ///     The 'd' virtual key.
    /// </summary>
    D = 0x00000064u,

    /// <summary>
    ///     The 'e' virtual key.
    /// </summary>
    E = 0x00000065u,

    /// <summary>
    ///     The 'f' virtual key.
    /// </summary>
    F = 0x00000066u,

    /// <summary>
    ///     The 'g' virtual key.
    /// </summary>
    G = 0x00000067u,

    /// <summary>
    ///     The 'h' virtual key.
    /// </summary>
    H = 0x00000068u,

    /// <summary>
    ///     The 'i' virtual key.
    /// </summary>
    I = 0x00000069u,

    /// <summary>
    ///     The 'j' virtual key.
    /// </summary>
    J = 0x0000006au,

    /// <summary>
    ///     The 'k' virtual key.
    /// </summary>
    K = 0x0000006bu,

    /// <summary>
    ///     The 'l' virtual key.
    /// </summary>
    L = 0x0000006cu,

    /// <summary>
    ///     The 'm' virtual key.
    /// </summary>
    M = 0x0000006du,

    /// <summary>
    ///     The 'n' virtual key.
    /// </summary>
    N = 0x0000006eu,

    /// <summary>
    ///     The 'o' virtual key.
    /// </summary>
    O = 0x0000006fu,

    /// <summary>
    ///     The 'p' virtual key.
    /// </summary>
    P = 0x00000070u,

    /// <summary>
    ///     The 'q' virtual key.
    /// </summary>
    Q = 0x00000071u,

    /// <summary>
    ///     The 'r' virtual key.
    /// </summary>
    R = 0x00000072u,

    /// <summary>
    ///     The 's' virtual key.
    /// </summary>
    S = 0x00000073u,

    /// <summary>
    ///     The 't' virtual key.
    /// </summary>
    T = 0x00000074u,

    /// <summary>
    ///     The 'u' virtual key.
    /// </summary>
    U = 0x00000075u,

    /// <summary>
    ///     The 'v' virtual key.
    /// </summary>
    V = 0x00000076u,

    /// <summary>
    ///     The 'w' virtual key.
    /// </summary>
    W = 0x00000077u,

    /// <summary>
    ///     The 'x' virtual key.
    /// </summary>
    X = 0x00000078u,

    /// <summary>
    ///     The 'y' virtual key.
    /// </summary>
    Y = 0x00000079u,

    /// <summary>
    ///     The 'z' virtual key.
    /// </summary>
    Z = 0x0000007au,

    /// <summary>
    ///     The '{' virtual key.
    /// </summary>
    LeftBrace = 0x0000007bu,

    /// <summary>
    ///     The '|' virtual key.
    /// </summary>
    Pipe = 0x0000007cu,

    /// <summary>
    ///     The '}' virtual key.
    /// </summary>
    RightBrace = 0x0000007du,

    /// <summary>
    ///     The '~' virtual key.
    /// </summary>
    Tilde = 0x0000007eu,

    /// <summary>
    ///     The '\x7F' virtual key.
    /// </summary>
    Delete = 0x0000007fu,

    /// <summary>
    ///     The '\xB1' virtual key.
    /// </summary>
    PlusMinus = 0x000000b1u,

    /// <summary>
    ///     The capitalization lock virtual key.
    /// </summary>
    CapsLock = 0x40000039u,

    /// <summary>
    ///     The F1 virtual key.
    /// </summary>
    F1 = 0x4000003au,

    /// <summary>
    ///     The F2 virtual key.
    /// </summary>
    F2 = 0x4000003bu,

    /// <summary>
    ///     The F3 virtual key.
    /// </summary>
    F3 = 0x4000003cu,

    /// <summary>
    ///     The F4 virtual key.
    /// </summary>
    F4 = 0x4000003du,

    /// <summary>
    ///     The F5 virtual key.
    /// </summary>
    F5 = 0x4000003eu,

    /// <summary>
    ///     The F6 virtual key.
    /// </summary>
    F6 = 0x4000003fu,

    /// <summary>
    ///     The F7 virtual key.
    /// </summary>
    F7 = 0x40000040u,

    /// <summary>
    ///     The F8 virtual key.
    /// </summary>
    F8 = 0x40000041u,

    /// <summary>
    ///     The F9 virtual key.
    /// </summary>
    F9 = 0x40000042u,

    /// <summary>
    ///     The F10 virtual key.
    /// </summary>
    F10 = 0x40000043u,

    /// <summary>
    ///     The F11 virtual key.
    /// </summary>
    F11 = 0x40000044u,

    /// <summary>
    ///     The F12 virtual key.
    /// </summary>
    F12 = 0x40000045u,

    /// <summary>
    ///     The print screen virtual key.
    /// </summary>
    PrintScreen = 0x40000046u,

    /// <summary>
    ///     The scroll lock virtual key.
    /// </summary>
    ScrollLock = 0x40000047u,

    /// <summary>
    ///     The pause virtual key.
    /// </summary>
    Pause = 0x40000048u,

    /// <summary>
    ///     The insert virtual key.
    /// </summary>
    Insert = 0x40000049u,

    /// <summary>
    ///     The home virtual key.
    /// </summary>
    Home = 0x4000004au,

    /// <summary>
    ///     The page up virtual key.
    /// </summary>
    PageUp = 0x4000004bu,

    /// <summary>
    ///     The end virtual key.
    /// </summary>
    End = 0x4000004du,

    /// <summary>
    ///     The page up virtual key.
    /// </summary>
    PageDown = 0x4000004eu,

    /// <summary>
    ///     The right virtual key.
    /// </summary>
    Right = 0x4000004fu,

    /// <summary>
    ///     The left virtual key.
    /// </summary>
    Left = 0x40000050u,

    /// <summary>
    ///     The down virtual key.
    /// </summary>
    Down = 0x40000051u,

    /// <summary>
    ///     The up virtual key.
    /// </summary>
    Up = 0x40000052u,

    /// <summary>
    ///     The num lock clear virtual key.
    /// </summary>
    NumLockClear = 0x40000053u,

    /// <summary>
    ///     The keypad divide virtual key.
    /// </summary>
    KeyPadDivide = 0x40000054u,

    /// <summary>
    ///     The keypad multiply virtual key.
    /// </summary>
    KeyPadMultiply = 0x40000055u,

    /// <summary>
    ///     The keypad minus virtual key.
    /// </summary>
    KeyPadMinus = 0x40000056u,

    /// <summary>
    ///     The keypad plus virtual key.
    /// </summary>
    KeyPadPlus = 0x40000057u,

    /// <summary>
    ///     The keypad enter virtual key.
    /// </summary>
    KeyPadEnter = 0x40000058u,

    /// <summary>
    ///     The keypad '1' virtual key.
    /// </summary>
    KeyPadNumber1 = 0x40000059u,

    /// <summary>
    ///     The keypad '2' virtual key.
    /// </summary>
    KeyPadNumber2 = 0x4000005au,

    /// <summary>
    ///     The keypad '3' virtual key.
    /// </summary>
    KeyPadNumber3 = 0x4000005bu,

    /// <summary>
    ///     The keypad '4' virtual key.
    /// </summary>
    KeyPadNumber4 = 0x4000005cu,

    /// <summary>
    ///     The keypad '5' virtual key.
    /// </summary>
    KeyPadNumber5 = 0x4000005du,

    /// <summary>
    ///     The keypad '6' virtual key.
    /// </summary>
    KeyPadNumber6 = 0x4000005eu,

    /// <summary>
    ///     The keypad '7' virtual key.
    /// </summary>
    KeyPadNumber7 = 0x4000005fu,

    /// <summary>
    ///     The keypad '8' virtual key.
    /// </summary>
    KeyPadNumber8 = 0x40000060u,

    /// <summary>
    ///     The keypad '9' virtual key.
    /// </summary>
    KeyPadNumber9 = 0x40000061u,

    /// <summary>
    ///     The keypad '0' virtual key.
    /// </summary>
    KeyPadNumber0 = 0x40000062u,

    /// <summary>
    ///     The keypad '.' virtual key.
    /// </summary>
    KeyPadPeriod = 0x40000063u,

    /// <summary>
    ///     The application virtual key.
    /// </summary>
    Application = 0x40000065u,

    /// <summary>
    ///     The power virtual key.
    /// </summary>
    Power = 0x40000066u,

    /// <summary>
    ///     The keypad '=' virtual key.
    /// </summary>
    KeyPadEquals = 0x40000067u,

    /// <summary>
    ///     The F13 virtual key.
    /// </summary>
    F13 = 0x40000068u,

    /// <summary>
    ///     The F14 virtual key.
    /// </summary>
    F14 = 0x40000069u,

    /// <summary>
    ///     The F15 virtual key.
    /// </summary>
    F15 = 0x4000006au,

    /// <summary>
    ///     The F16 virtual key.
    /// </summary>
    F16 = 0x4000006bu,

    /// <summary>
    ///     The F17 virtual key.
    /// </summary>
    F17 = 0x4000006cu,

    /// <summary>
    ///     The F18 virtual key.
    /// </summary>
    F18 = 0x4000006du,

    /// <summary>
    ///     The F19 virtual key.
    /// </summary>
    F19 = 0x4000006eu,

    /// <summary>
    ///     The F20 virtual key.
    /// </summary>
    F20 = 0x4000006fu,

    /// <summary>
    ///     The F21 virtual key.
    /// </summary>
    F21 = 0x40000070u,

    /// <summary>
    ///     The F22 virtual key.
    /// </summary>
    F22 = 0x40000071u,

    /// <summary>
    ///     The F23 virtual key.
    /// </summary>
    F23 = 0x40000072u,

    /// <summary>
    ///     The F24 virtual key.
    /// </summary>
    F24 = 0x40000073u,

    /// <summary>
    ///     The execute virtual key.
    /// </summary>
    Execute = 0x40000074u,

    /// <summary>
    ///     The help virtual key.
    /// </summary>
    Help = 0x40000075u,

    /// <summary>
    ///     The menu virtual key.
    /// </summary>
    Menu = 0x40000076u,

    /// <summary>
    ///     The select virtual key.
    /// </summary>
    Select = 0x40000077u,

    /// <summary>
    ///     The stop virtual key.
    /// </summary>
    Stop = 0x40000078u,

    /// <summary>
    ///     The again virtual key.
    /// </summary>
    Again = 0x40000079u,

    /// <summary>
    ///     The undo virtual key.
    /// </summary>
    Undo = 0x4000007au,

    /// <summary>
    ///     The cut virtual key.
    /// </summary>
    Cut = 0x4000007bu,

    /// <summary>
    ///     The copy virtual key.
    /// </summary>
    Copy = 0x4000007cu,

    /// <summary>
    ///     The paste virtual key.
    /// </summary>
    Paste = 0x4000007du,

    /// <summary>
    ///     The find virtual key.
    /// </summary>
    Find = 0x4000007eu,

    /// <summary>
    ///     The mute virtual key.
    /// </summary>
    Mute = 0x4000007fu,

    /// <summary>
    ///     The volume up virtual key.
    /// </summary>
    VolumeUp = 0x40000080u,

    /// <summary>
    ///     The volume down virtual key.
    /// </summary>
    VolumeDown = 0x40000081u,

    /// <summary>
    ///     The keypad ',' virtual key.
    /// </summary>
    KeyPadComma = 0x40000085u,

    /// <summary>
    ///     The left control virtual key.
    /// </summary>
    LeftControl = 0x400000e0u,

    /// <summary>
    ///     The left shift virtual key.
    /// </summary>
    LeftShift = 0x400000e1u,

    /// <summary>
    ///     The left alt virtual key.
    /// </summary>
    LeftAlt = 0x400000e2u,

    /// <summary>
    ///     The right control virtual key.
    /// </summary>
    RightControl = 0x400000e4u,

    /// <summary>
    ///     The right shift virtual key.
    /// </summary>
    RightShift = 0x400000e5u,

    /// <summary>
    ///     The right alt virtual key.
    /// </summary>
    RightAlt = 0x400000e6u,
}
