// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Parameters for creating a <see cref="GpuComputeShader" />.
/// </summary>
[PublicAPI]
public class GpuComputeShaderOptions : GpuBaseShaderOptions
{
    /// <summary>
    ///     Gets or sets the number of samplers.
    /// </summary>
    public int SamplerCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of read-only storage textures.
    /// </summary>
    public int ReadOnlyStorageTexturesCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of read-only storage buffers.
    /// </summary>
    public int ReadOnlyStorageBuffersCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of read-write storage textures.
    /// </summary>
    public int ReadWriteStorageTexturesCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of read-write storage buffers.
    /// </summary>
    public int ReadWriteStorageBuffersCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of uniform buffers.
    /// </summary>
    public int UniformBuffersCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of threads in the X dimension.
    /// </summary>
    public int ThreadsXCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of threads in the Y dimension.
    /// </summary>
    public int ThreadsYCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of threads in the Z dimension.
    /// </summary>
    public int ThreadsZCount { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GpuComputeShaderOptions" /> class.
    /// </summary>
    /// <param name="allocator">
    ///     The allocator to use for temporary interoperability allocations. If <c>null</c>, a
    ///     <see cref="ArenaNativeAllocator" /> is used with a capacity of 1 KB.
    /// </param>
    public GpuComputeShaderOptions(INativeAllocator? allocator = null)
        : base(allocator)
    {
    }

    /// <inheritdoc />
    protected override void OnReset()
    {
        base.OnReset();

        SamplerCount = 0;
        ReadOnlyStorageTexturesCount = 0;
        ReadOnlyStorageBuffersCount = 0;
        ReadWriteStorageTexturesCount = 0;
        ReadWriteStorageBuffersCount = 0;
        UniformBuffersCount = 0;
        ThreadsXCount = 0;
        ThreadsYCount = 0;
        ThreadsZCount = 0;
    }
}
