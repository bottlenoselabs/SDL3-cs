// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///  TODO.
/// </summary>
[PublicAPI]
public ref struct GpuComputePassParameters
{
    /// <summary>
    ///  TODO.
    /// </summary>
    public Span<GpuComputePassBindingTextureReadWrite> TextureWriteBindings;

    /// <summary>
    ///  TODO.
    /// </summary>
    public Span<GpuComputePassBindingDataBufferReadWrite> DataBufferWriteBindings;
}
