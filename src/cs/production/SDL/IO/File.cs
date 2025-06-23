// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL.IO;

/// <summary>
///     Represents a file using SDL.
/// </summary>
[PublicAPI]
public unsafe class File : Poolable<File>
{
    private string? _filePath;
    private IntPtr _dataPointer;
    private int _dataSize;

    private bool _hasData;

    /// <summary>
    ///     Gets the <see cref="FileSystem" /> instance associated with the file.
    /// </summary>
    public FileSystem FileSystem { get; }

    /// <summary>
    ///     Gets the file path of the file.
    /// </summary>
    public string? FilePath => _filePath;

    /// <summary>
    ///     Gets a value indicating whether the file has loaded data.
    /// </summary>
    public bool HasData => _dataPointer != IntPtr.Zero;

    /// <summary>
    ///     Gets the pointer to the loaded data of the file.
    /// </summary>
    public IntPtr Data => _dataPointer;

    /// <summary>
    ///     Gets the size of the file's loaded  data.
    /// </summary>
    public int Size => _dataSize;

    internal File(FileSystem fileSystem)
    {
        FileSystem = fileSystem;
    }

    internal void Set(string filePath, IntPtr dataPointer, int dataSize)
    {
        _filePath = filePath;
        _dataPointer = dataPointer;
        _dataSize = dataSize;
        _hasData = true;
    }

    /// <inheritdoc />
    protected override void Reset()
    {
        var hasData = Interlocked.CompareExchange(ref _hasData, true, false);
        if (hasData)
        {
            SDL_free((void*)_dataPointer);
            _dataPointer = IntPtr.Zero;
        }

        _dataSize = 0;
        _filePath = null;
    }
}
