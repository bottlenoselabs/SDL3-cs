// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Represents a context for rendering 2D graphics.
/// </summary>
[PublicAPI]
public sealed unsafe class Renderer2D : NativeHandle
{
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once MemberCanBePrivate.Global
    internal SDL_Renderer* _handle;

    /// <summary>
    ///     Gets or sets the color used for rendering operations such as <see cref="RenderRectangle" />,
    ///     <see cref="RenderLine" />, and <see cref="Clear" />.
    /// </summary>
    public Rgba8U DrawColor
    {
        get => GetDrawColor();
        set => SetDrawColor(value);
    }

    /// <summary>
    ///     Gets or sets the viewport of the current render target.
    /// </summary>
    public Rectangle Viewport
    {
        get => GetViewport();
        set => SetViewport(value);
    }

    /// <summary>
    ///     Gets the current <see cref="bottlenoselabs.SDL.VSyncMode" />.
    /// </summary>
    public VSyncMode VSyncMode => GetVSyncMode();

    internal Renderer2D(IntPtr handle)
        : base(handle)
    {
        _handle = (SDL_Renderer*)Handle;
    }

    /// <summary>
    ///     Create a <see cref="Texture" /> from a <see cref="Surface" />.
    /// </summary>
    /// <param name="surface">The surface.</param>
    /// <returns>A new <see cref="Texture" /> instance.</returns>
    /// <exception cref="NativeFunctionFailedException">Native function failed.</exception>
    /// <remarks>
    ///     <para>
    ///         The surface is not modified or freed by <see cref="CreateTextureFrom" />.
    ///     </para>
    ///     <para>
    ///         <see cref="CreateTextureFrom" /> should only be called from the main thread.
    ///     </para>
    /// </remarks>
    public Texture CreateTextureFrom(Surface surface)
    {
        var textureHandle = SDL_CreateTextureFromSurface(
            (SDL_Renderer*)Handle, (SDL_Surface*)surface.Handle);
        if (textureHandle == null)
        {
            Error.NativeFunctionFailed(nameof(SDL_CreateTextureFromSurface), true);
        }

        var texture = new Texture((IntPtr)textureHandle);
        return texture;
    }

    /// <summary>
    ///     Clears the current entire rendering target with the <see cref="DrawColor" /> ignoring the viewport and the
    ///     clip rectangle.
    /// </summary>
    public void Clear()
    {
        SDL_RenderClear(_handle);
    }

