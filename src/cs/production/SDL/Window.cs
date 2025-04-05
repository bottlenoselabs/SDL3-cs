// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using SDL.GPU;

namespace SDL;

/// <summary>
///     Represents a desktop operating system window using SDL.
/// </summary>
[PublicAPI]
public sealed unsafe class Window : NativeHandle
{
    private readonly ArenaNativeAllocator _allocator = new(1024);

    /// <summary>
    ///     Gets the text title of the window.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the width of the window.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    ///     Gets the height of the window.
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether the window is claimed by a <see cref="Device" />.
    /// </summary>
    public bool IsClaimed => Swapchain != null;

    /// <summary>
    ///     Gets the <see cref="GPU.Swapchain" /> instance associated with the window.
    /// </summary>
    public Swapchain? Swapchain { get; internal set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Window" /> class.
    /// </summary>
    /// <param name="options">
    ///     The parameters used to create the window. If <c>null</c>, uses a width of 640, a height of 480,
    ///     positioned on the screen's center.
    /// </param>
    /// <exception cref="InvalidOperationException">Failed to create the window.</exception>
    public Window(WindowOptions? options = null)
        : base(IntPtr.Zero)
    {
        options ??= new WindowOptions
        {
            Width = 640,
            Height = 480,
            IsResizable = true,
            IsStartingMaximized = false
        };

        var flags = default(SDL_WindowFlags);

        if (options.IsResizable)
        {
            flags |= SDL_WINDOW_RESIZABLE;
        }

        if (options.IsStartingMaximized)
        {
            flags |= SDL_WINDOW_MAXIMIZED;
        }

        _allocator.Reset();
        var windowTitleC = _allocator.AllocateCString(options.Title);
        Handle = (IntPtr)SDL_CreateWindow(
            windowTitleC,
            options.Width,
            options.Height,
            flags);
        if (Handle == IntPtr.Zero)
        {
            var errorMessage = Error.GetMessage();
            throw new InvalidOperationException(errorMessage);
        }

        _allocator.Reset();

        int widthActual, heightActual;
        SDL_GetWindowSize((SDL_Window*)Handle, &widthActual, &heightActual);
        Width = widthActual;
        Height = heightActual;
    }

    /// <summary>
    ///     Attempts to set the text title of the window.
    /// </summary>
    /// <param name="title">The new text title.</param>
    /// <returns><c>true</c> if the window's title was successfully set; otherwise, <c>false</c>.</returns>
    public bool TrySetTitle(string? title)
    {
        if (string.IsNullOrEmpty(title))
        {
            title = string.Empty;
        }

        _allocator.Reset();
        var titleC = _allocator.AllocateCString(title);
        var isSuccess = SDL_SetWindowTitle((SDL_Window*)Handle, titleC);
        _allocator.Reset();
        if (!isSuccess)
        {
            return false;
        }

        Title = title;
        return true;
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
    ///     Attempts to get the <see cref="Surface" /> of the window.
    /// </summary>
    /// <param name="surface">The surface.</param>
    /// <returns><c>true</c> if the surface was successfully acquired; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     <para>
    ///         The surface will be invalidated if the window is resized. After resizing a window
    ///         <see cref="TryGetSurface" /> must be called again to return a valid surface.
    ///     </para>
    ///     <para>
    ///         <see cref="TryGetSurface" /> should only be called on the main thread.
    ///     </para>
    /// </remarks>
    public bool TryGetSurface(out Surface? surface)
    {
        var handle = SDL_GetWindowSurface((SDL_Window*)Handle);
        if (handle == null)
        {
            surface = null;
            return false;
        }

        surface = new Surface((IntPtr)handle);
        return true;
    }

    /// <summary>
    ///     Attempts to copy the window's surface to the screen.
    /// </summary>
    /// <returns><c>true</c> if the window's surface was successfully copied to the screen; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     <para>
    ///         Use <see cref="TryUpdateFromSurface" /> to reflect any changes made to the surface on the screen.
    ///     </para>
    ///     <para>
    ///         <see cref="TryUpdateFromSurface" /> should only be called on the main thread.
    ///     </para>
    /// </remarks>
    public bool TryUpdateFromSurface()
    {
        var result = SDL_UpdateWindowSurface((SDL_Window*)Handle);
        return result;
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
}
