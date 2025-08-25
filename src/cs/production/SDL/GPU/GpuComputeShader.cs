// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Represents a developer programmable compute program used in the compute pipeline that transforms large amounts
///     of data in parallel.
/// </summary>
[PublicAPI]
public sealed unsafe class GpuComputeShader : GpuResource<SDL_GPUComputePipeline>
{
    // NOTE: SDL_GPU uses the name "pipeline" for the compute shader as it collapses when binding.

    internal GpuComputeShader(GpuDevice device, SDL_GPUComputePipeline* handle)
        : base(device, handle)
    {
    }
}
