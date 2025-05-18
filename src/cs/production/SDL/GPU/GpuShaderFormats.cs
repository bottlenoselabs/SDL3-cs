// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

// ReSharper disable InconsistentNaming

namespace SDL.GPU;

/// <summary>
///     Defines the data formats (byte code or source code) available for graphics or compute shaders.
/// </summary>
/// <remarks>
///     <para>Each format corresponds to a specific <see cref="GpuDriver" /> that accepts it.</para>
/// </remarks>
[PublicAPI]
[Flags]
public enum GpuShaderFormats
{
    /// <summary>
    ///     Shader data format is not specified. Defaults to the available shader formats for the current operating
    ///     system.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Shader data format is specific to a platform/driver behind a non-disclosure agreement (NDA).
    /// </summary>
    Private = 1 << 0,

    /// <summary>
    ///     Shader data format is binary pre-compiled Standard Portable Intermediate Representation - Vulkan (SPIR-V).
    ///     This format is specific to <see cref="GpuDriver.Vulkan" />.
    /// </summary>
    /// <remarks>
    ///     <para><see cref="SPIRV" /> usually has the file extension '.spv'.</para>
    /// </remarks>
    SPIRV = 1 << 1,

    /// <summary>
    ///     Shader data format is binary pre-compiled DirectX 12 Byte Code (DXBC) using Shader Model 5.1 (SM5_1).
    ///     This format is specific to <see cref="GpuDriver.DirectX12" />.
    /// </summary>
    /// <remarks>
    ///     <para><see cref="DXBC" /> usually has the file extension '.dxbc'.</para>
    /// </remarks>
    DXBC = 1 << 2,

    /// <summary>
    ///     Shader data format is binary pre-compiled DirectX 12 Intermediate Language (DXIL) using Shader Model 6.0 (SM6_0).
    ///     This format is specific to <see cref="GpuDriver.DirectX12" />.
    /// </summary>
    /// <remarks>
    ///     <para><see cref="DXIL" /> usually has the file extension '.dxil'.</para>
    /// </remarks>
    DXIL = 1 << 3,

    /// <summary>
    ///     Shader data format is text Metal Shader Language (MSL).
    ///     This format is specific to <see cref="GpuDriver.Metal" />.
    /// </summary>
    /// <remarks>
    ///     <para><see cref="MSL" /> usually has the file extension '.msl'.</para>
    /// </remarks>
    MSL = 1 << 4,

    /// <summary>
    ///     Shader data format is binary pre-compiled Metal Library.
    ///     This format is specific to <see cref="GpuDriver.Metal" />.
    /// </summary>
    /// <remarks>
    ///     <para><see cref="MetalLib" /> usually has the file extension '.metallib'.</para>
    /// </remarks>
    MetalLib = 1 << 5
}
