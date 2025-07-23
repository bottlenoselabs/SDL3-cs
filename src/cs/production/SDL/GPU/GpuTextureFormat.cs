// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

// ReSharper disable InconsistentNaming
#pragma warning disable CA1707

/// <summary>
///     Defines how the elements of a <see cref="GpuTexture" /> (texels) are encoded and organized.
/// </summary>
[PublicAPI]
public enum GpuTextureFormat
{
    /// <summary>
    ///     Invalid texture format.
    /// </summary>
    Invalid = 0,

    /// <summary>
    ///     TODO.
    /// </summary>
    A8_UNORM = 1,

    /// <summary>
    ///     TODO.
    /// </summary>
    R8_UNORM = 2,

    /// <summary>
    ///     TODO.
    /// </summary>
    R8G8_UNORM = 3,

    /// <summary>
    ///     TODO.
    /// </summary>
    R8G8B8A8_UNORM = 4,

    /// <summary>
    ///     TODO.
    /// </summary>
    R16_UNORM = 5,

    /// <summary>
    ///     TODO.
    /// </summary>
    R16G16_UNORM = 6,

    /// <summary>
    ///     TODO.
    /// </summary>
    R16G16B16A16_UNORM = 7,

    /// <summary>
    ///     TODO.
    /// </summary>
    R10G10B10A2_UNORM = 8,

    /// <summary>
    ///     TODO.
    /// </summary>
    B5G6R5_UNORM = 9,

    /// <summary>
    ///     TODO.
    /// </summary>
    B5G5R5A1_UNORM = 10,

    /// <summary>
    ///     TODO.
    /// </summary>
    B4G4R4A4_UNORM = 11,

    /// <summary>
    ///     TODO.
    /// </summary>
    B8G8R8A8_UNORM = 12,

    /// <summary>
    ///     TODO.
    /// </summary>
    BC1_RGBA_UNORM = 13,

    /// <summary>
    ///     TODO.
    /// </summary>
    BC2_RGBA_UNORM = 14,

    /// <summary>
    ///     TODO.
    /// </summary>
    BC3_RGBA_UNORM = 15,

    /// <summary>
    ///     TODO.
    /// </summary>
    BC4_R_UNORM = 16,

    /// <summary>
    ///     TODO.
    /// </summary>
    BC5_RG_UNORM = 17,

    /// <summary>
    ///     TODO.
    /// </summary>
    BC7_RGBA_UNORM = 18,

    /// <summary>
    ///     TODO.
    /// </summary>
    BC6H_RGB_FLOAT = 19,

    /// <summary>
    ///     TODO.
    /// </summary>
    BC6H_RGB_UFLOAT = 20,

    /// <summary>
    ///     TODO.
    /// </summary>
    R8_SNORM = 21,

    /// <summary>
    ///     TODO.
    /// </summary>
    R8G8_SNORM = 22,

    /// <summary>
    ///     TODO.
    /// </summary>
    R8G8B8A8_SNORM = 23,

    /// <summary>
    ///     TODO.
    /// </summary>
    R16_SNORM = 24,

    /// <summary>
    ///     TODO.
    /// </summary>
    R16G16_SNORM = 25,

    /// <summary>
    ///     TODO.
    /// </summary>
    R16G16B16A16_SNORM = 26,

    /// <summary>
    ///     TODO.
    /// </summary>
    R16_FLOAT = 27,

    /// <summary>
    ///     TODO.
    /// </summary>
    R16G16_FLOAT = 28,

    /// <summary>
    ///     TODO.
    /// </summary>
    R16G16B16A16_FLOAT = 29,

    /// <summary>
    ///     TODO.
    /// </summary>
    R32_FLOAT = 30,

    /// <summary>
    ///     TODO.
    /// </summary>
    R32G32_FLOAT = 31,

    /// <summary>
    ///     TODO.
    /// </summary>
    R32G32B32A32_FLOAT = 32,

    /// <summary>
    ///     TODO.
    /// </summary>
    R11G11B10_UFLOAT = 33,

    /// <summary>
    ///     TODO.
    /// </summary>
    R8_UINT = 34,

    /// <summary>
    ///     TODO.
    /// </summary>
    R8G8_UINT = 35,

    /// <summary>
    ///     TODO.
    /// </summary>
    R8G8B8A8_UINT = 36,

    /// <summary>
    ///     TODO.
    /// </summary>
    R16_UINT = 37,

