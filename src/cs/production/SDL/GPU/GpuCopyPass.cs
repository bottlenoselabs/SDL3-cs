// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Represents a context for transferring data to or from the GPU to the application.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="GpuCopyPass" /> instances are <b>not</b> pooled and must not be used or referenced after calling
///         <see cref="GpuRenderPass.End" />. To get a <see cref="GpuCopyPass" /> instance call
///         <see cref="GpuCommandBuffer.BeginCopyPass" />.
///     </para>
/// </remarks>
[PublicAPI]
public unsafe class GpuCopyPass : GpuResource
{
    /// <summary>
    ///     Gets the <see cref="CommandBuffer" /> instance associated with the render pass.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="CommandBuffer" /> is <c>null</c> when <see cref="Disposable.IsDisposed" /> is <c>true</c>.
    ///     </para>
    /// </remarks>
    public GpuCommandBuffer CommandBuffer { get; private set; }

    internal GpuCopyPass(GpuDevice device, IntPtr handle, GpuCommandBuffer commandBuffer)
        : base(device, handle)
    {
        CommandBuffer = commandBuffer;
    }

    /// <summary>
    ///     Uploads data to a data buffer.
    /// </summary>
    /// <param name="transferBuffer">The transfer buffer used as the source.</param>
    /// <param name="transferBufferOffset">The starting byte of the data in the transfer buffer.</param>
    /// <param name="dataBuffer">The data buffer used as the destination.</param>
    /// <param name="dataBufferOffset">The starting byte within the data buffer to upload data to.</param>
    /// <param name="dataBufferByteCount">The size in bytes of the data to upload to the data buffer.</param>
    /// <param name="isCycled">
    ///     If <c>true</c>, cycles the data buffer if it is already bound. If <c>false</c>, does not cycle the
    ///     data buffer, overwriting the data.
    /// </param>
    public void UploadToDataBuffer(
        GpuTransferBuffer? transferBuffer,
        int transferBufferOffset,
        GpuDataBuffer? dataBuffer,
        int dataBufferOffset,
        int dataBufferByteCount,
        bool isCycled = false)
    {
        if (transferBuffer == null || dataBuffer == null)
        {
            return;
        }

        var bufferSourceLocation = default(SDL_GPUTransferBufferLocation);
        bufferSourceLocation.transfer_buffer = (SDL_GPUTransferBuffer*)transferBuffer.Handle;
        bufferSourceLocation.offset = (uint)transferBufferOffset;

        var bufferDestinationRegion = default(SDL_GPUBufferRegion);
        bufferDestinationRegion.buffer = (SDL_GPUBuffer*)dataBuffer.Handle;
        bufferDestinationRegion.offset = (uint)dataBufferOffset;
        bufferDestinationRegion.size = (uint)dataBufferByteCount;

        SDL_UploadToGPUBuffer((SDL_GPUCopyPass*)Handle, &bufferSourceLocation, &bufferDestinationRegion, isCycled);
    }

    /// <summary>
    ///     Uploads data to a texture.
    /// </summary>
    /// <param name="transferBuffer">The transfer buffer used as the source.</param>
    /// <param name="transferBufferOffset">The starting byte of the data in the transfer buffer.</param>
    /// <param name="texture">The texture used as the destination.</param>
    /// <param name="textureWidth">The width of the texture.</param>
    /// <param name="textureHeight">The height of the texture.</param>
    /// <param name="textureDepth">The depth of the texture.</param>
    /// <param name="isCycled">
    ///     If <c>true</c>, cycles the data buffer if it is already bound. If <c>false</c>, does not cycle the
    ///     data buffer, overwriting the data.
    /// </param>
    public void UploadToTexture(
        GpuTransferBuffer? transferBuffer,
        int transferBufferOffset,
        GpuTexture? texture,
        int textureWidth,
        int textureHeight,
        int textureDepth = 1,
        bool isCycled = false)
    {
        if (transferBuffer == null || texture == null)
        {
            return;
        }

        var bufferSourceTexture = default(SDL_GPUTextureTransferInfo);
        bufferSourceTexture.transfer_buffer = (SDL_GPUTransferBuffer*)transferBuffer.Handle;
        bufferSourceTexture.offset = (uint)transferBufferOffset;
        var bufferDestinationTexture = default(SDL_GPUTextureRegion);
        bufferDestinationTexture.texture = (SDL_GPUTexture*)texture.Handle;
        bufferDestinationTexture.w = (uint)textureWidth;
        bufferDestinationTexture.h = (uint)textureHeight;
        bufferDestinationTexture.d = (uint)textureDepth;
        SDL_UploadToGPUTexture(
            (SDL_GPUCopyPass*)Handle, &bufferSourceTexture, &bufferDestinationTexture, isCycled);
    }

    /// <summary>
    ///     Ends the copy pass.
    /// </summary>
    public void End()
    {
        if (IsDisposed)
        {
            return;
        }

        var handle = Handle;
        if (handle == IntPtr.Zero)
        {
            return;
        }

        SDL_EndGPUCopyPass((SDL_GPUCopyPass*)handle);
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        End();
        CommandBuffer = null!;
        base.Dispose(isDisposing);
    }
}
