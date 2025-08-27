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
    public int ReadOnlyStorageTextureCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of read-only storage buffers.
    /// </summary>
    public int ReadOnlyStorageBuffersCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of read-write storage textures.
    /// </summary>
    public int ReadWriteStorageTextureCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of read-write storage buffers.
    /// </summary>
    public int ReadWriteStorageBufferCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of uniform buffers.
    /// </summary>
    public int UniformBufferCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of threads in the X dimension.
    /// </summary>
    public int ThreadXCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of threads in the Y dimension.
    /// </summary>
    public int ThreadYCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of threads in the Z dimension.
    /// </summary>
    public int ThreadZCount { get; set; }

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

    /// <summary>
    ///     Sets the properties of a <see cref="GpuComputeShaderOptions" /> instance using a specified loaded file path
    ///     of a compute shader file.
    /// </summary>
    /// <param name="file">The loaded file of the compute shader.</param>
    /// <returns>
    ///     <c>true</c> if the options was successfully set using the <paramref name="file" />; otherwise,
    ///     <c>false</c>.
    /// </returns>
    /// <exception cref="InvalidOperationException"><paramref name="file" /> has no data.</exception>
    public override bool TrySetFromFile(File file)
    {
        if (!base.TrySetFromFile(file))
        {
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    protected override void OnReset()
    {
        base.OnReset();

        SamplerCount = 0;
        ReadOnlyStorageTextureCount = 0;
        ReadOnlyStorageBuffersCount = 0;
        ReadWriteStorageTextureCount = 0;
        ReadWriteStorageBufferCount = 0;
        UniformBufferCount = 0;
        ThreadXCount = 0;
        ThreadYCount = 0;
        ThreadZCount = 0;
    }
}
