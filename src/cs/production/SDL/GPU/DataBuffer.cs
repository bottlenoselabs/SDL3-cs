// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Represents a GPU resource that holds data in video memory.
/// </summary>
[PublicAPI]
public sealed unsafe class DataBuffer : Resource
{
    internal DataBuffer(Device device, IntPtr handle)
        : base(device, handle)
    {
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        SDL_ReleaseGPUBuffer((SDL_GPUDevice*)Device.Handle, (SDL_GPUBuffer*)Handle);
        base.Dispose(isDisposing);
    }
}
