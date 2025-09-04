// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Parameters for creating a <see cref="GpuComputePipeline" />.
/// </summary>
[PublicAPI]
public class GpuComputePipelineOptions : BaseOptions
{
    /// <summary>
    ///     Gets or sets the compute shader.
    /// </summary>
    public GpuShaderFormats? ShaderFormat { get; set; }

    /// <summary>
    ///     Gets or sets the code.
    /// </summary>
    public ImmutableArray<byte>? ShaderCode { get; set; }

    /// <summary>
    ///    Gets or sets the entry point function name.
    /// </summary>
    public string? EntryPoint { get; set; }

    /// <summary>
    ///    Gets or sets the entry point function name.
    /// </summary>
    public int ThreadCountX { get; set; }

    /// <summary>
    ///    Gets or sets the entry point function name.
    /// </summary>
    public int ThreadCountY { get; set; }

    /// <summary>
    ///    Gets or sets the entry point function name.
    /// </summary>
    public int ThreadCountZ { get; set; }

    /// <summary>
    ///    Gets or sets the entry point function name.
    /// </summary>
    public int ReadWriteStorageTexturesCount { get; set; }

    /// <summary>
    ///    Gets or sets the entry point function name.
    /// </summary>
    public int ReadOnlyStorageTexturesCount { get; set; }

    /// <summary>
    ///    Gets or sets the entry point function name.
    /// </summary>
    public int ReadWriteStorageBuffersCount { get; set; }

    /// <summary>
    ///    Gets or sets the entry point function name.
    /// </summary>
    public int ReadOnlyStorageBuffersCount { get; set; }

    /// <summary>
    ///    Gets or sets the entry point function name.
    /// </summary>
    public int SamplersCount { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GpuComputePipelineOptions" /> class.
    /// </summary>
    /// <param name="allocator">
    ///     The allocator to use for temporary interoperability allocations. If <c>null</c>, a
    ///     <see cref="ArenaNativeAllocator" /> is used with a capacity of 1 KB.
    /// </param>
    public GpuComputePipelineOptions(
        INativeAllocator? allocator = null)
        : base(allocator)
    {
    }

    /// <inheritdoc />
    protected override void OnReset()
    {
        ShaderFormat = GpuShaderFormats.None;
        ShaderCode = [];
        EntryPoint = null;
        ThreadCountX = 0;
        ThreadCountY = 0;
        ThreadCountZ = 0;
        ReadWriteStorageTexturesCount = 0;
        ReadOnlyStorageTexturesCount = 0;
        ReadWriteStorageBuffersCount = 0;
        ReadOnlyStorageBuffersCount = 0;
        SamplersCount = 0;
    }
}
