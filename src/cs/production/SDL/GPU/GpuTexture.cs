// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Represents a GPU resource that holds one or many layers of structured texture elements, called texels, and the
///     related information such as how many texels there are and how they are encoded and organized.
/// </summary>
[PublicAPI]
public sealed unsafe class GpuTexture : GpuResource<SDL_GPUTexture>
{
    private readonly bool _isSwapchain;

    /// <summary>
    ///     Gets the <see cref="GpuTextureType" /> of the texture.
    /// </summary>
    public GpuTextureType Type { get; private set; }

    /// <summary>
    ///     Gets the <see cref="GpuTextureFormat" /> of the texture.
    /// </summary>
    public GpuTextureFormat Format { get; private set; }

    /// <summary>
    ///     Gets the number of texels in the X axis of the texture.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    ///     Gets the number of texels in the Y axis of the texture.
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    ///     Gets the layer count (number of textures) or depth (number of texels in the Z axis) of the texture.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="LayerCountOrDepth" /> is treated as a layer count on 2D array textures, and as a depth value
    ///         on 3D textures.
    ///     </para>
    /// </remarks>
    public int LayerCountOrDepth { get; private set; }

    /// <summary>
    ///     Gets the number of mipmap levels of the texture.
    /// </summary>
    public int MipMapLevelCount { get; private set; }

    /// <summary>
    ///     Gets the number of samples per texel of the render target texture.
    /// </summary>
    /// <remarks>
    ///     <para><see cref="SampleCount" /> only applies if the texture is used as a render target.</para>
    /// </remarks>
    public int SampleCount { get; private set; }

    /// <summary>
    ///     Gets the usages of the texture.
    /// </summary>
    public GpuTextureUsages Usages { get; private set; }

    internal GpuTexture(
        GpuDevice device,
        SDL_GPUTexture* handle,
        GpuTextureType type,
        GpuTextureFormat format,
        int width,
        int height,
        int layerCountOrDepth,
        int mipMapLevelCount,
        int sampleCount,
        GpuTextureUsages usages)
        : base(device, handle)
    {
        Type = type;
        Format = format;
        Width = width;
        Height = height;
        LayerCountOrDepth = layerCountOrDepth;
        Usages = usages;
    }

    /// <inheritdoc cref="Disposable.Dispose()" />
    public new void Dispose()
    {
        // NOTE: Swapchain texture handles are managed by the driver backend.
        //  So in this case, we do nothing; the object instance can't be disposed through the public API.
        if (_isSwapchain)
        {
            return;
        }

        base.Dispose();
    }

    internal void UpdateTextureSwapchain(
        SDL_GPUTexture* handle,
        int width,
        int height)
    {
        Handle = (IntPtr)handle;
        HandleTyped = handle;
        Width = width;
        Height = height;
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        SDL_ReleaseGPUTexture(Device.HandleTyped, HandleTyped);
        base.Dispose(isDisposing);
    }
}
