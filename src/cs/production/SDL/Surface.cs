// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Represents a two-dimensional array of pixels.
/// </summary>
[PublicAPI]
public sealed unsafe class Surface : NativeHandleTyped<SDL_Surface>
{
    /// <summary>
    ///     Gets the width of the surface.
    /// </summary>
    public int Width => HandleTyped == null ? 0 : HandleTyped->w;

    /// <summary>
    ///     Gets the height of the surface.
    /// </summary>
    public int Height => HandleTyped == null ? 0 : HandleTyped->h;

    /// <summary>
    ///     Gets the pixel format of the surface.
    /// </summary>
    public PixelFormat PixelFormat => HandleTyped == null ? 0 : (PixelFormat)HandleTyped->format;

    /// <summary>
    ///     Gets the pointer to the surface's raw pixel data.
    /// </summary>
    public IntPtr DataPointer => HandleTyped == null ? IntPtr.Zero : (IntPtr)HandleTyped->pixels;

    /// <summary>
    ///     Gets or sets the transparent color of the surface.
    /// </summary>
    public Rgb8U ColorKey
    {
        get => GetColorKey();
        set => SetColorKey(value);
    }

    /// <summary>
    ///     Gets or sets the clip rectangle of the surface.
    /// </summary>
    public Rectangle ClipRectangle
    {
        get
        {
            GetClipRectangle(out var rect);
            return rect;
        }
        set => SetClipRectangle(value);
    }

    internal Surface(SDL_Surface* handle)
        : base(handle)
    {
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
        var mappedColor = SDL_MapSurfaceRGB(HandleTyped, color.R, color.G, color.B);
        return mappedColor;
    }

    /// <summary>
    ///     Blit the current surface to the destination surface.
    /// </summary>
    /// <param name="destinationSurface">The destination surface.</param>
    /// <param name="scaleMode">
    ///     The surface scale mode to use. If <c>null</c>, no scaling is performed.
    /// </param>
    public void BlitTo(Surface destinationSurface, ScaleMode? scaleMode = null)
    {
        var src = HandleTyped;
        var dst = destinationSurface.HandleTyped;
        var srcRect = (SDL_Rect*)null;
        var dstRect = (SDL_Rect*)null;

        if (scaleMode == null)
        {
            var isSuccess = SDL_BlitSurface(src, srcRect, dst, dstRect);
            if (!isSuccess)
            {
                Error.NativeFunctionFailed(nameof(SDL_BlitSurface));
            }
        }
        else
        {
            var isSuccess = SDL_BlitSurfaceScaled(
                src, srcRect, dst, dstRect, (SDL_ScaleMode)scaleMode.Value);
            if (!isSuccess)
            {
                Error.NativeFunctionFailed(nameof(SDL_BlitSurfaceScaled));
            }
        }
    }

    /// <summary>
    ///     Attempts to fast copy the current surface to the destination surface.
    /// </summary>
    /// <param name="destinationSurface">The destination surface.</param>
    /// <param name="sourceRectangle">
    ///     The source rectangle. If the copy was successful, <paramref name="sourceRectangle" /> is
    ///     set with the final source rectangle after clipping is performed.
    /// </param>
    /// <param name="destinationRectangle">
    ///     The destination rectangle. Only <see cref="Rectangle.X" /> and <see cref="Rectangle.Y" /> are used as input
    ///     because the width and height are copied from <paramref name="sourceRectangle" />. If the copy was
    ///     successful, <paramref name="destinationRectangle" /> is set with the final destination rectangle after
    ///     clipping is performed.
    /// </param>
    /// <param name="scaleMode">
    ///     The surface scale mode to use. If <c>null</c>, no scaling is performed.
    /// </param>
    /// <returns><c>true</c> if the surface was successfully copied; otherwise, <c>false</c>.</returns>
    public bool BlitTo(
        Surface destinationSurface,
        ref Rectangle sourceRectangle,
        ref Rectangle destinationRectangle,
        ScaleMode? scaleMode = null)
    {
        var src = HandleTyped;
        var dst = destinationSurface.HandleTyped;
        var srcRect = (SDL_Rect*)Unsafe.AsPointer(ref sourceRectangle);
        var dstRect = (SDL_Rect*)Unsafe.AsPointer(ref destinationRectangle);

        bool isSuccess;
        if (scaleMode == null)
        {
            isSuccess = SDL_BlitSurface(src, srcRect, dst, dstRect);
            if (isSuccess)
            {
                Error.NativeFunctionFailed(nameof(SDL_BlitSurface));
            }
        }
        else
        {
            isSuccess = SDL_BlitSurfaceScaled(
                src, srcRect, dst, dstRect, (SDL_ScaleMode)scaleMode.Value);
            if (isSuccess)
            {
                Error.NativeFunctionFailed(nameof(SDL_BlitSurfaceScaled));
            }
        }

        if (isSuccess)
        {
            sourceRectangle = Unsafe.AsRef<Rectangle>(srcRect);
            destinationRectangle = Unsafe.AsRef<Rectangle>(dstRect);
        }

        return isSuccess;
    }

