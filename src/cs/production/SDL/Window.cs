// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL.GPU;

namespace bottlenoselabs.SDL;

/// <summary>
///     Represents a desktop operating system window using SDL.
/// </summary>
[PublicAPI]
public sealed unsafe class Window : NativeHandleTyped<SDL_Window>
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

        HandleTyped = SDL_CreateWindow(
            options.TitleCString,
            options.Width,
            options.Height,
            flags);
        Handle = (IntPtr)HandleTyped;

        if (Handle == IntPtr.Zero)
        {
            Error.NativeFunctionFailed(nameof(SDL_CreateWindow), isExceptionThrown: true);
        }

        var windowId = SDL_GetWindowID(HandleTyped);
        if (windowId == 0)
        {
            Error.NativeFunctionFailed(nameof(SDL_GetWindowID), isExceptionThrown: true);
        }

        Application.WindowsById[windowId] = this;

        if (options is { IsEnabledCreateSurface: true, IsEnabledCreateRenderer: true })
        {
            throw new InvalidOperationException(
                "Can not have window create surface and create renderer at the same time.");
        }

        if (options.IsEnabledCreateSurface)
        {
            var surfaceHandle = SDL_GetWindowSurface(HandleTyped);
            if (surfaceHandle == null)
            {
                Error.NativeFunctionFailed(nameof(SDL_GetWindowSurface), isExceptionThrown: true);
            }
            else
            {
                Surface = new Surface(surfaceHandle);
            }
        }

        if (options.IsEnabledCreateRenderer)
        {
            var rendererHandle = SDL_CreateRenderer(HandleTyped, null);
            if (rendererHandle == null)
            {
                Error.NativeFunctionFailed(nameof(SDL_CreateRenderer), isExceptionThrown: true);
            }
            else
            {
                Renderer = new Renderer2D(rendererHandle);
            }
        }

        int widthActual, heightActual;
        if (!SDL_GetWindowSize(HandleTyped, &widthActual, &heightActual))
        {
            Error.NativeFunctionFailed(nameof(SDL_GetWindowSize), isExceptionThrown: true);
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
        var isSuccess = SDL_SetWindowPosition(HandleTyped, x, y);
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
        var isSuccess = SDL_UpdateWindowSurface(HandleTyped);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_UpdateWindowSurface));
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        var windowId = SDL_GetWindowID(HandleTyped);
        if (windowId == 0)
        {
            Error.NativeFunctionFailed(nameof(SDL_GetWindowID), isExceptionThrown: true);
        }

        Application.WindowsById.Remove(windowId);

        _allocator.Dispose();

        if (IsClaimed)
        {
            Swapchain!.Dispose();
            Swapchain = null;
        }

        SDL_DestroyWindow(HandleTyped);
        base.Dispose(isDisposing);
    }

    private void TrySetTitle(string value)
    {
        _allocator.Reset();
        var cString = _allocator.AllocateCString(value);
        var isSuccess = SDL_SetWindowTitle(HandleTyped, cString);
        _allocator.Reset();

        if (isSuccess)
        {
            _title = value;
        }
        else
        {
            Error.NativeFunctionFailed(nameof(SDL_SetWindowTitle));
        }
    }
}
