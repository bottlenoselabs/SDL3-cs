// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Parameters for creating a <see cref="GpuDevice" />.
/// </summary>
[PublicAPI]
public sealed class GpuDeviceOptions : BaseOptions
{
    /// <summary>
    ///     Gets or sets the <see cref="GpuShaderFormats" /> to use.
    /// </summary>
    public GpuShaderFormats ShaderFormats { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="GpuDriver" /> to use. If <c>null</c>, the optimal driver for the current
    ///     operating system is automatically selected.
    /// </summary>
    public GpuDriver? Driver { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to enable debug properties and validations.
    /// </summary>
    public bool IsDebugMode { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to prefer energy efficiency over maximum GPU performance.
    /// </summary>
    public bool IsLowPowerMode { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GpuDeviceOptions" /> class.
    /// </summary>
    /// <param name="allocator">
    ///     The allocator to use for temporary interoperability allocations. If <c>null</c>, a
    ///     <see cref="ArenaNativeAllocator" /> is used with a capacity of 1 KB.
    /// </param>
    public GpuDeviceOptions(INativeAllocator? allocator = null)
        : base(allocator)
    {
    }

    /// <inheritdoc/>
    protected override void OnReset()
    {
        ShaderFormats = GpuShaderFormats.None;
        Driver = null;
        IsDebugMode = false;
        IsLowPowerMode = false;
    }
}
