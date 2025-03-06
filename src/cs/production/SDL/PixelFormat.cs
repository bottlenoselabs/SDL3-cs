// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL;

// ReSharper disable IdentifierTypo

/// <summary>
///     TODO.
/// </summary>
[PublicAPI]
public enum PixelFormat
{
    /// <summary>
    ///  TODO.
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///  TODO.
    /// </summary>
    Index1Lsb = 286261504,

    /// <summary>
    ///     TODO.
    /// </summary>
    Index1Msb = 287310080,

    /// <summary>
    ///     TODO.
    /// </summary>
    Index2Lsb = 470811136,

    /// <summary>
    ///     TODO.
    /// </summary>
    Index2Msb = 471859712,

    /// <summary>
    ///     TODO.
    /// </summary>
    Index4Lsb = 303039488,

    /// <summary>
    ///     TODO.
    /// </summary>
    Index4Msb = 304088064,

    /// <summary>
    ///     TODO.
    /// </summary>
    Index8 = 318769153,

    /// <summary>
    ///     TODO.
    /// </summary>
    Rgb332 = 336660481,

    /// <summary>
    ///     TODO.
    /// </summary>
    Xrgb4444 = 353504258,

    /// <summary>
    ///     TODO.
    /// </summary>
    Xbgr4444 = 357698562,

    /// <summary>
    ///     TODO.
    /// </summary>
    Xrgb1555 = 353570562,

    /// <summary>
    ///     TODO.
    /// </summary>
    Xbgr1555 = 357764866,

    /// <summary>
    ///     TODO.
    /// </summary>
    Argb4444 = 355602434,

    /// <summary>
    ///     TODO.
    /// </summary>
    RrgbA4444 = 356651010,

    /// <summary>
    ///     TODO.
    /// </summary>
    Abgr4444 = 359796738,

    /// <summary>
    ///     TODO.
    /// </summary>
    Bgra4444 = 360845314,

    /// <summary>
    ///     TODO.
    /// </summary>
    Argb1555 = 355667970,

    /// <summary>
    ///     TODO.
    /// </summary>
    Rgba5551 = 356782082,

    /// <summary>
    ///     TODO.
    /// </summary>
    Abgr1555 = 359862274,

    /// <summary>
    ///     TODO.
    /// </summary>
    Bgra5551 = 360976386,

    /// <summary>
    ///     TODO.
    /// </summary>
    Rgb565 = 353701890,

    /// <summary>
    ///     TODO.
    /// </summary>
    Bgr565 = 357896194,

    /// <summary>
    ///     TODO.
    /// </summary>
    Rgb24 = 386930691,

    /// <summary>
    ///     TODO.
    /// </summary>
    Bgr24 = 390076419,

    /// <summary>
    ///     TODO.
    /// </summary>
    Xrgb8888 = 370546692,

    /// <summary>
    ///     TODO.
    /// </summary>
    Rgbx8888 = 371595268,

    /// <summary>
    ///     TODO.
    /// </summary>
    Xbgr8888 = 374740996,

    /// <summary>
    ///     TODO.
    /// </summary>
    Bgrx8888 = 375789572,

    /// <summary>
    ///     TODO.
    /// </summary>
    Argb8888 = 372645892,

    /// <summary>
    ///     TODO.
    /// </summary>
    Rgba8888 = 373694468,

    /// <summary>
    ///     TODO.
    /// </summary>
    Abgr8888 = 376840196,

    /// <summary>
    ///     TODO.
    /// </summary>
    Bgra8888 = 377888772,

    /// <summary>
    ///     TODO.
    /// </summary>
    Xrgb2101010 = 370614276,

    /// <summary>
    ///     TODO.
    /// </summary>
    Xbgr2101010 = 374808580,

    /// <summary>
    ///     TODO.
    /// </summary>
    Argb2101010 = 372711428,

    /// <summary>
    ///     TODO.
    /// </summary>
    Abgr2101010 = 376905732,

    /// <summary>
    ///     TODO.
    /// </summary>
    Rgb48 = 403714054,

    /// <summary>
    ///     TODO.
    /// </summary>
    Bgr48 = 406859782,

    /// <summary>
    ///     TODO.
    /// </summary>
    Rgba64 = 404766728,

    /// <summary>
    ///     TODO.
    /// </summary>
    Argb64 = 405815304,

    /// <summary>
    ///     TODO.
    /// </summary>
    Bgra64 = 407912456,

    /// <summary>
    ///     TODO.
    /// </summary>
    Abgr64 = 408961032,

    /// <summary>
    ///     TODO.
    /// </summary>
    Rgb48Float = 437268486,

    /// <summary>
    ///     TODO.
    /// </summary>
    Bgr48Float = 440414214,

    /// <summary>
    ///     TODO.
    /// </summary>
    Rgba64Float = 438321160,

    /// <summary>
    ///     TODO.
    /// </summary>
    Argb64Float = 439369736,

    /// <summary>
    ///     TODO.
    /// </summary>
    Bgra64Float = 441466888,

    /// <summary>
    ///     TODO.
    /// </summary>
    Abgr64Float = 442515464,

    /// <summary>
    ///     TODO.
    /// </summary>
    Rgb96Float = 454057996,

    /// <summary>
    ///     TODO.
    /// </summary>
    Bgr96Float = 457203724,

    /// <summary>
    ///     TODO.
    /// </summary>
    Rgba128Float = 455114768,

    /// <summary>
    ///     TODO.
    /// </summary>
    Argb128Float = 456163344,

    /// <summary>
    ///     TODO.
    /// </summary>
    Bgra128Float = 458260496,

    /// <summary>
    ///     TODO.
    /// </summary>
    Abgr128Float = 459309072,

    /// <summary>
    ///     TODO.
    /// </summary>
    Yv12 = 842094169,

    /// <summary>
    ///     TODO.
    /// </summary>
    Iyuv = 1448433993,

    /// <summary>
    ///     TODO.
    /// </summary>
    Yuy2 = 844715353,

    /// <summary>
    ///     TODO.
    /// </summary>
    Uyvy = 1498831189,

    /// <summary>
    ///     TODO.
    /// </summary>
    Yvyu = 1431918169,

    /// <summary>
    ///     TODO.
    /// </summary>
    Nv12 = 842094158,

    /// <summary>
    ///     TODO.
    /// </summary>
    Nv21 = 825382478,

    /// <summary>
    ///     TODO.
    /// </summary>
    P010 = 808530000,

    /// <summary>
    ///     TODO.
    /// </summary>
    ExternalOes = 542328143,

    /// <summary>
    ///     TODO.
    /// </summary>
    Mjpg = 1196444237
}
