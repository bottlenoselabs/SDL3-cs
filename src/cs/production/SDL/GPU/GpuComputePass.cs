// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///  TODO.
/// </summary>
[PublicAPI]
public sealed unsafe class GpuComputePass : Poolable<GpuComputePass>
{
#pragma warning disable SA1401
    internal SDL_GPUComputePass* Handle;
#pragma warning restore SA1401

    /// <summary>
    ///     Gets the <see cref="GpuDevice" /> instance associated with the compute pass.
    /// </summary>
    public GpuDevice Device { get; }

    /// <summary>
    ///     Gets the <see cref="CommandBuffer" /> instance associated with the compute pass.
    /// </summary>
    public GpuCommandBuffer CommandBuffer { get; internal set; }

    internal GpuComputePass(GpuDevice device)
    {
        Device = device;
        CommandBuffer = null!;
        Handle = null;
    }

    /// <summary>
    ///     TODO.
    /// </summary>
    /// <param name="computeShader">The compute shader.</param>
    public void BindShader(GpuComputeShader computeShader)
    {
        SDL_BindGPUComputePipeline(Handle, computeShader.HandleTyped);
    }

    /// <summary>
    ///     TODO.
    /// </summary>
    /// <param name="workGroupsCountX">The number of thread groups to dispatch in the X dimension.</param>
    /// <param name="workGroupsCountY">The number of thread groups to dispatch in the Y dimension.</param>
    /// <param name="workGroupsCountZ">The number of thread groups to dispatch in the Z dimension.</param>
    public void Dispatch(
        int workGroupsCountX,
        int workGroupsCountY,
        int workGroupsCountZ)
    {
        SDL_DispatchGPUCompute(
            Handle, (uint)workGroupsCountX, (uint)workGroupsCountY, (uint)workGroupsCountZ);
    }

    /// <summary>
    ///     Ends the compute pass.
    /// </summary>
    /// <exception cref="InvalidOperationException">The associated command buffer was submitted.</exception>
    public void End()
    {
        Device.EndComputePassTryInternal(this);
        _ = TryToReturnToPool();
    }

    /// <inheritdoc />
    protected override void Reset()
    {
        Device.EndComputePassTryInternal(this);
    }
}
