// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace bottlenoselabs.SDL;

/// <summary>
///     Represents a context for the CPU to queue up commands that are later all submitted at once to a
///     <see cref="GpuDevice" /> instance for execution.
/// </summary>
/// <remarks>
///     <para>
///         Commands only begin execution on the GPU once <see cref="Submit" /> is called. Multiple
///         <see cref="GpuCommandBuffer" /> instances have their commands executed relative to the order the
///         <see cref="GpuCommandBuffer" /> instance is submitted. For example, if you submit command buffer A and then
///         command buffer B, all commands in A will begin executing before any command in B begins executing.
///     </para>
///     <para>
///         <see cref="GpuCommandBuffer" /> instances are pooled and must not be used or referenced after calling
///         <see cref="GpuCommandBuffer.Submit" />. To get a <see cref="GpuCommandBuffer" /> instance call
///         <see cref="GpuDevice.GetCommandBuffer" />.
///     </para>
///     <para>
///         In multi-threading scenarios, you should only use and submit a <see cref="GpuCommandBuffer" /> instance on
///         the thread you acquired it from.
///     </para>
///     <para>
///         It is valid to acquire multiple <see cref="GpuCommandBuffer" /> instances on the same thread at once.
///         In fact a common design pattern is to acquire two command buffers per frame: one for render and compute
///         passes and the other for copy passes and other preparatory work such as generating mipmaps. Interleaving
///         commands between the two command buffers reduces the total amount of passes overall which improves
///         rendering performance.
///     </para>
/// </remarks>
[PublicAPI]
public sealed unsafe class GpuCommandBuffer : Poolable<GpuCommandBuffer>
{
    private bool _isSubmitted;

    /// <summary>
    ///     Gets the <see cref="GpuDevice" /> instance associated with the command buffer.
    /// </summary>
    public GpuDevice Device { get; }

    /// <summary>
    ///     Gets the unmanaged handle associated with the object instance.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="HandleTyped" /> is <c>null</c> when <see cref="Poolable{TSelf}.IsDisposed" /> is
    ///         <c>true</c>.
    ///     </para>
    /// </remarks>
    public SDL_GPUCommandBuffer* HandleTyped { get; private set; }

    internal GpuCommandBuffer(GpuDevice device)
    {
        Device = device;
        HandleTyped = null;
    }

    /// <summary>
    ///     Blocks until a swapchain render-target texture is available for the specified <see cref="Window" /> and then
    ///     acquires the swapchain render-target texture.
    /// </summary>
    /// <param name="window">The <see cref="Window" />.</param>
    /// <param name="swapchainTexture">
    ///     The render-target <see cref="GpuTexture" /> which will be presented to the <paramref name="window" /> when
    ///     <see cref="GpuCommandBuffer.Submit" /> is called.
    /// </param>
    /// <returns><c>true</c> if the texture was successfully acquired; otherwise, <c>false</c>.</returns>
    /// <exception cref="InvalidOperationException">The command buffer is submitted to the device.</exception>
    /// <remarks>
    ///     <para>
    ///         It is an error to acquire two swapchain textures from the same window using the same command buffer.
    ///     </para>
    /// </remarks>
    public bool TryGetSwapchainTexture(
        Window window,
        [NotNullWhen(true)] out GpuTexture? swapchainTexture)
    {
        ThrowIfSubmitted();

        SDL_GPUTexture* textureSwapchainHandle;
        uint width;
        uint height;

        if (!SDL_WaitAndAcquireGPUSwapchainTexture(
                HandleTyped,
                (SDL_Window*)window.Handle,
                &textureSwapchainHandle,
                &width,
                &height))
        {
            Error.NativeFunctionFailed(nameof(SDL_WaitAndAcquireGPUSwapchainTexture));
            swapchainTexture = null;
            return false;
        }

        if (textureSwapchainHandle == null)
        {
            swapchainTexture = null;
            return false;
        }

        swapchainTexture = window.Swapchain!.Texture;
        swapchainTexture.UpdateTextureSwapchain(textureSwapchainHandle, (int)width, (int)height);
        return true;
    }

