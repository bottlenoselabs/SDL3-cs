// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Represents a GPU resource.
/// </summary>
[PublicAPI]
public class GpuResource : NativeHandle
{
    /// <summary>
    ///     Gets the <see cref="GpuDevice" /> instance associated with the GPU resource.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="Device" /> is <c>null</c> when <see cref="Disposable.IsDisposed" /> is <c>true</c>.
    ///     </para>
    /// </remarks>
    public GpuDevice Device { get; private set; }

    internal GpuResource(GpuDevice device, IntPtr handle)
        : base(handle)
    {
        Device = device;
    }
}
