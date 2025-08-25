// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Represents a file system using SDL.
/// </summary>
/// <remarks>
///     <para>
///         It's recommended to use <see cref="FileSystem" /> instead of .NET's APIs for IO because
///         <see cref="FileSystem" /> uses SDL internally which is guaranteed to be work across SDL platforms.
///     </para>
/// </remarks>
public sealed unsafe class FileSystem : Disposable
{
    private readonly ArenaNativeAllocator _filePathAllocator;
    private readonly Pool<File> _poolFile;

    internal FileSystem(ILogger<FileSystem> logger)
    {
        _filePathAllocator = new ArenaNativeAllocator(1024);
        _poolFile = new Pool<File>(logger, () => new File(this), "Files");
    }

    /// <summary>
    ///     Attempts to load a file given a specified file path.
    /// </summary>
    /// <param name="filePath">
    ///     The path to the file. If the path is relative, it is assumed to be relative to
    ///     <see cref="AppContext.BaseDirectory" />.
    /// </param>
    /// <param name="file">The resulting file if successfully loaded; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the file was successfully loaded; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     <para>
    ///         <see cref="TryLoadFile" /> is not thread safe.
    ///     </para>
    /// </remarks>
    public bool TryLoadFile(string filePath, out File file)
    {
        var fullFilePath = GetFullFilePath(filePath);

        ulong dataSize;
        _filePathAllocator.Reset();
        var fullFilePathC = _filePathAllocator.AllocateCString(fullFilePath);
        var filePointer = SDL_LoadFile(fullFilePathC, &dataSize);
        _filePathAllocator.Reset();
        if (filePointer == null)
        {
            Error.NativeFunctionFailed(nameof(SDL_LoadFile));
            file = null!;
            return false;
        }

        var file2 = _poolFile.GetOrCreate();
        if (file2 == null)
        {
            SDL_free(filePointer);
            file = null!;
            return false;
        }

        file = file2;
        file.Set(fullFilePath, (IntPtr)filePointer, (int)dataSize);
        return true;
    }

    /// <summary>
    ///     Attempts to load a file as an image and create a new <see cref="Surface" /> instance given the specified
    ///     file path.
    /// </summary>
    /// <param name="filePath">
    ///     The path to the file. If the path is relative, it is assumed to be relative to
    ///     <see cref="AppContext.BaseDirectory" />.
    /// </param>
    /// <param name="surface">The resulting surface if successfully loaded; otherwise, <c>null</c>.</param>
    /// <param name="desiredPixelFormat">
    ///     The desired pixel format of the image. If the image after loaded is not already in this pixel format, the
    ///     image will attempt to be converted to this pixel format. Use <c>null</c> to disable conversion and have the
    ///     image in the same pixel format as the file format.
    /// </param>
    /// <returns><c>true</c> if the image was successfully loaded; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentException">Invalid <paramref name="desiredPixelFormat" />.</exception>
    /// <remarks>
    ///     <para>
    ///         <see cref="TryLoadImage" /> is not thread safe.
    ///     </para>
    /// </remarks>
    public bool TryLoadImage(
        string filePath,
        out Surface? surface,
        PixelFormat? desiredPixelFormat = null)
    {
        if (desiredPixelFormat != null &&
            (desiredPixelFormat.Value == PixelFormat.Unknown || !Enum.IsDefined(desiredPixelFormat.Value)))
        {
            throw new ArgumentException("Invalid desired pixel format.");
        }

        var fullFilePath = GetFullFilePath(filePath);

        _filePathAllocator.Reset();
        var fullFilePathC = _filePathAllocator.AllocateCString(fullFilePath);
        var surfacePointer = IMG_Load(fullFilePathC);
        _filePathAllocator.Reset();
        if (surfacePointer == null)
        {
            Error.NativeFunctionFailed(nameof(IMG_Load));
            surface = null;
            return false;
        }

        var desiredPixelFormat2 = (SDL_PixelFormat)(desiredPixelFormat ?? PixelFormat.Unknown);
        if (desiredPixelFormat != null && surfacePointer->format != desiredPixelFormat2)
        {
            var convertedSurfacePointer = SDL_ConvertSurface(surfacePointer, desiredPixelFormat2);
            if (convertedSurfacePointer == null)
            {
                Error.NativeFunctionFailed(nameof(SDL_ConvertSurface));
                surface = null;
                return false;
            }

            SDL_DestroySurface(surfacePointer);
            surfacePointer = convertedSurfacePointer;
        }

        surface = new Surface(surfacePointer);
        return true;
    }

