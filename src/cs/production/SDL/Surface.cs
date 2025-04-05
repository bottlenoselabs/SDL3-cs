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

    /// <summary>
    ///     Maps an RGB color to the closest surface's color pixel format.
    /// </summary>
    /// <param name="color">The color to map.</param>
    /// <returns>A <see cref="Rgb8U" /> mapped to the surface's color pixel format.</returns>
    /// <remarks>
    ///     <para>
    ///         If the surface has a palette, the index of the closest matching color in the palette will be
    ///         returned.
    ///     </para>
    /// </remarks>
    public uint MapRgb(Rgb8U color)
    {
        var mappedColor = SDL_MapSurfaceRGB(_handle, color.R, color.G, color.B);
        return mappedColor;
    }

    /// <summary>
    ///     Attempts to color fill the entire surface.
    /// </summary>
    /// <param name="pixelColor">The pixel format color to fill the surface.</param>
    /// <returns><c>true</c> if the surface was successfully filled; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     <para>
    ///         If there is a clip rectangle set via <see cref="TrySetClipRectangle" />, then
    ///         <see cref="TryFillRectangle" /> will fill based on that clip rectangle.
    ///     </para>
    /// </remarks>
    public bool TryFill(uint pixelColor)
    {
        var result = SDL_FillSurfaceRect(_handle, null, pixelColor);
        return result;
    }

    /// <summary>
    ///     Attempts to color fill a rectangle area of the surface.
    /// </summary>
    /// <param name="rectangle">The rectangle area to fill.</param>
    /// <param name="pixelColor">The pixel format color to fill the surface.</param>
    /// <returns><c>true</c> if the surface was successfully filled; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     <para>
    ///         If there is a clip rectangle set via <see cref="TrySetClipRectangle" />, then
    ///         <see cref="TryFillRectangle" /> will fill based on the intersection of the clip rectangle and
    ///         <paramref name="rectangle" />.
    ///     </para>
    /// </remarks>
    public bool TryFillRectangle(in Rectangle rectangle, uint pixelColor)
    {
        fixed (Rectangle* rectanglePointer = &rectangle)
        {
            var rectanglePointer2 = (SDL_Rect*)rectanglePointer;
            var result = SDL_FillSurfaceRect(_handle, rectanglePointer2, pixelColor);
            return result;
        }
    }

    /// <summary>
    ///     Attempts to set the clipping rectangle of the surface.
    /// </summary>
    /// <param name="rectangle">The clipping rectangle.</param>
    /// <returns><c>true</c> if the clipping rectangle was successful set for the surface; otherwise, <c>false</c>.</returns>
    public bool TrySetClipRectangle(in Rectangle rectangle)
    {
        fixed (Rectangle* rectanglePointer = &rectangle)
        {
            var rectanglePointer2 = (SDL_Rect*)rectanglePointer;
            var result = SDL_SetSurfaceClipRect(_handle, rectanglePointer2);
            return result;
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        SDL_DestroySurface(_handle);
        _handle = null;
        base.Dispose(isDisposing);
    }
}
