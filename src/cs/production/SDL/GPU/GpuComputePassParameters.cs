// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

#pragma warning disable CA1815

/// <summary>
///     TODO.
/// </summary>
[PublicAPI]
public record struct GpuComputePassParameters
{
    /// <summary>
    ///     The native array of writeable storage texture bindings.
    /// </summary>
    public NativeArray TextureWriteBindingsArray;

    /// <summary>
    ///     The naive array of writeable storage data buffer bindings.
    /// </summary>
    public NativeArray DataBufferWriteBindingsArray;

    /// <summary>
    ///     TODO.
    /// </summary>
    /// <param name="textureWriteBindings">The writable storage texture bindings.</param>
    public void SetBindingsTextureWrite(
        params Span<GpuComputePassBindingTextureReadWrite> textureWriteBindings)
    {
        TextureWriteBindingsArray = NativeArray.CreateFromSpan(textureWriteBindings);
    }

    /// <summary>
    ///     TODO.
    /// </summary>
    /// <param name="dataBufferWriteBindings">The writeable storage data buffer bindings.</param>
    public void SetDataBufferWriteBindings(
        params Span<GpuComputePassBindingDataBufferReadWrite> dataBufferWriteBindings)
    {
        DataBufferWriteBindingsArray = NativeArray.CreateFromSpan(dataBufferWriteBindings);
    }
}
