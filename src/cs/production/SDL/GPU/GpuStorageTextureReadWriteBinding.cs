// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

#pragma warning disable CA1815
#pragma warning disable SA1623

/// <summary>
///     TODO.
/// </summary>
[PublicAPI]
public struct GpuStorageTextureReadWriteBinding
{
    /// <summary>
    ///     TODO.
    /// </summary>
    public GpuTexture Texture { get; set; }

    /// <summary>
    ///     TODO.
    /// </summary>
    public uint MipLevel { get; set; }

    /// <summary>
    ///     TODO.
    /// </summary>
    public uint Layer { get; set; }

    /// <summary>
    ///     TODO.
    /// </summary>
    public bool Cycle { get; set; }
}