    /// <summary>
    ///     Begins a render pass.
    /// </summary>
    /// <param name="depthStencilTargetInfo">The depth-stencil render-target to use in the render pass.</param>
    /// <param name="colorTargetInfos">The color render-targets to use in the render pass.</param>
    /// <returns>A pooled <see cref="GpuRenderPass" /> instance.</returns>
    /// <remarks>
    ///     <para>
    ///         <see cref="GpuRenderPass.End" /> must be called before starting another render pass, compute pass, or
    ///         copy pass.
    ///     </para>
    /// </remarks>
    public GpuRenderPass BeginRenderPass(
        in GpuRenderTargetInfoDepthStencil? depthStencilTargetInfo = null,
        params Span<GpuRenderTargetInfoColor> colorTargetInfos)
    {
        ThrowIfSubmitted();

        if (colorTargetInfos.IsEmpty)
        {
            throw new ArgumentException("Color render-targets can not be empty.", nameof(colorTargetInfos));
        }

        SDL_GPUDepthStencilTargetInfo* depthStencilInfoPointer;
        if (depthStencilTargetInfo == null)
        {
            depthStencilInfoPointer = null;
        }
        else
        {
            var destination = default(SDL_GPUDepthStencilTargetInfo);
            depthStencilInfoPointer = &destination;

            var source = depthStencilTargetInfo.Value;
            destination.texture = (SDL_GPUTexture*)(source.Texture?.Handle ?? IntPtr.Zero);
            destination.cycle = source.IsTextureCycled;
            destination.clear_depth = source.ClearDepth;
            destination.clear_stencil = source.ClearStencil;
            destination.load_op = (SDL_GPULoadOp)source.LoadOperation;
            destination.store_op = (SDL_GPUStoreOp)source.StoreOp;
            destination.stencil_load_op = (SDL_GPULoadOp)source.StencilLoadOperation;
            destination.stencil_store_op = (SDL_GPUStoreOp)source.StencilStoreOp;
        }

        var colorTargetInfosPointer = stackalloc SDL_GPUColorTargetInfo[colorTargetInfos.Length];
        for (var i = 0; i < colorTargetInfos.Length; i++)
        {
            ref var source = ref colorTargetInfos[i];
            ref var destination = ref colorTargetInfosPointer[i];
            destination.texture = (SDL_GPUTexture*)(source.Texture?.Handle ?? IntPtr.Zero);
            destination.mip_level = (uint)source.MipMapLevel;
            destination.layer_or_depth_plane = (uint)source.LayerOrDepthPlane;
            destination.clear_color = source.ClearColor;
            destination.load_op = (SDL_GPULoadOp)source.LoadOperation;
            destination.store_op = (SDL_GPUStoreOp)source.StoreOp;
            destination.resolve_texture = (SDL_GPUTexture*)(source.ResolveTexture?.Handle ?? IntPtr.Zero);
            destination.resolve_mip_level = (uint)source.ResolveMipMapLevel;
            destination.resolve_layer = (uint)source.ResolveLayer;
            destination.cycle = source.IsTextureCycled;
            destination.cycle_resolve_texture = source.IsResolveTextureCycled;
        }

        var handle = SDL_BeginGPURenderPass(
            HandleTyped, colorTargetInfosPointer, (uint)colorTargetInfos.Length, depthStencilInfoPointer);
        if (handle == null)
        {
            Error.NativeFunctionFailed(nameof(SDL_BeginGPURenderPass), isExceptionThrown: true);
        }

        var renderPass = Device.PoolRenderPass.GetOrCreate()!;
        renderPass.Handle = handle;
        renderPass.CommandBuffer = this;
        return renderPass;
    }

    /// <summary>
    ///     Begins a copy pass.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         All operations related to copying to or from buffers and textures take place inside a copy pass.
    ///     </para>
    ///     <para>
    ///         <see cref="GpuCopyPass.End" /> must be called before starting another copy pass, render pass, or compute
    ///         pass.
    ///     </para>
    /// </remarks>
    /// <returns>A pooled <see cref="GpuCopyPass" /> instance.</returns>
    public GpuCopyPass BeginCopyPass()
    {
        ThrowIfSubmitted();

        var handle = SDL_BeginGPUCopyPass(HandleTyped);
        var copyPass = new GpuCopyPass(Device, handle, this);
        return copyPass;
    }

