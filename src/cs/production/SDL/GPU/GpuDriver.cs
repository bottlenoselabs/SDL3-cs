// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Defines the supported native graphics and compute APIs of a <see cref="GpuDevice" /> which are tied to specific
///     vendors.
/// </summary>
[PublicAPI]
public enum GpuDriver
{
    /// <summary>
    ///     The Vulkan graphics and compute API. May be available on Microsoft Windows but generally specific to Linux.
    ///     Also includes Nintendo: Nintendo Switch.
    /// </summary>
    Vulkan,

    /// <summary>
    ///     The Direct3D 12 graphics and compute API. Specific to Microsoft: Windows, Xbox, etc.
    /// </summary>
    DirectX12,

    /// <summary>
    ///     The Metal graphics and compute API. Specific to Apple: macOS, iOS, tvOS, etc.
    /// </summary>
    Metal
}