    /// <summary>
    ///     Attempts to load a file as a font and create a new <see cref="Font" /> instance given the specified file
    ///     path and point size.
    /// </summary>
    /// <param name="filePath">
    ///     The path to the file. If the path is relative, it is assumed to be relative to
    ///     <see cref="AppContext.BaseDirectory" />.
    /// </param>
    /// <param name="font">The resulting font if successfully loaded; otherwise, <c>null</c>.</param>
    /// <param name="pointSize">
    ///     The point size of the font. Some fonts will have several sizes embedded in the file so the
    ///     point size becomes the index of choosing the closest size. If the value is too high, the last indexed size
    ///     will be used.
    /// </param>
    /// <returns><c>true</c> if the font was successfully loaded; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     <para>
    ///         <see cref="TryLoadFont" /> is not thread safe.
    ///     </para>
    /// </remarks>
    public bool TryLoadFont(
        string filePath,
        out Font? font,
        float pointSize)
    {
        var fullFilePath = GetFullFilePath(filePath);

        _filePathAllocator.Reset();
        var fullFilePathC = _filePathAllocator.AllocateCString(fullFilePath);
        var fontPointer = TTF_OpenFont(fullFilePathC, pointSize);
        _filePathAllocator.Reset();
        if (fontPointer == null)
        {
            Error.NativeFunctionFailed(nameof(TTF_OpenFont));
            font = null;
            return false;
        }

        font = new Font((IntPtr)fontPointer);
        return true;
    }

    /// <summary>
    ///     Attempts to load a file as a graphics shader and create a new <see cref="GpuGraphicsShader" /> instance
    ///     using the specified file path.
    /// </summary>
    /// <param name="filePath">
    ///     The path to the file. If the path is relative, it is assumed to be relative to
    ///     <see cref="AppContext.BaseDirectory" />.
    /// </param>
    /// <param name="device">The <see cref="GpuDevice" /> instance.</param>
    /// <param name="shader">If successful, a new <see cref="GpuGraphicsShader" /> instance; otherwise, <c>null</c>.</param>
    /// <param name="samplerCount">The number of samplers used in the shader.</param>
    /// <param name="uniformBufferCount">The number of uniform buffers used in the shader.</param>
    /// <returns><c>true</c> if the shader was successfully created; otherwise, <c>false</c>.</returns>
    public bool TryLoadGraphicsShader(
        string filePath,
        GpuDevice device,
        out GpuGraphicsShader? shader,
        int samplerCount = 0,
        int uniformBufferCount = 0)
    {
        if (!TryLoadFile(filePath, out var file))
        {
            shader = null;
            return false;
        }

        using var options = new GpuGraphicsShaderOptions();
        options.SamplerCount = samplerCount;
        options.UniformBufferCount = uniformBufferCount;
        if (!options.TrySetFromFile(file))
        {
            shader = null;
            file.Dispose();
            return false;
        }

        if (!device.TryCreateGraphicsShader(options, out shader))
        {
            shader = null;
            file.Dispose();
            return false;
        }

        file.Dispose();
        return true;
    }

    /// <summary>
    ///     Attempts to load a file as a compute shader and create a new <see cref="GpuComputeShader" /> instance
    ///     using the specified file path.
    /// </summary>
    /// <param name="filePath">
    ///     The path to the file. If the path is relative, it is assumed to be relative to
    ///     <see cref="AppContext.BaseDirectory" />.
    /// </param>
    /// <param name="device">The <see cref="GpuDevice" /> instance.</param>
    /// <param name="shader">If successful, a new <see cref="GpuComputeShader" /> instance; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the shader was successfully created; otherwise, <c>false</c>.</returns>
    public bool TryLoadComputeShader(
        string filePath,
        GpuDevice device,
        out GpuComputeShader? shader)
    {
        if (!TryLoadFile(filePath, out var file))
        {
            shader = null;
            return false;
        }

        using var options = new GpuComputeShaderOptions();
        if (!options.TrySetFromFile(file))
        {
            shader = null;
            file.Dispose();
            return false;
        }

        if (!device.TryCreateComputeShader(options, out shader))
        {
            shader = null;
            file.Dispose();
            return false;
        }

        file.Dispose();
        return true;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool isDisposing)
    {
        _poolFile.Dispose();
        _filePathAllocator.Dispose();
    }

    private static string GetFullFilePath(string filePath)
    {
        var fullFilePath = filePath;
        if (!Path.IsPathRooted(filePath))
        {
            fullFilePath = Path.Combine(AppContext.BaseDirectory, filePath);
        }

        return fullFilePath;
    }
}
