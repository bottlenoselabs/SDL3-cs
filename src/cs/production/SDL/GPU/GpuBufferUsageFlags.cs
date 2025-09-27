// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Specifies how a buffer is intended to be used by the client.
/// </summary>
[Flags]
public enum GpuBufferUsageFlags
{
    /// <summary>
    ///     Buffer is a vertex buffer.
    /// </summary>
    Vertex = 1 << 0,

    /// <summary>
    ///     Buffer is an index buffer.
    /// </summary>
    Index = 1 << 1,

    /// <summary>
    ///     Buffer is an indirect buffer.
    /// </summary>
    Indirect = 1 << 2,

    /// <summary>
    ///     Buffer supports storage reads in graphics stages.
    /// </summary>
    GraphicsStorageRead = 1 << 3,

    /// <summary>
    ///     Buffer supports storage reads in the compute stage.
    /// </summary>
    ComputeStorageRead = 1 << 4,

    /// <summary>
    ///     Buffer supports storage writes in the compute stage.
    /// </summary>
    ComputeStorageWrite = 1 << 5
}
