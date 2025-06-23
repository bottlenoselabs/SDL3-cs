// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL.GPU;

namespace bottlenoselabs.SDL;

/// <summary>
///     Represents a desktop operating system window using SDL.
/// </summary>
[PublicAPI]
public sealed unsafe class Window : NativeHandle
{
    private string _title = string.Empty;

    private readonly ArenaNativeAllocator _allocator = new(1024);

    /// <summary>
    ///     Gets or sets the text title of the window.
    /// </summary>
    public string Title
    {
        get => _title;
        set => TrySetTitle(value);
    }

    /// <summary>
    ///     Gets the width of the window.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    ///     Gets the height of the window.
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether the window is claimed by a <see cref="GpuDevice" />.
    /// </summary>
    public bool IsClaimed => Swapchain != null;

    /// <summary>
    ///     Gets the surface associated with the window.
    /// </summary>
    public Surface? Surface { get; internal set; }

    /// <summary>
    ///     Gets the renderer associated with the window.
    /// </summary>
    public Renderer2D? Renderer { get; internal set; }

    /// <summary>
    ///     Gets the swapchain instance associated with the window.
    /// </summary>
    public GpuSwapchain? Swapchain { get; internal set; }

    internal Window(WindowOptions options)
        : base(IntPtr.Zero)
    {
        var flags = default(SDL_WindowFlags);

        if (options.IsResizable)
        {
            flags |= SDL_WINDOW_RESIZABLE;
        }

        if (options.IsStartingMaximized)
        {
            flags |= SDL_WINDOW_MAXIMIZED;
        }

        Handle = (IntPtr)SDL_CreateWindow(
            options.TitleCString,
            options.Width,
            options.Height,
            flags);

        if (Handle == IntPtr.Zero)
        {
            bottlenoselabs.SDL.Error.NativeFunctionFailed(nameof(SDL_CreateWindow), isExceptionThrown: true);
        }

        if (options is { IsEnabledCreateSurface: true, IsEnabledCreateRenderer: true })
        {
            throw new InvalidOperationException(
                "Can not have window create surface and create renderer at the same time.");
        }

        if (options.IsEnabledCreateSurface)
        {
            var surfaceHandle = SDL_GetWindowSurface((SDL_Window*)Handle);
            if (surfaceHandle == null)
            {
                bottlenoselabs.SDL.Error.NativeFunctionFailed(nameof(SDL_GetWindowSurface), isExceptionThrown: true);
            }
            else
            {
                Surface = new Surface((IntPtr)surfaceHandle);
            }
        }

        if (options.IsEnabledCreateRenderer)
        {
            var rendererHandle = SDL_CreateRenderer((SDL_Window*)Handle, null);
            if (rendererHandle == null)
            {
                bottlenoselabs.SDL.Error.NativeFunctionFailed(nameof(SDL_CreateRenderer), isExceptionThrown: true);
            }
            else
            {
                Renderer = new Renderer2D((IntPtr)rendererHandle);
            }
        }

        int widthActual, heightActual;
        if (!SDL_GetWindowSize((SDL_Window*)Handle, &widthActual, &heightActual))
        {
            bottlenoselabs.SDL.Error.NativeFunctionFailed(nameof(SDL_GetWindowSize), isExceptionThrown: true);
        }

        Width = widthActual;
        Height = heightActual;
    }

    /// <summary>
    ///     Attempts to set the position of the window.
    /// </summary>
    /// <param name="x">The x coordinate of the window.</param>
    /// <param name="y">The y coordinate of the window.</param>
    /// <returns><c>true</c> if the window's position was successfully set; otherwise, <c>false</c>.</returns>
    public bool TrySetPosition(int x, int y)
    {
        var isSuccess = SDL_SetWindowPosition((SDL_Window*)Handle, x, y);
        return isSuccess;
    }

    /// <summary>
    ///     Presents the window's surface.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="Present" /> should only be called on the main thread.
    ///     </para>
    /// </remarks>
    /// <exception cref="NativeFunctionFailedException">Native function failed.</exception>
    public void Present()
    {
        var isSuccess = SDL_UpdateWindowSurface((SDL_Window*)Handle);
        if (!isSuccess)
        {
            bottlenoselabs.SDL.Error.NativeFunctionFailed(nameof(SDL_UpdateWindowSurface));
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        _allocator.Dispose();

        if (IsClaimed)
        {
            Swapchain!.Dispose();
            Swapchain = null;
        }

        SDL_DestroyWindow((SDL_Window*)Handle);
        base.Dispose(isDisposing);
    }

    private void TrySetTitle(string value)
    {
        _allocator.Reset();
        var cString = _allocator.AllocateCString(value);
        var isSuccess = SDL_SetWindowTitle((SDL_Window*)Handle, cString);
        _allocator.Reset();

        if (isSuccess)
        {
            _title = value;
        }
        else
        {
            bottlenoselabs.SDL.Error.NativeFunctionFailed(nameof(SDL_SetWindowTitle));
        }
    }
}