    /// <summary>
    ///     Updates the screen with any rendering performed.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Call <see cref="Present" /> once a frame to present the back buffer to the screen. Rendering is
    ///         always performed on the back buffer first.
    ///     </para>
    ///     <para>
    ///         The back buffer should be considered invalidated after calling <see cref="Present" />. It is strongly
    ///         encouraged to call <see cref="Clear" /> to initialize the back buffer before any rendering operations
    ///         even if you plan to overwrite every pixel.
    ///     </para>
    ///     <para>
    ///         When rendering to a <see cref="Texture" />, there is no need to call <see cref="Present" /> and it
    ///         should not be called because textures don't have a back buffer. Calling <see cref="Present" /> is for
    ///         only updating the screen.
    ///     </para>
    /// </remarks>
    public void Present()
    {
        var isSuccess = SDL_RenderPresent(_handle);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_RenderPresent), isExceptionThrown: true);
        }
    }

    /// <summary>
    ///     Renders a <see cref="Texture" /> to the current rendering target.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="sourceRectangle">
    ///     The rectangle area of the texture to render. Use <c>null</c> for the entire
    ///     texture.
    /// </param>
    /// <param name="destinationRectangle">
    ///     The rectangle area of the render target to render the texture to. Use <c>null</c>
    ///     for the entire render target.
    /// </param>
    public void RenderTexture(
        Texture texture,
        RectangleF? sourceRectangle = null,
        RectangleF? destinationRectangle = null)
    {
        SDL_FRect* src = null;
        SDL_FRect* dest = null;

        if (sourceRectangle != null)
        {
            var rect = sourceRectangle.Value;
            var srcRect = default(SDL_FRect);
            srcRect.x = rect.X;
            srcRect.y = rect.Y;
            srcRect.w = rect.Width;
            srcRect.h = rect.Height;
            src = &srcRect;
        }

        if (destinationRectangle != null)
        {
            var rect = destinationRectangle.Value;
            var destRect = default(SDL_FRect);
            destRect.x = rect.X;
            destRect.y = rect.Y;
            destRect.w = rect.Width;
            destRect.h = rect.Height;
            dest = &destRect;
        }

        var isSuccess = SDL_RenderTexture(_handle, texture._handle, src, dest);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_RenderTexture), isExceptionThrown: true);
        }
    }

    /// <summary>
    ///     Renders a filled rectangle to the current render target using the <see cref="DrawColor" />.
    /// </summary>
    /// <param name="rectangle">The rectangle area. Use <c>null</c> for the entire render target.</param>
    public void RenderRectangleFill(RectangleF? rectangle = null)
    {
        SDL_FRect* rect = null;

        if (rectangle != null)
        {
            var rectangle1 = rectangle.Value;
            var rect1 = default(SDL_FRect);
            rect1.x = rectangle1.X;
            rect1.y = rectangle1.Y;
            rect1.w = rectangle1.Width;
            rect1.h = rectangle1.Height;
            rect = &rect1;
        }

        var isSuccess = SDL_RenderFillRect(_handle, rect);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_RenderFillRect), isExceptionThrown: true);
        }
    }

    /// <summary>
    ///     Renders an outlined rectangle to the current render target using the <see cref="DrawColor" />.
    /// </summary>
    /// <param name="rectangle">The rectangle area. Use <c>null</c> for the entire render target.</param>
    public void RenderRectangle(RectangleF? rectangle = null)
    {
        SDL_FRect* rect = null;

        if (rectangle != null)
        {
            var rectangle1 = rectangle.Value;
            var rect1 = default(SDL_FRect);
            rect1.x = rectangle1.X;
            rect1.y = rectangle1.Y;
            rect1.w = rectangle1.Width;
            rect1.h = rectangle1.Height;
            rect = &rect1;
        }

        var isSuccess = SDL_RenderRect(_handle, rect);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_RenderRect), isExceptionThrown: true);
        }
    }

    /// <summary>
    ///     Renders a line to the current render target using the <see cref="DrawColor" />.
    /// </summary>
    /// <param name="x1">The x coordinate of the start point.</param>
    /// <param name="y1">The y coordinate of the start point.</param>
    /// <param name="x2">The x coordinate of the end point.</param>
    /// <param name="y2">The y coordinate of the end point.</param>
    public void RenderLine(float x1, float y1, float x2, float y2)
    {
        var isSuccess = SDL_RenderLine(_handle, x1, y1, x2, y2);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_RenderLine), isExceptionThrown: true);
        }
    }

    /// <summary>
    ///     Renders a point to the current render target using the <see cref="DrawColor" />.
    /// </summary>
    /// <param name="x">The x coordinate of the point.</param>
    /// <param name="y">The y coordinate of the point.</param>
    public void RenderPoint(float x, float y)
    {
        var isSuccess = SDL_RenderPoint(_handle, x, y);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_RenderPoint), isExceptionThrown: true);
        }
    }

    /// <summary>
    ///     Attempts to set the current <see cref="bottlenoselabs.SDL.VSyncMode" />.
    /// </summary>
    /// <param name="value">The vertical sync mode.</param>
    /// <returns>
    ///     <c>true</c> if the vertical sync mode was successfully set; otherwise, <c>false</c> indicating
    ///     <paramref name="value" /> is not supported for the current driver.
    /// </returns>
    /// <remarks>
    ///     <para>Can return <c>false</c> if the <paramref name="value" /> is not supported by the current driver.</para>
    /// </remarks>
    public bool TrySetVSyncMode(VSyncMode value)
    {
        return SetVSyncMode(value);
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        SDL_DestroyRenderer(_handle);
        _handle = null;
        base.Dispose(isDisposing);
    }

    private Rgba8U GetDrawColor()
    {
        byte r;
        byte g;
        byte b;
        byte a;

        var isSuccess = SDL_GetRenderDrawColor(_handle, &r, &g, &b, &a);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_GetRenderDrawColor), isExceptionThrown: true);
        }

        var color = new Rgba8U(r, g, b, a);
        return color;
    }

    private void SetDrawColor(in Rgba8U color)
    {
        var isSuccess = SDL_SetRenderDrawColor(_handle, color.R, color.G, color.B, color.A);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_SetRenderDrawColor), isExceptionThrown: true);
        }
    }

    private Rectangle GetViewport()
    {
        SDL_Rect rect;
        var isSuccess = SDL_GetRenderViewport(_handle, &rect);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_GetRenderViewport), isExceptionThrown: true);
        }

        var viewport = *(Rectangle*)&rect;
        return viewport;
    }

    private void SetViewport(in Rectangle rectangle)
    {
        fixed (Rectangle* rectanglePointer = &rectangle)
        {
            var pointer = (SDL_Rect*)rectanglePointer;
            var isSuccess = SDL_SetRenderViewport(_handle, pointer);
            if (!isSuccess)
            {
                Error.NativeFunctionFailed(nameof(SDL_SetRenderViewport), isExceptionThrown: true);
            }
        }
    }

    private VSyncMode GetVSyncMode()
    {
        int vsync;
        var isSuccess = SDL_GetRenderVSync(_handle, &vsync);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_GetRenderVSync));
            return VSyncMode.Unknown;
        }

        return vsync switch
        {
            0 => VSyncMode.Disabled,
            1 => VSyncMode.EnabledEveryRefresh,
            2 => VSyncMode.EnabledEverySecondRefresh,
            -1 => VSyncMode.Adaptive,
            _ => VSyncMode.Unknown
        };
    }

    private bool SetVSyncMode(VSyncMode value)
    {
        if (value == VSyncMode.Unknown)
        {
            return false;
        }

        var vsync = value switch
        {
            VSyncMode.Disabled => 0,
            VSyncMode.EnabledEveryRefresh => 1,
            VSyncMode.EnabledEverySecondRefresh => 2,
            VSyncMode.Adaptive => -1,
            _ => 0
        };

        var isSuccess = SDL_SetRenderVSync(_handle, vsync);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_SetRenderVSync));
        }

        return isSuccess;
    }
}
