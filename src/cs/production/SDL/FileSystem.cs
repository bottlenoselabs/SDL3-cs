// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL;

/// <summary>
///     Represents a file system using SDL.
/// </summary>
/// <remarks>
///     <para>
///         It's recommended to use <see cref="FileSystem" /> instead of .NET's APIs for IO because
///         <see cref="FileSystem" /> uses SDL internally which is guaranteed to be work across platforms.
///     </para>
/// </remarks>
public sealed unsafe partial class FileSystem : Disposable
{
    private readonly ArenaNativeAllocator _allocator;

    private readonly ILogger<FileSystem> _logger;
    private readonly Pool<File> _poolFile;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FileSystem" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="temporaryBufferSize">The size in bytes of the buffer used for temporary allocations.</param>
    public FileSystem(
        ILogger<FileSystem> logger,
        int temporaryBufferSize = 1024)
    {
        _logger = logger;
        _allocator = new ArenaNativeAllocator(temporaryBufferSize);
        _poolFile = new Pool<File>(logger, () => new File(this), "Files");
    }

    /// <summary>
    ///     Attempts to to load a file given a specified file path.
    /// </summary>
    /// <param name="filePath">
    ///     The path to the file. If the path is relative, it is assumed to be relative to
    ///     <see cref="AppContext.BaseDirectory" />.
    /// </param>
    /// <param name="file">The resulting file if successfully loaded; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the file was successful loaded; otherwise, <c>false</c>.</returns>
    public bool TryLoadFile(string filePath, out File file)
    {
        ulong dataSize;
        _allocator.Reset();
        var filePathC = _allocator.AllocateCString(filePath);
        var dataPointer = SDL_LoadFile(filePathC, &dataSize);
        _allocator.Reset();
        if (dataPointer == null)
        {
            var errorMessage = Error.GetMessage();
            LogNativeFunctionFailed(nameof(SDL_LoadFile), errorMessage);
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
    ///     Attempts to load a file given the a specified file path as an image.
    /// </summary>
    /// <param name="filePath">
    ///     The path to the file. If the path is relative, it is assumed to be relative to
    ///     <see cref="AppContext.BaseDirectory" />.
    /// </param>
    /// <param name="surface">The resulting surface if successfully loaded; otherwise, <c>null</c>.</param>
    /// <param name="desiredPixelFormat">
    ///     The desired pixel format of the image. If the image after loaded is not already in this pixel format, the
    ///     image will attempt to be converted to this pixel format.
    /// </param>
    /// <returns><c>true</c> if the image was successful loaded; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentException">Invalid <paramref name="desiredPixelFormat" />.</exception>
    public bool TryLoadImage(
        string filePath, out Surface? surface, PixelFormat? desiredPixelFormat = null)
    {
        if (desiredPixelFormat == PixelFormat.Unknown)
        {
            throw new ArgumentException("Invalid desired pixel format.");
        }

        _allocator.Reset();
        var filePathC = _allocator.AllocateCString(filePath);
        var surfaceHandle = SDL_image.IMG_Load(filePathC);
        _allocator.Reset();
        if (surfaceHandle == null)
        {
            var errorMessage = Error.GetMessage();
            LogNativeFunctionFailed(nameof(SDL_image.IMG_Load), errorMessage);
            surface = null;
            return false;
        }

        var desiredPixelFormat2 = (SDL_PixelFormat)(desiredPixelFormat ?? PixelFormat.Unknown);
        if (desiredPixelFormat != null && surfaceHandle->format != desiredPixelFormat2)
        {
            var convertedSurfaceHandle = SDL_ConvertSurface(surfaceHandle, desiredPixelFormat2);
            if (convertedSurfaceHandle == null)
            {
                var errorMessage = Error.GetMessage();
                LogNativeFunctionFailed(nameof(SDL_ConvertSurface), errorMessage);
                surface = null;
                return false;
            }

            SDL_DestroySurface(surfaceHandle);
            surfaceHandle = convertedSurfaceHandle;
        }

        surface = new Surface((IntPtr)surfaceHandle);
        return true;
    }

    [LoggerMessage(LogEventId.NativeFunctionFailed, LogLevel.Error, "Native function failed: {FunctionName}. {ErrorMessage}")]
    internal partial void LogNativeFunctionFailed(string functionName, string errorMessage);

    /// <inheritdoc/>
    protected override void Dispose(bool isDisposing)
    {
        _poolFile.Dispose();
        _allocator.Dispose();
    }
}
