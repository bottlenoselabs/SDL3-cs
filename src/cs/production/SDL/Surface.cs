// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL;

/// <summary>
///     Represents a two-dimensional array of pixels.
/// </summary>
[PublicAPI]
public sealed unsafe class Surface : NativeHandle
{
    private SDL_Surface* _handle;

    /// <summary>
    ///     Gets the width of the surface.
    /// </summary>
    public int Width => _handle == null ? 0 : _handle->w;

    /// <summary>
    ///     Gets the height of the surface.
    /// </summary>
    public int Height => _handle == null ? 0 : _handle->h;

    /// <summary>
    ///     Gets the pointer to the surface's raw pixel data.
    /// </summary>
    public IntPtr DataPointer => _handle == null ? IntPtr.Zero : (IntPtr)_handle->pixels;

    internal Surface(IntPtr handle)
        : base(handle)
    {
        _handle = (SDL_Surface*)handle;
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        SDL_DestroySurface(_handle);
        _handle = null;
        base.Dispose(isDisposing);
    }
}