    /// <summary>
    ///     Begins a compute pass.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="GpuComputePass.End" /> must be called before starting another compute pass, render pass, or
    ///         copy pass.
    ///     </para>
    /// </remarks>
    /// <param name="parameters">The compute pass parameters.</param>
    /// <returns>A pooled <see cref="GpuComputePass" /> instance.</returns>
    public GpuComputePass BeginComputePass(in GpuComputePassParameters parameters)
    {
        ThrowIfSubmitted();

        var textureWriteBindingsCount = parameters.TextureWriteBindings.Length;
        var textureWriteBindings =
            stackalloc SDL_GPUStorageTextureReadWriteBinding[textureWriteBindingsCount];
        for (var i = 0; i < textureWriteBindingsCount; i++)
        {
            ref var source = ref parameters.TextureWriteBindings[i];
            ref var destination = ref textureWriteBindings[i];
            destination.texture = source.Texture.HandleTyped;
            destination.mip_level = (uint)source.MipMapLevelIndex;
            destination.layer = (uint)source.LayerOrDepthIndex;
            destination.cycle = source.IsCycled;
        }

        var dataBufferWriteBindingsCount = parameters.DataBufferWriteBindings.Length;
        var dataBufferWriteBindings =
            stackalloc SDL_GPUStorageBufferReadWriteBinding[dataBufferWriteBindingsCount];
        for (var i = 0; i < dataBufferWriteBindingsCount; i++)
        {
            ref var source = ref parameters.DataBufferWriteBindings[i];
            ref var destination = ref dataBufferWriteBindings[i];
            destination.buffer = source.Buffer.HandleTyped;
            destination.cycle = source.IsCycled;
        }

        var handle = SDL_BeginGPUComputePass(
            HandleTyped,
            textureWriteBindings,
            (uint)textureWriteBindingsCount,
            dataBufferWriteBindings,
            (uint)dataBufferWriteBindingsCount);
        if (handle == null)
        {
            Error.NativeFunctionFailed(nameof(SDL_BeginGPUComputePass), isExceptionThrown: true);
        }

        var computePass = Device.PoolComputePass.GetOrCreate()!;
        computePass.Handle = handle;
        computePass.CommandBuffer = this;
        return computePass;
    }

    /// <summary>
    ///     Pushes the specified data to a vertex shader uniform slot. Subsequent draw calls will
    ///     use this uniform data.
    /// </summary>
    /// <param name="data">TODO.</param>
    /// <param name="startIndex">Index of the uniform slot to push data to.</param>
    /// <typeparam name="T">Data type. It must respect std140 layout conventions.</typeparam>
    public void PushVertexShaderUniformData<T>(in T data, int startIndex = 0)
        where T : unmanaged
    {
        fixed (T* pointer = &data)
        {
            SDL_PushGPUVertexUniformData(HandleTyped, (uint)startIndex, pointer, (uint)sizeof(T));
        }
    }

    /// <summary>
    ///     Pushes the specified <see cref="Matrix4x4" /> to a vertex shader uniform slot. Subsequent draw calls will
    ///     use this uniform data.
    /// </summary>
    /// <param name="matrix">The matrix to push.</param>
    /// <param name="slotIndex">The vertex shader uniform slot to push data to.</param>
    public void PushVertexShaderUniformMatrix(in Matrix4x4 matrix, int slotIndex = 0)
    {
        PushVertexShaderUniformData(matrix, slotIndex);
    }

    /// <summary>
    ///     Pushes the specified data to a fragment shader uniform slot. Subsequent draw calls will
    ///     use this uniform data.
    /// </summary>
    /// <param name="data">TODO.</param>
    /// <param name="startIndex">Index of the uniform slot to push data to.</param>
    /// <typeparam name="T">Data type. It must respect std140 layout conventions.</typeparam>
    public void PushFragmentShaderUniformData<T>(in T data, int startIndex = 0)
        where T : unmanaged
    {
        fixed (T* pointer = &data)
        {
            SDL_PushGPUFragmentUniformData(HandleTyped, (uint)startIndex, pointer, (uint)sizeof(T));
        }
    }

    /// <summary>
    ///     Pushes the specified <see cref="Rgba32F" /> color to a fragment shader uniform slot. Subsequent draw calls
    ///     will use this uniform data.
    /// </summary>
    /// <param name="color">The color to push.</param>
    /// <param name="slotIndex">The fragment shader uniform slot to push data to.</param>
    public void PushFragmentShaderUniformColor(in Rgba32F color, int slotIndex = 0)
    {
        PushFragmentShaderUniformData(color, slotIndex);
    }

