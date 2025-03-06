// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Diagnostics;

namespace SDL.GPU;

/// <summary>
///     Represents a context for the CPU to queue up commands that are later all submitted at once to a
///     <see cref="GPU.Device" /> instance for execution.
/// </summary>
/// <remarks>
///     <para>
///         Commands only begin execution on the GPU once <see cref="Submit" /> is called. Multiple
///         <see cref="CommandBuffer" /> instances have their commands executed relative to the order the
///         <see cref="CommandBuffer" /> instance is submitted. For example, if you submit command buffer A and then
///         command buffer B, all commands in A will begin executing before any command in B begins executing.
///     </para>
///     <para>
///         <see cref="CommandBuffer" /> instances are pooled and must not be used or referenced after calling
///         <see cref="CommandBuffer.Submit" />. To get a <see cref="CommandBuffer" /> instance call
///         <see cref="GPU.Device.GetCommandBuffer" />.
///     </para>
///     <para>
///         In multi-threading scenarios, you should only use and submit a <see cref="CommandBuffer" /> instance on
///         the thread you acquired it from.
///     </para>
///     <para>
///         It is valid to acquire multiple <see cref="CommandBuffer" /> instances on the same thread at once.
///         In fact a common design pattern is to acquire two command buffers per frame: one for render and compute
///         passes and the other for copy passes and other preparatory work such as generating mipmaps. Interleaving
///         commands between the two command buffers reduces the total amount of passes overall which improves
///         rendering performance.
///     </para>
/// </remarks>
[PublicAPI]
public sealed unsafe class CommandBuffer : Poolable<CommandBuffer>
{
    private SDL_GPUCommandBuffer* _handle;
    private bool _isSubmitted;

    /// <summary>
    ///     Gets the <see cref="GPU.Device" /> instance associated with the command buffer.
    /// </summary>
    public Device Device { get; }

    internal CommandBuffer(Device device)
    {
        Device = device;
        _handle = null;
    }

    /// <summary>
    ///     Blocks until a swapchain render-target texture is available for the specified <see cref="Window" /> and then
    ///     acquires the swapchain render-target texture.
    /// </summary>
    /// <param name="window">The <see cref="Window" />.</param>
    /// <param name="swapchainTexture">
    ///     The render-target <see cref="Texture" /> which will be presented to the <paramref name="window" /> when
    ///     <see cref="CommandBuffer.Submit" /> is called.
    /// </param>
    /// <returns><c>true</c> if the texture was successful acquired; otherwise, <c>false</c>.</returns>
    /// <exception cref="InvalidOperationException">The command buffer is submitted to the device.</exception>
    /// <remarks>
    ///     <para>
    ///         It is an error to acquire two swapchain textures from the same window using the same command buffer.
    ///     </para>
    /// </remarks>
    public bool TryGetSwapchainTexture(Window window, out Texture? swapchainTexture)
    {
        ThrowIfSubmitted();

        SDL_GPUTexture* textureSwapchainHandle;
        uint width;
        uint height;

        if (!SDL_WaitAndAcquireGPUSwapchainTexture(
                _handle,
                (SDL_Window*)window.Handle,
                &textureSwapchainHandle,
                &width,
                &height))
        {
            var errorMessage = Error.GetMessage();
            Device.LogNativeFunctionFailed(nameof(SDL_WaitAndAcquireGPUSwapchainTexture), errorMessage);
            swapchainTexture = null;
            return false;
        }

        if (textureSwapchainHandle == null)
        {
            swapchainTexture = null;
            return false;
        }

        swapchainTexture = window.Swapchain!.Texture;
        swapchainTexture.UpdateTextureSwapchain((IntPtr)textureSwapchainHandle, (int)width, (int)height);
        return true;
    }

