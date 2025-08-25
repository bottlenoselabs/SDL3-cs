// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///    Represents a GPU compute pipeline.
/// </summary>
[PublicAPI]
public sealed unsafe class GpuComputePipeline : GpuResource
{
    internal GpuComputePipeline(
        GpuDevice device,
        IntPtr handle)
        : base(device, handle)
    {
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        SDL_ReleaseGPUComputePipeline((SDL_GPUDevice*)Device.Handle, (SDL_GPUComputePipeline*)Handle);
        base.Dispose(isDisposing);
    }
}
