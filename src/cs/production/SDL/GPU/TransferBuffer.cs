// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Represents an GPU resource for transferring (uploading or downloading) data between the application and GPU.
/// </summary>
[PublicAPI]
public sealed unsafe class TransferBuffer : Resource
{
    /// <summary>
    ///     Gets the data size of the transfer buffer.
    /// </summary>
    public int Size { get; }

    internal TransferBuffer(Device device, IntPtr handle, int size)
        : base(device, handle)
    {
        Size = size;
    }

    /// <summary>
    ///     Maps the transfer buffer into application address space.
    /// </summary>
    /// <param name="isCycled">
    ///     If <c>true</c>, cycles the transfer buffer if it is already bound. If <c>false</c>, does not cycle the
    ///     transfer buffer, overwriting the data.
    /// </param>
    /// <returns>
    ///     If successful, the memory pointer address of the mapped transfer buffer; otherwise,
    ///     <see cref="IntPtr.Zero" />.
    /// </returns>
    public IntPtr MapAsPointer(bool isCycled = false)
    {
        var ptr = SDL_MapGPUTransferBuffer(
            (SDL_GPUDevice*)Device.Handle, (SDL_GPUTransferBuffer*)Handle, isCycled);
        if (ptr == null)
        {
            var errorMessage = Error.GetMessage();
            Device.LogNativeFunctionFailed(nameof(SDL_MapGPUTransferBuffer), errorMessage);
            return IntPtr.Zero;
        }

        return (IntPtr)ptr;
    }

    /// <summary>
    ///     Maps the transfer buffer into application address space.
    /// </summary>
    /// <param name="isCycled">
    ///     If <c>true</c>, cycles the transfer buffer if it is already bound. If <c>false</c>, does not cycle the
    ///     transfer buffer, overwriting the data.
    /// </param>
    /// <returns>
    ///     If successful, the byte span over the memory pointer address and size of the mapped transfer buffer;
    ///     otherwise, <see cref="Span{T}.Empty" />. Use <see cref="MemoryMarshal" /> to cast the span to a different
    ///     type.
    /// </returns>
    public Span<byte> MapAsSpan(bool isCycled = false)
    {
        var ptr = SDL_MapGPUTransferBuffer(
            (SDL_GPUDevice*)Device.Handle, (SDL_GPUTransferBuffer*)Handle, isCycled);
        if (ptr == null)
        {
            var errorMessage = Error.GetMessage();
            Device.LogNativeFunctionFailed(nameof(SDL_MapGPUTransferBuffer), errorMessage);
            return default;
        }

        var span = new Span<byte>(ptr, (int)Size);
        return span;
    }

    /// <summary>
    ///     Unmaps the transfer buffer that was previously mapped.
    /// </summary>
    public void Unmap()
    {
        SDL_UnmapGPUTransferBuffer((SDL_GPUDevice*)Device.Handle, (SDL_GPUTransferBuffer*)Handle);
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        SDL_ReleaseGPUTransferBuffer((SDL_GPUDevice*)Device.Handle, (SDL_GPUTransferBuffer*)Handle);
        base.Dispose(isDisposing);
    }
}
