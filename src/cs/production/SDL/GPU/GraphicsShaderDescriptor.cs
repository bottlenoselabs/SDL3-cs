// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Parameters for creating a <see cref="GraphicsShader" /> instance.
/// </summary>
[PublicAPI]
public class GraphicsShaderDescriptor : Descriptor
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
    ///     Gets or sets the <see cref="GraphicsShaderStage" /> of the shader.
    /// </summary>
    public GraphicsShaderStage Stage { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="GraphicsShaderFormats" /> of the shader.
    /// </summary>
    public GraphicsShaderFormats Format { get; set; }

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
    ///     Initializes a new instance of the <see cref="GraphicsShaderDescriptor" /> class.
    /// </summary>
    /// <param name="allocator">
    ///     The allocator to use for temporary interoperability allocations. If <c>null</c>, a
    ///     <see cref="ArenaNativeAllocator" /> is used with a capacity of 1 KB.
    /// </param>
    public GraphicsShaderDescriptor(INativeAllocator? allocator = null)
        : base(allocator)
    {
    }

    /// <summary>
    ///     Creates a new instance of <see cref="GraphicsShaderDescriptor" /> struct using a specified file path to a shader
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
        Stage = GraphicsShaderStage.Fragment;
        Format = GraphicsShaderFormats.None;
        SamplerCount = 0;
        UniformBufferCount = 0;
        StorageBufferCount = 0;
        StorageTextureCount = 0;
    }

    private static string? TryGetEntryPoint(GraphicsShaderFormats formatSingle)
    {
        switch (formatSingle)
        {
            case GraphicsShaderFormats.SPIRV:
            case GraphicsShaderFormats.DXIL:
            case GraphicsShaderFormats.DXBC:
                return "main";
            case GraphicsShaderFormats.MSL:
            case GraphicsShaderFormats.MetalLib:
                return "main0";
            default:
                return null;
        }
    }

    private static GraphicsShaderStage? TryGetShaderStageFromFileName(string fileName)
    {
        // NOTE: Auto-detect the shader stage from the file name for convenience
        if (fileName.Contains(".vert.", StringComparison.CurrentCultureIgnoreCase))
        {
            return GraphicsShaderStage.Vertex;
        }

        if (fileName.Contains(".frag.", StringComparison.CurrentCultureIgnoreCase))
        {
            return GraphicsShaderStage.Fragment;
        }

        return null;
    }

    private static GraphicsShaderFormats? TryGetFormatSingleFromFileName(string fileName)
    {
        // NOTE: Auto-detect the shader format from the file name for convenience
        if (fileName.EndsWith(".spv", StringComparison.InvariantCultureIgnoreCase))
        {
            return GraphicsShaderFormats.SPIRV;
        }

        if (fileName.EndsWith(".dxil", StringComparison.InvariantCultureIgnoreCase))
        {
            return GraphicsShaderFormats.DXIL;
        }

        if (fileName.EndsWith(".msl", StringComparison.InvariantCultureIgnoreCase))
        {
            return GraphicsShaderFormats.MSL;
        }

        return null;
    }

    private static GraphicsShaderFormats? TryGetFormatSingle(GraphicsShaderFormats format)
    {
        if (format == GraphicsShaderFormats.None)
        {
            return null;
        }

        // NOTE: We want multiple driver / preferred formats to be checked before others.
        if ((format & GraphicsShaderFormats.SPIRV) != 0)
        {
            return GraphicsShaderFormats.SPIRV;
        }

        if ((format & GraphicsShaderFormats.MetalLib) != 0)
        {
            return GraphicsShaderFormats.MetalLib;
        }

        if ((format & GraphicsShaderFormats.MSL) != 0)
        {
            return GraphicsShaderFormats.MSL;
        }

        if ((format & GraphicsShaderFormats.DXIL) != 0)
        {
            return GraphicsShaderFormats.DXIL;
        }

        if ((format & GraphicsShaderFormats.DXBC) != 0)
        {
            return GraphicsShaderFormats.DXBC;
        }

        return null;
    }
}