    /// <summary>
    ///     Begins a render pass.
    /// </summary>
    /// <param name="depthStencilTargetInfo">The depth-stencil render-target to use in the render pass.</param>
    /// <param name="colorTargetInfos">The color render-targets to use in the render pass.</param>
    /// <returns>A pooled <see cref="RenderPass" /> instance.</returns>
    public RenderPass BeginRenderPass(
        in RenderTargetInfoDepthStencil? depthStencilTargetInfo = null,
        params Span<RenderTargetInfoColor> colorTargetInfos)
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
            destination.load_op = (SDL_GPULoadOp)source.LoadOp;
            destination.store_op = (SDL_GPUStoreOp)source.StoreOp;
            destination.stencil_load_op = (SDL_GPULoadOp)source.StencilLoadOp;
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
            destination.load_op = (SDL_GPULoadOp)source.LoadOp;
            destination.store_op = (SDL_GPUStoreOp)source.StoreOp;
            destination.resolve_texture = (SDL_GPUTexture*)(source.ResolveTexture?.Handle ?? IntPtr.Zero);
            destination.resolve_mip_level = (uint)source.ResolveMipMapLevel;
            destination.resolve_layer = (uint)source.ResolveLayer;
            destination.cycle = source.IsTextureCycled;
            destination.cycle_resolve_texture = source.IsResolveTextureCycled;
        }

        var handle = SDL_BeginGPURenderPass(
            _handle, colorTargetInfosPointer, (uint)colorTargetInfos.Length, depthStencilInfoPointer);
        if (handle == null)
        {
            var errorMessage = Error.GetMessage();
            Device.LogNativeFunctionFailed(nameof(SDL_BeginGPURenderPass), errorMessage);
            throw new InvalidOperationException(errorMessage);
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
    ///         <see cref="CopyPass.End"/> must be called before starting another copy pass, render pass, or compute
    ///         pass.
    ///     </para>
    /// </remarks>
    /// <returns>A pooled <see cref="CopyPass" /> instance.</returns>
    public CopyPass BeginCopyPass()
    {
        ThrowIfSubmitted();

        var handle = SDL_BeginGPUCopyPass(_handle);
        var copyPass = new CopyPass(Device, (IntPtr)handle, this);
        return copyPass;
    }

    /// <summary>
    ///     Pushes the specified <see cref="Matrix4x4" /> to a vertex shader uniform slot. Subsequent draw calls will
    ///     use this uniform data.
    /// </summary>
    /// <param name="matrix">The matrix to push.</param>
    /// <param name="slotIndex">The vertex shader uniform slot to push data to.</param>
    public void PushVertexShaderUniformMatrix(in Matrix4x4 matrix, int slotIndex = 0)
    {
        fixed (Matrix4x4* pointer = &matrix)
        {
            SDL_PushGPUVertexUniformData(_handle, (uint)slotIndex, pointer, (uint)sizeof(Matrix4x4));
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
        fixed (Rgba32F* pointer = &color)
        {
            SDL_PushGPUFragmentUniformData(_handle, (uint)slotIndex, pointer, (uint)sizeof(Rgba32F));
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
        var result = SDL_CancelGPUCommandBuffer(_handle);
        if (!result)
        {
            var errorMessage = Error.GetMessage();
            throw new InvalidOperationException(errorMessage);
        }

        _handle = null;
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

    internal void EndRenderPass(RenderPass renderPass)
    {
        ThrowIfSubmitted();
        _ = renderPass.TryToReturnToPool();
    }

    internal void Set(SDL_GPUCommandBuffer* handle)
    {
        _handle = handle;
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
        if (_handle == null)
        {
            return;
        }

        var isSubmitted = Interlocked.CompareExchange(ref _isSubmitted, true, false);
        if (isSubmitted)
        {
            ThrowIfSubmitted();
            return;
        }

        var isSuccess = SDL_SubmitGPUCommandBuffer(_handle);
        if (!isSuccess)
        {
            var errorMessage = Error.GetMessage();
            Device.LogNativeFunctionFailed(nameof(SDL_SubmitGPUCommandBuffer), errorMessage);
        }

        _handle = null;
    }
}
