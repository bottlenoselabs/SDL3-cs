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

    private bool _isPipelineBound;

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
        _isPipelineBound = true;
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
        if (!_isPipelineBound)
        {
            throw new InvalidOperationException("A compute shader must be bound before dispatching.");
        }

        SDL_DispatchGPUCompute(
            Handle, (uint)workGroupsCountX, (uint)workGroupsCountY, (uint)workGroupsCountZ);
    }

    /// <summary>
    ///     Dispatches compute work with parameters set from a buffer.
    /// </summary>
    /// <param name="buffer">The buffer containing dispatch parameters.</param>
    /// <param name="offset">the offset to start reading from the dispatch buffer.</param>
    public void DispatchIndirect(GpuDataBuffer buffer, int offset)
    {
        if (!_isPipelineBound)
        {
            throw new InvalidOperationException("A compute shader must be bound before dispatching.");
        }

        SDL_DispatchGPUComputeIndirect(Handle, buffer.HandleTyped, (uint)offset);
    }

    /// <summary>
    ///     Binds storage textures as readonly for use on the compute pipeline.
    /// </summary>
    /// <param name="startIndex">Index of the slot to begin binding from.</param>
    /// <param name="textures">An array of <see cref="GpuTexture"/>s to bind.</param>
    public void BindStorageTextures(int startIndex, params ReadOnlySpan<GpuTexture> textures)
    {
        var handles = stackalloc SDL_GPUTexture*[textures.Length];
        for (var i = 0; i < textures.Length; i++)
        {
            handles[i] = (SDL_GPUTexture*)textures[i].Handle;
        }

        SDL_BindGPUComputeStorageTextures(
            Handle,
            (uint)startIndex,
            handles,
            (uint)textures.Length);
    }

    /// <summary>
    ///     Binds storage buffers as readonly for use on the compute pipeline.
    /// </summary>
    /// <param name="startIndex">Index of the slot to begin binding from.</param>
    /// <param name="buffers">An array of <see cref="GpuDataBuffer"/>s to bind.</param>
    public void BindStorageBuffers(int startIndex, params ReadOnlySpan<GpuDataBuffer> buffers)
    {
        var handles = stackalloc SDL_GPUBuffer*[buffers.Length];
        for (var i = 0; i < buffers.Length; i++)
        {
            handles[i] = (SDL_GPUBuffer*)buffers[i].Handle;
        }

        SDL_BindGPUComputeStorageBuffers(
            Handle,
            (uint)startIndex,
            handles,
            (uint)buffers.Length);
    }

    /// <summary>
    ///     Binds texture-sampler pairs for use on the compute shader pipeline.
    /// </summary>
    /// <param name="startIndex">Index of the slot to begin binding from.</param>
    /// <param name="samplers">An array of <see cref="GpuTexture"/> and <see cref="GpuSampler"/> pairs to bind.</param>
    public void BindSamplers(
        int startIndex,
        params ReadOnlySpan<(GpuTexture Texture, GpuSampler Sampler)> samplers)
    {
        var bindings = stackalloc SDL_GPUTextureSamplerBinding[samplers.Length];
        for (var i = 0; i < samplers.Length; i++)
        {
            var src = samplers[i];
            ref var dst = ref bindings[i];
            dst.texture = (SDL_GPUTexture*)src.Texture.Handle;
            dst.sampler = (SDL_GPUSampler*)src.Sampler.Handle;
        }

        SDL_BindGPUComputeSamplers(
            Handle,
            (uint)startIndex,
            bindings,
            (uint)samplers.Length);
    }

    /// <summary>
    ///     Ends a render pass.
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
        _isPipelineBound = false;
    }
}
