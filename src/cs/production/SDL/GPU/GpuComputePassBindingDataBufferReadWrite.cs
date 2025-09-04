// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     TODO.
/// </summary>
public record struct GpuComputePassBindingDataBufferReadWrite
{
    /// <summary>
    ///     TODO.
    /// </summary>
    public nint Buffer;

    /// <summary>
    ///     TODO.
    /// </summary>
    public bool IsCycled;

    /// <summary>
    ///     TODO.
    /// </summary>
    /// <param name="buffer">The data buffer.</param>
    public unsafe void SetBuffer(GpuDataBuffer buffer)
    {
        Buffer = (IntPtr)buffer.HandleTyped;
    }
}
