// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using File = SDL.IO.File;

namespace SDL.GPU;

/// <summary>
///     Parameters for creating a <see cref="GpuGraphicsShader" />.
/// </summary>
[PublicAPI]
public class GpuGraphicsShaderOptions : BaseOptions
{
    /// <summary>
    ///     Gets or sets the data byte pointer of the shader's code.
    /// </summary>
    public IntPtr DataPointer { get; set; }

    /// <summary>
    ///     Gets or sets the data byte size of the shader's code.
    /// </summary>
    public int DataSize { get; set; }

    /// <summary>
    ///     Gets or sets the function name of the shader's entry point.
    /// </summary>
    public string? EntryPoint { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="GpuGraphicsShaderStage" /> of the shader.
    /// </summary>
    public GpuGraphicsShaderStage Stage { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="GpuShaderFormats" /> of the shader.
    /// </summary>
    public GpuShaderFormats Format { get; set; }

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
    ///     Creates a new instance of <see cref="GpuGraphicsShaderOptions" /> struct using a specified file path to a shader
    ///     file to load data from.
    /// </summary>
    /// <param name="file">The file of the shader.</param>
    /// <returns>
    ///     <c>true</c> if the descriptor was successfully set using the <paramref name="file" />; otherwise,
    ///     <c>false</c>.
    /// </returns>
    /// <exception cref="InvalidOperationException"><paramref name="file" /> has no data.</exception>
    public bool TrySetFromFile(File file)
    {
        if (!file.HasData)
        {
            throw new InvalidOperationException("File has no data.");
        }

        var fileName = Path.GetFileName(file.FilePath!);
        var format = TryGetFormatSingleFromFileName(fileName);
        if (format == null)
        {
            return false;
        }

        var stage = TryGetShaderStageFromFileName(fileName);
        if (stage == null)
        {
            return false;
        }

        var entryPoint = TryGetEntryPoint(format.Value);
        if (entryPoint == null)
        {
            return false;
        }

        Format = format.Value;
        Stage = stage.Value;
        EntryPoint = entryPoint;
        DataPointer = file.Data;
        DataSize = file.Size;

        return true;
    }

    /// <inheritdoc />
    protected override void OnReset()
    {
        DataPointer = IntPtr.Zero;
        DataSize = 0;
        EntryPoint = null;
        Stage = GpuGraphicsShaderStage.Fragment;
        Format = GpuShaderFormats.None;
        SamplerCount = 0;
        UniformBufferCount = 0;
        StorageBufferCount = 0;
        StorageTextureCount = 0;
    }

    private static string? TryGetEntryPoint(GpuShaderFormats formatSingle)
    {
        switch (formatSingle)
        {
            case GpuShaderFormats.SPIRV:
            case GpuShaderFormats.DXIL:
            case GpuShaderFormats.DXBC:
                return "main";
            case GpuShaderFormats.MSL:
            case GpuShaderFormats.MetalLib:
                return "main0";
            default:
                return null;
        }
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

    private static GpuShaderFormats? TryGetFormatSingleFromFileName(string fileName)
    {
        // NOTE: Auto-detect the shader format from the file name for convenience
        if (fileName.EndsWith(".spv", StringComparison.InvariantCultureIgnoreCase))
        {
            return GpuShaderFormats.SPIRV;
        }

        if (fileName.EndsWith(".dxil", StringComparison.InvariantCultureIgnoreCase))
        {
            return GpuShaderFormats.DXIL;
        }

        if (fileName.EndsWith(".msl", StringComparison.InvariantCultureIgnoreCase))
        {
            return GpuShaderFormats.MSL;
        }

        return null;
    }

    private static GpuShaderFormats? TryGetFormatSingle(GpuShaderFormats format)
    {
        if (format == GpuShaderFormats.None)
        {
            return null;
        }

        // NOTE: We want multiple driver / preferred formats to be checked before others.
        if ((format & GpuShaderFormats.SPIRV) != 0)
        {
            return GpuShaderFormats.SPIRV;
        }

        if ((format & GpuShaderFormats.MetalLib) != 0)
        {
            return GpuShaderFormats.MetalLib;
        }

        if ((format & GpuShaderFormats.MSL) != 0)
        {
            return GpuShaderFormats.MSL;
        }

        if ((format & GpuShaderFormats.DXIL) != 0)
        {
            return GpuShaderFormats.DXIL;
        }

        if ((format & GpuShaderFormats.DXBC) != 0)
        {
            return GpuShaderFormats.DXBC;
        }

        return null;
    }
}
