// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Represents a context for applying render state and issuing draw calls to one or more render-targets
///     (e.g. colors, depth, stencil) along with their load/store operations (e.g. clearing, discarding, writing).
/// </summary>
/// <remarks>
///     <para>
///         <see cref="RenderPass" /> instances are pooled and must not be used or referenced after calling
///         <see cref="RenderPass.End" />. To get a <see cref="RenderPass" /> instance call
///         <see cref="GPU.CommandBuffer.BeginRenderPass" />.
///     </para>
/// </remarks>
[PublicAPI]
public sealed unsafe class RenderPass : Poolable<RenderPass>
{
#pragma warning disable SA1401
    internal SDL_GPURenderPass* Handle;
#pragma warning restore SA1401

    private bool _isPipelineBound;

    /// <summary>
    ///     Gets the <see cref="GPU.Device" /> instance associated with the render pass.
    /// </summary>
    public Device Device { get; }

    /// <summary>
    ///     Gets the <see cref="CommandBuffer" /> instance associated with the render pass.
    /// </summary>
    public CommandBuffer CommandBuffer { get; internal set; }

    internal RenderPass(Device device)
    {
        Device = device;
        CommandBuffer = null!;
        Handle = null;
    }

    /// <summary>
    ///     Binds a graphics pipeline for use with subsequent draw calls.
    /// </summary>
    /// <param name="pipeline">The graphics pipeline.</param>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline" /> is null.</exception>
    public void BindPipeline(GraphicsPipeline? pipeline)
    {
        ArgumentNullException.ThrowIfNull(pipeline);
        SDL_BindGPUGraphicsPipeline(Handle, (SDL_GPUGraphicsPipeline*)pipeline.Handle);
        _isPipelineBound = true;
    }

    /// <summary>
    ///     Binds a vertex buffer for use with subsequent draw calls.
    /// </summary>
    /// <param name="vertexBuffer">The vertex buffer.</param>
    /// <param name="offset">The starting byte index of the data to bind in the vertex buffer.</param>
    /// <exception cref="ArgumentNullException"><paramref name="vertexBuffer" /> is null.</exception>
    public void BindVertexBuffer(DataBuffer? vertexBuffer, int offset = 0)
    {
        ArgumentNullException.ThrowIfNull(vertexBuffer);
        var bufferBinding = default(SDL_GPUBufferBinding);
        bufferBinding.buffer = (SDL_GPUBuffer*)vertexBuffer.Handle;
        bufferBinding.offset = (uint)offset;
        SDL_BindGPUVertexBuffers(Handle, 0, &bufferBinding, 1);
    }

    /// <summary>
    ///     Binds an index buffer for use with subsequent draw calls.
    /// </summary>
    /// <param name="buffer">The index buffer.</param>
    /// <param name="offset">The starting byte index of the data to bind in the index buffer.</param>
    /// <param name="indexElementSize">The size of each index element.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="indexElementSize" /> is not 2 nor 4.</exception>
    public void BindIndexBuffer(DataBuffer? buffer, int offset = 0, int indexElementSize = 2)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (indexElementSize != 2 && indexElementSize != 4)
        {
            throw new ArgumentOutOfRangeException(
                nameof(indexElementSize), "Index element size in bytes must be either 2 or 4.");
        }

        var indexElementSize2 = indexElementSize == 2
            ? SDL_GPUIndexElementSize.SDL_GPU_INDEXELEMENTSIZE_16BIT
            : SDL_GPUIndexElementSize.SDL_GPU_INDEXELEMENTSIZE_32BIT;

        var bufferBindingIndex = default(SDL_GPUBufferBinding);
        var handle = buffer?.Handle ?? IntPtr.Zero;
        bufferBindingIndex.buffer = (SDL_GPUBuffer*)handle;
        bufferBindingIndex.offset = (uint)offset;

