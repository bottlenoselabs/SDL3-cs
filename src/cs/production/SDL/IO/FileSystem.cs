// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.IO;

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
        ulong dataSize;
        _filePathAllocator.Reset();
        var filePathC = _filePathAllocator.AllocateCString(filePath);
        var dataPointer = SDL_LoadFile(filePathC, &dataSize);
        _filePathAllocator.Reset();
        if (dataPointer == null)
        {
            Error.NativeFunctionFailed(nameof(SDL_LoadFile));
            file = null!;
            return false;
        }

        var file2 = _poolFile.GetOrCreate();
        if (file2 == null)
        {
            SDL_free(dataPointer);
            file = null!;
            return false;
        }

        file = file2;
        file.Set(filePath, (IntPtr)dataPointer, (int)dataSize);
        return true;
    }

    /// <summary>
    ///     Attempts to load a file as an image given the specified file path.
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
        if (desiredPixelFormat == PixelFormat.Unknown)
        {
            throw new ArgumentException("Invalid desired pixel format.");
        }

        _filePathAllocator.Reset();
        var filePathC = _filePathAllocator.AllocateCString(filePath);
        var surfaceHandle = SDL_image.IMG_Load(filePathC);
        _filePathAllocator.Reset();
        if (surfaceHandle == null)
        {
            Error.NativeFunctionFailed(nameof(SDL_image.IMG_Load));
            surface = null;
            return false;
        }

        var desiredPixelFormat2 = (SDL_PixelFormat)(desiredPixelFormat ?? PixelFormat.Unknown);
        if (desiredPixelFormat != null && surfaceHandle->format != desiredPixelFormat2)
        {
            var convertedSurfaceHandle = SDL_ConvertSurface(surfaceHandle, desiredPixelFormat2);
            if (convertedSurfaceHandle == null)
            {
                Error.NativeFunctionFailed(nameof(SDL_ConvertSurface));
                surface = null;
                return false;
            }

            SDL_DestroySurface(surfaceHandle);
            surfaceHandle = convertedSurfaceHandle;
        }

        surface = new Surface((IntPtr)surfaceHandle);
        return true;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool isDisposing)
    {
        _poolFile.Dispose();
        _filePathAllocator.Dispose();
    }
}
