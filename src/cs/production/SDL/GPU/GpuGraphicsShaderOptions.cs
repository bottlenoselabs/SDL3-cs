// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Parameters for creating a <see cref="GpuGraphicsShader" />.
/// </summary>
[PublicAPI]
public class GpuGraphicsShaderOptions : GpuBaseShaderOptions
{
    /// <summary>
    ///     Gets or sets the <see cref="GpuGraphicsShaderStage" /> of the shader.
    /// </summary>
    public GpuGraphicsShaderStage Stage { get; set; }

    /// <summary>
    ///     Gets or sets the number of samplers used in the shader.
    /// </summary>
    public int SamplerCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of uniform buffers used in the shader.
    /// </summary>
    public int UniformBufferCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of storage buffers used in the shader.
    /// </summary>
    public int StorageBufferCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of storage textures used in the shader.
    /// </summary>
    public int StorageTextureCount { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GpuGraphicsShaderOptions" /> class.
    /// </summary>
    /// <param name="allocator">
    ///     The allocator to use for temporary interoperability allocations. If <c>null</c>, a
    ///     <see cref="ArenaNativeAllocator" /> is used with a capacity of 1 KB.
    /// </param>
    public GpuGraphicsShaderOptions(INativeAllocator? allocator = null)
        : base(allocator)
    {
    }

    /// <summary>
    ///     Sets the properties of a <see cref="GpuGraphicsShaderOptions" /> instance using a specified loaded file path
    ///     of a graphics shader file.
    /// </summary>
    /// <param name="file">The loaded file of the graphics shader.</param>
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

        var fileName = Path.GetFileName(file.FilePath!);
        var stage = TryGetShaderStageFromFileName(fileName);
        if (stage == null)
        {
            return false;
        }

        Stage = stage.Value;

        return true;
    }

    /// <inheritdoc />
    protected override void OnReset()
    {
        base.OnReset();

        Stage = GpuGraphicsShaderStage.Fragment;
        SamplerCount = 0;
        UniformBufferCount = 0;
        StorageBufferCount = 0;
        StorageTextureCount = 0;
    }

    private static GpuGraphicsShaderStage? TryGetShaderStageFromFileName(string fileName)
    {
        // NOTE: Auto-detect the shader stage from the file name for convenience
        if (fileName.Contains(".vert.", StringComparison.CurrentCultureIgnoreCase))
        {
            return GpuGraphicsShaderStage.Vertex;
        }

        if (fileName.Contains(".frag.", StringComparison.CurrentCultureIgnoreCase))
        {
            return GpuGraphicsShaderStage.Fragment;
        }

        return null;
    }
}
