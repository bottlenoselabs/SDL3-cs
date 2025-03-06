// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Represents a collection of texture render-targets that are used in cyclic manner for displaying rendered frames
///     on screen.
/// </summary>
[PublicAPI]
public sealed unsafe class Swapchain : Disposable
{
    /// <summary>
    ///     Gets the <see cref="Window" /> instance associated with the swapchain.
    /// </summary>
    public Window Window { get; private set; }

    /// <summary>
    ///     Gets the <see cref="GPU.Device" /> instance associated with the swapchain.
    /// </summary>
    public Device Device { get; private set; }

    /// <summary>
    ///     Gets the texture format and color-space of the swapchain.
    /// </summary>
    public SwapchainComposition Composition { get; private set; }

    /// <summary>
    ///     Gets the texture format of the swapchain.
    /// </summary>
    public TextureFormat TextureFormat => Texture.Format;

    internal Texture Texture { get; private set; }

    internal Swapchain(Device device, Window window)
    {
        Device = device;
        Window = window;
        Composition = SwapchainComposition.SDR;
        var format = (TextureFormat)SDL_GetGPUSwapchainTextureFormat(
            (SDL_GPUDevice*)device.Handle, (SDL_Window*)window.Handle);

        // NOTE: Swapchain texture handles are managed by the driver backend.
        //  Width and height are subject to change on the fly as the window could resize, so they are zero here.
        var handle = IntPtr.Zero;
        Texture = new Texture(
            device,
            handle,
            TextureType.TwoDimensional,
            format,
            0,
            0,
            1,
            1,
            1,
            TextureUsages.ColorRenderTarget);
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        SDL_ReleaseWindowFromGPUDevice((SDL_GPUDevice*)Device.Handle, (SDL_Window*)Window.Handle);
        Window = null!;
        Device = null!;
        Texture = null!;
    }
}