    /// <summary>
    ///     TODO.
    /// </summary>
    R16G16_UINT = 38,

    /// <summary>
    ///     TODO.
    /// </summary>
    R16G16B16A16_UINT = 39,

    /// <summary>
    ///     TODO.
    /// </summary>
    R32_UINT = 40,

    /// <summary>
    ///     TODO.
    /// </summary>
    R32G32_UINT = 41,

    /// <summary>
    ///     TODO.
    /// </summary>
    R32G32B32A32_UINT = 42,

    /// <summary>
    ///     TODO.
    /// </summary>
    R8_INT = 43,

    /// <summary>
    ///     TODO.
    /// </summary>
    R8G8_INT = 44,

    /// <summary>
    ///     TODO.
    /// </summary>
    R8G8B8A8_INT = 45,

    /// <summary>
    ///     TODO.
    /// </summary>
    R16_INT = 46,

    /// <summary>
    ///     TODO.
    /// </summary>
    R16G16_INT = 47,

    /// <summary>
    ///     TODO.
    /// </summary>
    R16G16B16A16_INT = 48,

    /// <summary>
    ///     TODO.
    /// </summary>
    R32_INT = 49,

    /// <summary>
    ///     TODO.
    /// </summary>
    R32G32_INT = 50,

    /// <summary>
    ///     TODO.
    /// </summary>
    R32G32B32A32_INT = 51,

    /// <summary>
    ///     TODO.
    /// </summary>
    R8G8B8A8_UNORM_SRGB = 52,

    /// <summary>
    ///     TODO.
    /// </summary>
    B8G8R8A8_UNORM_SRGB = 53,

    /// <summary>
    ///     TODO.
    /// </summary>
    BC1_RGBA_UNORM_SRGB = 54,

    /// <summary>
    ///     TODO.
    /// </summary>
    BC2_RGBA_UNORM_SRGB = 55,

    /// <summary>
    ///     TODO.
    /// </summary>
    BC3_RGBA_UNORM_SRGB = 56,

    /// <summary>
    ///     TODO.
    /// </summary>
    BC7_RGBA_UNORM_SRGB = 57,

    /// <summary>
    ///     TODO.
    /// </summary>
    D16_UNORM = 58,

    /// <summary>
    ///     TODO.
    /// </summary>
    D24_UNORM = 59,

    /// <summary>
    ///     TODO.
    /// </summary>
    D32_FLOAT = 60,

    /// <summary>
    ///     TODO.
    /// </summary>
    D24_UNORM_S8_UINT = 61,

    /// <summary>
    ///     TODO.
    /// </summary>
    D32_FLOAT_S8_UINT = 62,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_4x4_UNORM = 63,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_5x4_UNORM = 64,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_5x5_UNORM = 65,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_6x5_UNORM = 66,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_6x6_UNORM = 67,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_8x5_UNORM = 68,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_8x6_UNORM = 69,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_8x8_UNORM = 70,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_10x5_UNORM = 71,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_10x6_UNORM = 72,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_10x8_UNORM = 73,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_10x10_UNORM = 74,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_12x10_UNORM = 75,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_12x12_UNORM = 76,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_4x4_UNORM_SRGB = 77,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_5x4_UNORM_SRGB = 78,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_5x5_UNORM_SRGB = 79,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_6x5_UNORM_SRGB = 80,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_6x6_UNORM_SRGB = 81,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_8x5_UNORM_SRGB = 82,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_8x6_UNORM_SRGB = 83,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_8x8_UNORM_SRGB = 84,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_10x5_UNORM_SRGB = 85,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_10x6_UNORM_SRGB = 86,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_10x8_UNORM_SRGB = 87,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_10x10_UNORM_SRGB = 88,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_12x10_UNORM_SRGB = 89,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_12x12_UNORM_SRGB = 90,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_4x4_FLOAT = 91,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_5x4_FLOAT = 92,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_5x5_FLOAT = 93,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_6x5_FLOAT = 94,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_6x6_FLOAT = 95,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_8x5_FLOAT = 96,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_8x6_FLOAT = 97,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_8x8_FLOAT = 98,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_10x5_FLOAT = 99,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_10x6_FLOAT = 100,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_10x8_FLOAT = 101,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_10x10_FLOAT = 102,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_12x10_FLOAT = 103,

    /// <summary>
    ///     TODO.
    /// </summary>
    ASTC_12x12_FLOAT = 104
}