        SDL_BindGPUIndexBuffer(Handle, &bufferBindingIndex, indexElementSize2);
    }

    /// <summary>
    ///     Binds a texture and sampler pairs for use with a fragment shader.
    /// </summary>
    /// <param name="texture">The texture.</param>
    /// <param name="sampler">The sampler.</param>
    /// <exception cref="ArgumentException">
    ///     <paramref name="texture" /> usage flags are not <see cref="TextureUsages.Sampler" />.
    /// </exception>
    public void BindFragmentSampler(Texture? texture, Sampler? sampler)
    {
        ArgumentNullException.ThrowIfNull(texture);
        ArgumentNullException.ThrowIfNull(sampler);

        if ((texture.Usages & TextureUsages.Sampler) == 0)
        {
            throw new ArgumentException("Texture usage flags are not TextureUsages.Sampler.");
        }

        var samplerBinding = default(SDL_GPUTextureSamplerBinding);
        samplerBinding.texture = (SDL_GPUTexture*)texture.Handle;
        samplerBinding.sampler = (SDL_GPUSampler*)sampler.Handle;
        SDL_BindGPUFragmentSamplers(Handle, 0, &samplerBinding, 1);
    }

    /// <summary>
    ///     Sets the <see cref="Viewport" /> of the render pass.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    public void SetViewport(in Viewport viewport)
    {
        // NOTE: Using the `in` keyword passes the struct by reference and is optimal when the scope is read-only.

        fixed (Viewport* pointer = &viewport)
        {
            // NOTE: Pointer casting assumes the two structs have the exact same memory layout.
            var pointerCasted = (SDL_GPUViewport*)pointer;
            SDL_SetGPUViewport(Handle, pointerCasted);
        }
    }

    /// <summary>
    ///     Sets the scissor <see cref="Rectangle" /> of the render pass.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    public void SetScissorRectangle(in Rectangle rectangle)
    {
        // NOTE: Using the `in` passes the struct by reference and is optimal when the scope is read-only.

        fixed (Rectangle* pointer = &rectangle)
        {
            // NOTE: Pointer casting assumes the two structs have the exact same memory layout.
            var pointerCasted = (SDL_Rect*)pointer;
            SDL_SetGPUScissor(Handle, pointerCasted);
        }
    }

    /// <summary>
    ///     Sets the stencil reference value of the render pass.
    /// </summary>
    /// <param name="value">The stencil reference value to set.</param>
    public void SetStencilReference(byte value)
    {
        SDL_SetGPUStencilReference(Handle, value);
    }

    /// <summary>
    ///     Draws primitives using the bound state.
    /// </summary>
    /// <param name="vertexCount">The number of vertices to draw.</param>
    /// <param name="instanceCount">The number of instances to draw.</param>
    /// <param name="firstVertexIndex">The index of the first vertex to draw.</param>
    /// <param name="firstInstanceId">The identifier of the first instance to draw.</param>
    /// <exception cref="InvalidOperationException">No graphics pipeline is bound.</exception>
    public void DrawPrimitives(
        int vertexCount, int instanceCount, int firstVertexIndex, int firstInstanceId)
    {
        if (!_isPipelineBound)
        {
            throw new InvalidOperationException("A graphics pipeline must be bound before drawing primitives.");
        }

        SDL_DrawGPUPrimitives(
            Handle,
            (uint)vertexCount,
            (uint)instanceCount,
            (uint)firstVertexIndex,
            (uint)firstInstanceId);
    }

    /// <summary>
    ///     Draws primitives using the bound state with an index buffer and instancing enabled.
    /// </summary>
    /// <param name="indexCount">The number of indices to draw per instance.</param>
    /// <param name="instanceCount">The number of instances to draw.</param>
    /// <param name="firstIndex">The starting index within the index buffer.</param>
    /// <param name="vertexOffset">The value added to vertex indexing before indexing into the vertex buffer.</param>
    /// <param name="firstInstanceId">The identifier of the first instance to draw.</param>
    /// <exception cref="InvalidOperationException">No graphics pipeline is bound.</exception>
    public void DrawPrimitivesIndexed(
        int indexCount, int instanceCount, int firstIndex, int vertexOffset, int firstInstanceId)
    {
        if (!_isPipelineBound)
        {
            throw new InvalidOperationException("A graphics pipeline must be bound before drawing primitives.");
        }

        SDL_DrawGPUIndexedPrimitives(
            Handle,
            (uint)indexCount,
            (uint)instanceCount,
            (uint)firstIndex,
            vertexOffset,
            (uint)firstInstanceId);
    }

    /// <summary>
    ///     Ends a render pass.
    /// </summary>
    /// <exception cref="InvalidOperationException">The associated command buffer was submitted.</exception>
    public void End()
    {
        Device.EndRenderPassTryInternal(this);
        _ = TryToReturnToPool();
    }

    /// <inheritdoc />
    protected override void Reset()
    {
        Device.EndRenderPassTryInternal(this);
        _isPipelineBound = false;
    }
}