    /// <summary>
    ///     Color fills the entire surface.
    /// </summary>
    /// <param name="pixelColor">The pixel format color to fill the surface.</param>
    /// <remarks>
    ///     <para>
    ///         If there is a clip rectangle set via <see cref="SetClipRectangle" />, then
    ///         <see cref="Fill" /> will fill based on that clip rectangle.
    ///     </para>
    /// </remarks>
    public void Fill(uint pixelColor)
    {
        var isSuccess = SDL_FillSurfaceRect(HandleTyped, null, pixelColor);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_FillSurfaceRect), isExceptionThrown: true);
        }
    }

    /// <summary>
    ///     Color fills a rectangle area of the surface.
    /// </summary>
    /// <param name="rectangle">The rectangle area to fill.</param>
    /// <param name="pixelColor">The pixel format color to fill the surface.</param>
    /// <remarks>
    ///     <para>
    ///         If there is a clip rectangle set via <see cref="ClipRectangle" />, then
    ///         <see cref="FillRectangle" /> will fill based on the intersection of the clip rectangle and
    ///         <paramref name="rectangle" />.
    ///     </para>
    /// </remarks>
    public void FillRectangle(in Rectangle rectangle, uint pixelColor)
    {
        fixed (Rectangle* rectanglePointer = &rectangle)
        {
            var rectanglePointer2 = (SDL_Rect*)rectanglePointer;
            var isSuccess = SDL_FillSurfaceRect(HandleTyped, rectanglePointer2, pixelColor);
            if (!isSuccess)
            {
                Error.NativeFunctionFailed(nameof(SDL_FillSurfaceRect), isExceptionThrown: true);
            }
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        SDL_DestroySurface(HandleTyped);
        base.Dispose(isDisposing);
    }

    private void GetClipRectangle(out Rectangle rectangle)
    {
        fixed (Rectangle* rectanglePointer = &rectangle)
        {
            var rectanglePointer2 = (SDL_Rect*)rectanglePointer;
            var isSuccess = SDL_GetSurfaceClipRect(HandleTyped, rectanglePointer2);
            if (!isSuccess)
            {
                Error.NativeFunctionFailed(nameof(SDL_SetSurfaceClipRect), isExceptionThrown: true);
            }
        }
    }

    private void SetClipRectangle(in Rectangle rectangle)
    {
        fixed (Rectangle* rectanglePointer = &rectangle)
        {
            var rectanglePointer2 = (SDL_Rect*)rectanglePointer;
            var isSuccess = SDL_SetSurfaceClipRect(HandleTyped, rectanglePointer2);
            if (!isSuccess)
            {
                Error.NativeFunctionFailed(nameof(SDL_SetSurfaceClipRect), isExceptionThrown: true);
            }
        }
    }

    private Rgb8U GetColorKey()
    {
        var hasColorKey = SDL_SurfaceHasColorKey(HandleTyped);
        if (!hasColorKey)
        {
            return Rgb8U.Transparent;
        }

        uint colorKeyMapped;
        var isSuccess = SDL_GetSurfaceColorKey(HandleTyped, &colorKeyMapped);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_GetSurfaceColorKey), isExceptionThrown: true);
        }

        var pixelFormat = (SDL_PixelFormat)PixelFormat;
        var pixelFormatDetails = SDL_GetPixelFormatDetails(pixelFormat);
        if (pixelFormatDetails == null)
        {
            Error.NativeFunctionFailed(nameof(SDL_GetPixelFormatDetails), isExceptionThrown: true);
        }

        byte r;
        byte g;
        byte b;
        SDL_GetRGB(colorKeyMapped, pixelFormatDetails, null, &r, &g, &b);

        var color = new Rgb8U(r, g, b);
        return color;
    }

    private void SetColorKey(Rgb8U value)
    {
        var colorKeyMapped = MapRgb(value);
        var isSuccess = SDL_SetSurfaceColorKey(HandleTyped, true, colorKeyMapped);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_SetSurfaceColorKey), isExceptionThrown: true);
        }
    }
}
