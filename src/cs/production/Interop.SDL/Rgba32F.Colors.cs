// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.Interop;

public partial struct Rgba32F
{
    // https://en.wikipedia.org/wiki/X11_color_names

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #00000000.
    /// </summary>
    public static readonly Rgba32F TransparentBlack = 0x00000000;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #000000FF.
    /// </summary>
    public static readonly Rgba32F Transparent = 0x000000FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #E73C00FF.
    /// </summary>
    public static readonly Rgba32F MonoGameOrange = 0xE73C00FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #F0F8FFFF.
    /// </summary>
    public static readonly Rgba32F AliceBlue = 0xF0F8FFFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FAEBD7FF.
    /// </summary>
    public static readonly Rgba32F AntiqueWhite = 0xFAEBD7FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #00FFFFFF.
    /// </summary>
    public static readonly Rgba32F Aqua = 0x00FFFFFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #7FFFD4FF.
    /// </summary>
    public static readonly Rgba32F Aquamarine = 0x7FFFD4FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #F0FFFFFF.
    /// </summary>
    public static readonly Rgba32F Azure = 0xF0FFFFFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #F5F5DCFF.
    /// </summary>
    public static readonly Rgba32F Beige = 0xF5F5DCFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFE4C4FF.
    /// </summary>
    public static readonly Rgba32F Bisque = 0xFFE4C4FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #000000FF.
    /// </summary>
    public static readonly Rgba32F Black = 0x000000FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFEBCDFF.
    /// </summary>
    public static readonly Rgba32F BlanchedAlmond = 0xFFEBCDFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #0000FFFF.
    /// </summary>
    public static readonly Rgba32F Blue = 0x0000FFFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #8A2BE2FF.
    /// </summary>
    public static readonly Rgba32F BlueViolet = 0x8A2BE2FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #A52A2AFF.
    /// </summary>
    public static readonly Rgba32F Brown = 0xA52A2AFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #DEB887FF.
    /// </summary>
    public static readonly Rgba32F BurlyWood = 0xDEB887FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #5F9EA0FF.
    /// </summary>
    public static readonly Rgba32F CadetBlue = 0x5F9EA0FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #7FFF00FF.
    /// </summary>
    public static readonly Rgba32F Chartreuse = 0x7FFF00FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #D2691EFF.
    /// </summary>
    public static readonly Rgba32F Chocolate = 0xD2691EFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FF7F50FF.
    /// </summary>
    public static readonly Rgba32F Coral = 0xFF7F50FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #6495EDFF.
    /// </summary>
    public static readonly Rgba32F CornflowerBlue = 0x6495EDFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFF8DCFF.
    /// </summary>
    public static readonly Rgba32F Cornsilk = 0xFFF8DCFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #DC143CFF.
    /// </summary>
    public static readonly Rgba32F Crimson = 0xDC143CFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #00FFFFFF.
    /// </summary>
    public static readonly Rgba32F Cyan = 0x00FFFFFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #00008BFF.
    /// </summary>
    public static readonly Rgba32F DarkBlue = 0x00008BFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #008B8BFF.
    /// </summary>
    public static readonly Rgba32F DarkCyan = 0x008B8BFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #B8860BFF.
    /// </summary>
    public static readonly Rgba32F DarkGoldenrod = 0xB8860BFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #A9A9A9FF.
    /// </summary>
    public static readonly Rgba32F DarkGray = 0xA9A9A9FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #006400FF.
    /// </summary>
    public static readonly Rgba32F DarkGreen = 0x006400FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #A9A9A9FF.
    /// </summary>
    public static readonly Rgba32F DarkGrey = 0xA9A9A9FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #BDB76BFF.
    /// </summary>
    public static readonly Rgba32F DarkKhaki = 0xBDB76BFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #8B008BFF.
    /// </summary>
    public static readonly Rgba32F DarkMagenta = 0x8B008BFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #556B2FFF.
    /// </summary>
    public static readonly Rgba32F DarkOliveGreen = 0x556B2FFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FF8C00FF.
    /// </summary>
    public static readonly Rgba32F DarkOrange = 0xFF8C00FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #9932CCFF.
    /// </summary>
    public static readonly Rgba32F DarkOrchid = 0x9932CCFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #8B0000FF.
    /// </summary>
    public static readonly Rgba32F DarkRed = 0x8B0000FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #E9967AFF.
    /// </summary>
    public static readonly Rgba32F DarkSalmon = 0xE9967AFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #8FBC8FFF.
    /// </summary>
    public static readonly Rgba32F DarkSeaGreen = 0x8FBC8FFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #483D8BFF.
    /// </summary>
    public static readonly Rgba32F DarkStateBlue = 0x483D8BFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #2F4F4FFF.
    /// </summary>
    public static readonly Rgba32F DarkStateGray = 0x2F4F4FFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #2F4F4FFF.
    /// </summary>
    public static readonly Rgba32F DarkStateGrey = 0x2F4F4FFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #00CED1FF.
    /// </summary>
    public static readonly Rgba32F DarkTurquoise = 0x00CED1FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #9400D3FF.
    /// </summary>
    public static readonly Rgba32F DarkViolet = 0x9400D3FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FF1493FF.
    /// </summary>
    public static readonly Rgba32F DeepPink = 0xFF1493FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #00BFFFFF.
    /// </summary>
    public static readonly Rgba32F DeepSkyBlue = 0x00BFFFFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #696969FF.
    /// </summary>
    public static readonly Rgba32F DimGray = 0x696969FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #696969FF.
    /// </summary>
    public static readonly Rgba32F DimGrey = 0x696969FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #1E90FFFF.
    /// </summary>
    public static readonly Rgba32F DodgerBlue = 0x1E90FFFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #B22222FF.
    /// </summary>
    public static readonly Rgba32F Firebrick = 0xB22222FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFFAF0FF.
    /// </summary>
    public static readonly Rgba32F FloralWhite = 0xFFFAF0FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #228B22FF.
    /// </summary>
    public static readonly Rgba32F ForestGreen = 0x228B22FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FF00FFFF.
    /// </summary>
    public static readonly Rgba32F Fuchsia = 0xFF00FFFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #DCDCDCFF.
    /// </summary>
    public static readonly Rgba32F Gainsboro = 0xDCDCDCFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #F8F8FFFF.
    /// </summary>
    public static readonly Rgba32F GhostWhite = 0xF8F8FFFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFD700FF.
    /// </summary>
    public static readonly Rgba32F Gold = 0xFFD700FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #DAA520FF.
    /// </summary>
    public static readonly Rgba32F Goldenrod = 0xDAA520FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #808080FF.
    /// </summary>
    public static readonly Rgba32F Gray = 0x808080FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #008000FF.
    /// </summary>
    public static readonly Rgba32F Green = 0x008000FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #ADFF2FFF.
    /// </summary>
    public static readonly Rgba32F GreenYellow = 0xADFF2FFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #808080FF.
    /// </summary>
    public static readonly Rgba32F Grey = 0x808080FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #F0FFF0FF.
    /// </summary>
    public static readonly Rgba32F Honeydew = 0xF0FFF0FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FF69B4FF.
    /// </summary>
    public static readonly Rgba32F HotPink = 0xFF69B4FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #CD5C5CFF.
    /// </summary>
    public static readonly Rgba32F IndianRed = 0xCD5C5CFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #4B0082FF.
    /// </summary>
    public static readonly Rgba32F Indigo = 0x4B0082FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFFFF0FF.
    /// </summary>
    public static readonly Rgba32F Ivory = 0xFFFFF0FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #F0E68CFF.
    /// </summary>
    public static readonly Rgba32F Khaki = 0xF0E68CFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #E6E6FAFF.
    /// </summary>
    public static readonly Rgba32F Lavender = 0xE6E6FAFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFF0F5FF.
    /// </summary>
    public static readonly Rgba32F LavenderBlush = 0xFFF0F5FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #7CFC00FF.
    /// </summary>
    public static readonly Rgba32F LawnGreen = 0x7CFC00FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFFACDFF.
    /// </summary>
    public static readonly Rgba32F LemonChiffon = 0xFFFACDFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #ADD8E6FF.
    /// </summary>
    public static readonly Rgba32F LightBlue = 0xADD8E6FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #F08080FF.
    /// </summary>
    public static readonly Rgba32F LightCoral = 0xF08080FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #E0FFFFFF.
    /// </summary>
    public static readonly Rgba32F LightCyan = 0xE0FFFFFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FAFAD2FF.
    /// </summary>
    public static readonly Rgba32F LightGoldenrodYellow = 0xFAFAD2FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #D3D3D3FF.
    /// </summary>
    public static readonly Rgba32F LightGray = 0xD3D3D3FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #90EE90FF.
    /// </summary>
    public static readonly Rgba32F LightGreen = 0x90EE90FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #D3D3D3FF.
    /// </summary>
    public static readonly Rgba32F LightGrey = 0xD3D3D3FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFB6C1FF.
    /// </summary>
    public static readonly Rgba32F LightPink = 0xFFB6C1FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFA07AFF.
    /// </summary>
    public static readonly Rgba32F LightSalmon = 0xFFA07AFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #20B2AAFF.
    /// </summary>
    public static readonly Rgba32F LightSeaGreen = 0x20B2AAFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #87CEFAFF.
    /// </summary>
    public static readonly Rgba32F LightSkyBlue = 0x87CEFAFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #778899FF.
    /// </summary>
    public static readonly Rgba32F LightSlateGray = 0x778899FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #778899FF.
    /// </summary>
    public static readonly Rgba32F LightSlateGrey = 0x778899FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #B0C4DEFF.
    /// </summary>
    public static readonly Rgba32F LightSteelBlue = 0xB0C4DEFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFFFE0FF.
    /// </summary>
    public static readonly Rgba32F LightYellow = 0xFFFFE0FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #00FF00FF.
    /// </summary>
    public static readonly Rgba32F Lime = 0x00FF00FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #32CD32FF.
    /// </summary>
    public static readonly Rgba32F LimeGreen = 0x32CD32FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FAF0E6FF.
    /// </summary>
    public static readonly Rgba32F Linen = 0xFAF0E6FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FF00FFFF.
    /// </summary>
    public static readonly Rgba32F Magenta = 0xFF00FFFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #800000FF.
    /// </summary>
    public static readonly Rgba32F Maroon = 0x800000FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #66CDAAFF.
    /// </summary>
    public static readonly Rgba32F MediumAquamarine = 0x66CDAAFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #0000CDFF.
    /// </summary>
    public static readonly Rgba32F MediumBlue = 0x0000CDFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #BA55D3FF.
    /// </summary>
    public static readonly Rgba32F MediumOrchid = 0xBA55D3FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #9370DBFF.
    /// </summary>
    public static readonly Rgba32F MediumPurple = 0x9370DBFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #3CB371FF.
    /// </summary>
    public static readonly Rgba32F MediumSeaGreen = 0x3CB371FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #7B68EEFF.
    /// </summary>
    public static readonly Rgba32F MediumStateBlue = 0x7B68EEFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #00FA9AFF.
    /// </summary>
    public static readonly Rgba32F MediumSpringGreen = 0x00FA9AFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #48D1CCFF.
    /// </summary>
    public static readonly Rgba32F MediumTurquoise = 0x48D1CCFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #C71585FF.
    /// </summary>
    public static readonly Rgba32F MediumVioletRed = 0xC71585FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #191970FF.
    /// </summary>
    public static readonly Rgba32F MidnightBlue = 0x191970FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #F5FFFAFF.
    /// </summary>
    public static readonly Rgba32F MintCream = 0xF5FFFAFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFE4E1FF.
    /// </summary>
    public static readonly Rgba32F MistyRose = 0xFFE4E1FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFE4B5FF.
    /// </summary>
    public static readonly Rgba32F Moccasin = 0xFFE4B5FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFDEADFF.
    /// </summary>
    public static readonly Rgba32F NavajoWhite = 0xFFDEADFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #000080FF.
    /// </summary>
    public static readonly Rgba32F Navy = 0x000080FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FDF5E6FF.
    /// </summary>
    public static readonly Rgba32F OldLace = 0xFDF5E6FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #808000FF.
    /// </summary>
    public static readonly Rgba32F Olive = 0x808000FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #6B8E23FF.
    /// </summary>
    public static readonly Rgba32F OliveDrab = 0x6B8E23FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFA500FF.
    /// </summary>
    public static readonly Rgba32F Orange = 0xFFA500FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FF4500FF.
    /// </summary>
    public static readonly Rgba32F OrangeRed = 0xFF4500FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #DA70D6FF.
    /// </summary>
    public static readonly Rgba32F Orchid = 0xDA70D6FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #EEE8AAFF.
    /// </summary>
    public static readonly Rgba32F PaleGoldenrod = 0xEEE8AAFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #98FB98FF.
    /// </summary>
    public static readonly Rgba32F PaleGreen = 0x98FB98FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #AFEEEEFF.
    /// </summary>
    public static readonly Rgba32F PaleTurquoise = 0xAFEEEEFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #DB7093FF.
    /// </summary>
    public static readonly Rgba32F PaleVioletRed = 0xDB7093FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFEFD5FF.
    /// </summary>
    public static readonly Rgba32F PapayaWhip = 0xFFEFD5FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFDAB9FF.
    /// </summary>
    public static readonly Rgba32F PeachPuff = 0xFFDAB9FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #CD853FFF.
    /// </summary>
    public static readonly Rgba32F Peru = 0xCD853FFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFC0CBFF.
    /// </summary>
    public static readonly Rgba32F Pink = 0xFFC0CBFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #DDA0DDFF.
    /// </summary>
    public static readonly Rgba32F Plum = 0xDDA0DDFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #B0E0E6FF.
    /// </summary>
    public static readonly Rgba32F PowderBlue = 0xB0E0E6FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #800080FF.
    /// </summary>
    public static readonly Rgba32F Purple = 0x800080FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FF0000FF.
    /// </summary>
    public static readonly Rgba32F Red = 0xFF0000FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #BC8F8FFF.
    /// </summary>
    public static readonly Rgba32F RosyBrown = 0xBC8F8FFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #4169E1FF.
    /// </summary>
    public static readonly Rgba32F RoyalBlue = 0x4169E1FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #8B4513FF.
    /// </summary>
    public static readonly Rgba32F SaddleBrown = 0x8B4513FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FA8072FF.
    /// </summary>
    public static readonly Rgba32F Salmon = 0xFA8072FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #F4A460FF.
    /// </summary>
    public static readonly Rgba32F SandyBrown = 0xF4A460FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #2E8B57FF.
    /// </summary>
    public static readonly Rgba32F SeaGreen = 0x2E8B57FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFF5EEFF.
    /// </summary>
    public static readonly Rgba32F SeaShell = 0xFFF5EEFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #A0522DFF.
    /// </summary>
    public static readonly Rgba32F Sienna = 0xA0522DFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #C0C0C0FF.
    /// </summary>
    public static readonly Rgba32F Silver = 0xC0C0C0FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #87CEEBFF.
    /// </summary>
    public static readonly Rgba32F SkyBlue = 0x87CEEBFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #6A5ACDFF.
    /// </summary>
    public static readonly Rgba32F SlateBlue = 0x6A5ACDFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #708090FF.
    /// </summary>
    public static readonly Rgba32F SlateGray = 0x708090FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #708090FF.
    /// </summary>
    public static readonly Rgba32F SlateGrey = 0x708090FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFFAFAFF.
    /// </summary>
    public static readonly Rgba32F Snow = 0xFFFAFAFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #00FF7FFF.
    /// </summary>
    public static readonly Rgba32F SpringGreen = 0x00FF7FFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #4682B4FF.
    /// </summary>
    public static readonly Rgba32F SteelBlue = 0x4682B4FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #D2B48CFF.
    /// </summary>
    public static readonly Rgba32F Tan = 0xD2B48CFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #008080FF.
    /// </summary>
    public static readonly Rgba32F Teal = 0x008080FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #D8BFD8FF.
    /// </summary>
    public static readonly Rgba32F Thistle = 0xD8BFD8FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FF6347FF.
    /// </summary>
    public static readonly Rgba32F Tomato = 0xFF6347FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #40E0D0FF.
    /// </summary>
    public static readonly Rgba32F Turquoise = 0x40E0D0FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #EE82EEFF.
    /// </summary>
    public static readonly Rgba32F Violet = 0xEE82EEFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #F5DEB3FF.
    /// </summary>
    public static readonly Rgba32F Wheat = 0xF5DEB3FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFFFFFFF.
    /// </summary>
    public static readonly Rgba32F White = 0xFFFFFFFF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #F5F5F5FF.
    /// </summary>
    public static readonly Rgba32F WhiteSmoke = 0xF5F5F5FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #FFFF00FF.
    /// </summary>
    public static readonly Rgba32F Yellow = 0xFFFF00FF;

    /// <summary>
    ///     Gets a <see cref="Rgba32F" /> that has a RGBA value of #9ACD32FF.
    /// </summary>
    public static readonly Rgba32F YellowGreen = 0x9ACD32FF;
}