    /// <summary>
    ///     Pushes data to a uniform slot on the command buffer.
    /// </summary>
    /// <param name="data">TODO.</param>
    /// <param name="startIndex">Index of the uniform slot to push data to.</param>
    /// <typeparam name="T">Data type. It must respect std140 layout conventions.</typeparam>
    public void PushComputeShaderUniformData<T>(in T data, int startIndex = 0)
        where T : unmanaged
    {
        fixed (T* pointer = &data)
        {
            SDL_PushGPUComputeUniformData(HandleTyped, (uint)startIndex, pointer, (uint)sizeof(T));
        }
    }

    /// <summary>
    ///     Cancels the command buffer, none of the enqueued commands are executed.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="Cancel" /> must be called from the thread the command buffer was acquired on.
    ///     </para>
    ///     <para>
    ///         It is an error to call this function after successfully calling <see cref="TryGetSwapchainTexture" />.
    ///     </para>
    /// </remarks>
    public void Cancel()
    {
        var result = SDL_CancelGPUCommandBuffer(HandleTyped);
        if (!result)
        {
            var errorMessage = Error.GetMessage();
            throw new InvalidOperationException(errorMessage);
        }

        HandleTyped = null;
        _ = TryToReturnToPool();
    }

    /// <summary>
    ///     Submits the command buffer to the device for executing the enqueued commands.
    /// </summary>
    public void Submit()
    {
        SubmitCore();
        _ = TryToReturnToPool();
    }

    /// <summary>
    ///     Blits from a source texture region to a destination texture region.
    /// </summary>
    /// <param name="info">The parameters for the blit.</param>
    /// <remarks>
    ///     <para>
    ///         <see cref="BlitTexture" /> must not be called inside of any <see cref="BeginRenderPass" /> and
    ///         <see cref="EndRenderPass" /> pair.
    ///     </para>
    /// </remarks>
    public void BlitTexture(GpuBlitInfo info)
    {
        var blitInfo = default(SDL_GPUBlitInfo);
        blitInfo.source.texture = info.Source.Texture.HandleTyped;
        blitInfo.source.mip_level = (uint)info.Source.MipMapLevel;
        blitInfo.source.layer_or_depth_plane = (uint)info.Source.LayerOrDepthPlane;
        blitInfo.source.x = (uint)info.Source.Bounds.X;
        blitInfo.source.y = (uint)info.Source.Bounds.Y;
        blitInfo.source.w = (uint)info.Source.Bounds.Width;
        blitInfo.source.h = (uint)info.Source.Bounds.Height;
        blitInfo.destination.texture = info.Destination.Texture.HandleTyped;
        blitInfo.destination.mip_level = (uint)info.Destination.MipMapLevel;
        blitInfo.destination.layer_or_depth_plane = (uint)info.Destination.LayerOrDepthPlane;
        blitInfo.destination.x = (uint)info.Destination.Bounds.X;
        blitInfo.destination.y = (uint)info.Destination.Bounds.Y;
        blitInfo.destination.w = (uint)info.Destination.Bounds.Width;
        blitInfo.destination.h = (uint)info.Destination.Bounds.Height;
        blitInfo.load_op = (SDL_GPULoadOp)info.LoadOperation;
        blitInfo.filter = (SDL_GPUFilter)info.FilterMode;
        blitInfo.cycle = info.IsDestinationTextureCycled;
        SDL_BlitGPUTexture(HandleTyped, &blitInfo);
    }

    internal void EndRenderPass(GpuRenderPass renderPass)
    {
        ThrowIfSubmitted();
        _ = renderPass.TryToReturnToPool();
    }

    internal void Set(SDL_GPUCommandBuffer* handle)
    {
        HandleTyped = handle;
        _isSubmitted = false;
    }

    [Conditional("DEBUG")]
    internal void ThrowIfSubmitted()
    {
        if (_isSubmitted)
        {
            throw new InvalidOperationException("Command buffer can not be used once submitted.");
        }
    }

    /// <inheritdoc />
    protected override void Reset()
    {
        SubmitCore();
        _isSubmitted = false;
    }

    private void SubmitCore()
    {
        if (HandleTyped == null)
        {
            return;
        }

        var isSubmitted = Interlocked.CompareExchange(ref _isSubmitted, true, false);
        if (isSubmitted)
        {
            ThrowIfSubmitted();
            return;
        }

        var isSuccess = SDL_SubmitGPUCommandBuffer(HandleTyped);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_SubmitGPUCommandBuffer));
        }

        HandleTyped = null;
    }
}
