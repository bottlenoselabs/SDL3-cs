// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Represents the context to a GPU for drawing graphics and running computations.
/// </summary>
[PublicAPI]
public sealed unsafe partial class Device : NativeHandle
{
    internal readonly Pool<RenderPass> PoolRenderPass;
    internal readonly ArenaNativeAllocator Allocator;

    private readonly ILogger<Device> _logger;
    private readonly Pool<CommandBuffer> _poolCommandBuffer;

    /// <summary>
    ///     Gets the <see cref="GPU.Driver " />.
    /// </summary>
    public Driver Driver { get; }

    /// <summary>
    ///     Gets the supported <see cref="GraphicsShaderFormats" />.
    /// </summary>
    /// <returns>The supported <see cref="GraphicsShaderFormats" />.</returns>
    public GraphicsShaderFormats SupportedShaderFormats { get; }

    /// <summary>
    ///     Gets the supported depth-stencil <see cref="TextureFormat" />.
    /// </summary>
    /// <returns>The supported depth-stencil <see cref="TextureFormat" />.</returns>
    public TextureFormat SupportedDepthStencilTargetFormat { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Device" /> class.
    /// </summary>
    /// <param name="logger">The logger instance to use.</param>
    /// <param name="graphicsShaderFormats">The <see cref="GraphicsShaderFormats" /> to use.</param>
    /// <param name="driver">
    ///     The <see cref="GPU.Driver" /> to use. If <c>null</c>, the optimal driver for the current operating
    ///     system is automatically selected.
    /// </param>
    /// <param name="isDebugMode">Whether to enable debug properties and validations.</param>
    /// <param name="isLowPowerMode">Whether to prefer energy efficiency over maximum GPU performance.</param>
    /// <param name="temporaryBufferSize">The size in bytes of the buffer used for temporary allocations.</param>
    /// <exception cref="ArgumentNullException"><paramref name="logger" /> is not specified.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="graphicsShaderFormats " /> is not specified.</exception>
    /// <exception cref="InvalidOperationException">Failed to create the device.</exception>
    /// <exception cref="NotImplementedException">Unknown GPU driver.</exception>
    public Device(
        ILogger<Device> logger,
        GraphicsShaderFormats graphicsShaderFormats,
        Driver? driver = null,
        bool isDebugMode = true,
        bool isLowPowerMode = false,
        int temporaryBufferSize = 1024)
        : base(IntPtr.Zero)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;

        if (graphicsShaderFormats == GraphicsShaderFormats.None)
        {
            throw new ArgumentOutOfRangeException(nameof(graphicsShaderFormats));
        }

        var driverName = driver switch
        {
            Driver.Vulkan => "vulkan",
            Driver.DirectX12 => "direct3d12",
            Driver.Metal => "metal",
            _ => string.Empty
        };

        Allocator = new ArenaNativeAllocator(1024);
        var properties = SDL_CreateProperties();
        SDL_SetBooleanProperty(properties, SDL_PROP_GPU_DEVICE_CREATE_DEBUGMODE_BOOLEAN, isDebugMode);
        SDL_SetBooleanProperty(properties, SDL_PROP_GPU_DEVICE_CREATE_PREFERLOWPOWER_BOOLEAN, isLowPowerMode);

        if (!string.IsNullOrEmpty(driverName))
        {
            var driverNameC = Allocator.AllocateCString(driverName);
            SDL_SetStringProperty(properties, SDL_PROP_GPU_DEVICE_CREATE_NAME_STRING, driverNameC);
        }

        if ((graphicsShaderFormats & GraphicsShaderFormats.Private) != 0)
        {
            SDL_SetBooleanProperty(properties, SDL_PROP_GPU_DEVICE_CREATE_SHADERS_PRIVATE_BOOLEAN, true);
        }

        if ((graphicsShaderFormats & GraphicsShaderFormats.SPIRV) != 0)
        {
            SDL_SetBooleanProperty(properties, SDL_PROP_GPU_DEVICE_CREATE_SHADERS_SPIRV_BOOLEAN, true);
        }

        if ((graphicsShaderFormats & GraphicsShaderFormats.DXBC) != 0)
        {
            SDL_SetBooleanProperty(properties, SDL_PROP_GPU_DEVICE_CREATE_SHADERS_DXBC_BOOLEAN, true);
        }

        if ((graphicsShaderFormats & GraphicsShaderFormats.DXIL) != 0)
        {
            SDL_SetBooleanProperty(properties, SDL_PROP_GPU_DEVICE_CREATE_SHADERS_DXIL_BOOLEAN, true);
        }

        if ((graphicsShaderFormats & GraphicsShaderFormats.MSL) != 0)
        {
            SDL_SetBooleanProperty(properties, SDL_PROP_GPU_DEVICE_CREATE_SHADERS_MSL_BOOLEAN, true);
        }

        if ((graphicsShaderFormats & GraphicsShaderFormats.MetalLib) != 0)
        {
            SDL_SetBooleanProperty(properties, SDL_PROP_GPU_DEVICE_CREATE_SHADERS_METALLIB_BOOLEAN, true);
        }

        Handle = (IntPtr)SDL_CreateGPUDeviceWithProperties(properties);
        SDL_DestroyProperties(properties);
        Allocator.Reset();

        if (Handle == IntPtr.Zero)
        {
            var errorMessage = Error.GetMessage();
            LogNativeFunctionFailed(nameof(SDL_CreateGPUDeviceWithProperties), errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        var driverNameCActual = SDL_GetGPUDeviceDriver((SDL_GPUDevice*)Handle);
        var driverNameActual = CString.ToString(driverNameCActual);
        Driver = driverNameActual switch
        {
            "vulkan" => Driver.Vulkan,
            "direct3d12" => Driver.DirectX12,
            "metal" => Driver.Metal,
            _ => throw new NotImplementedException()
        };

        _poolCommandBuffer = new Pool<CommandBuffer>(_logger, () => new CommandBuffer(this), "GpuCommandBuffers");
        PoolRenderPass = new Pool<RenderPass>(_logger, () => new RenderPass(this), "GpuRenderPasses");

        SupportedShaderFormats = (GraphicsShaderFormats)(uint)SDL_GetGPUShaderFormats((SDL_GPUDevice*)Handle);

        if (SDL_GPUTextureSupportsFormat(
                (SDL_GPUDevice*)Handle,
                SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D24_UNORM_S8_UINT,
                SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_2D,
                SDL_GPU_TEXTUREUSAGE_DEPTH_STENCIL_TARGET))
        {
            SupportedDepthStencilTargetFormat = (TextureFormat)SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D24_UNORM_S8_UINT;
        }
        else if (SDL_GPUTextureSupportsFormat(
                     (SDL_GPUDevice*)Handle,
                     SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D32_FLOAT_S8_UINT,
                     SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_2D,
                     SDL_GPU_TEXTUREUSAGE_DEPTH_STENCIL_TARGET))
        {
            SupportedDepthStencilTargetFormat = (TextureFormat)SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D32_FLOAT_S8_UINT;
        }
        else
        {
            SupportedDepthStencilTargetFormat = TextureFormat.Invalid;
        }
    }

    /// <summary>
    ///     Attempts to claim the specified <see cref="Window" /> instance and associate it with the device. If
    ///     successful, creates a <see cref="Swapchain" /> instance associated with the device and the window.
    /// </summary>
    /// <param name="window">The <see cref="Window" /> instance to claim.</param>
    /// <returns><c>true</c> if the window was successfully claimed; otherwise, <c>false</c>.</returns>
    public bool TryClaimWindow(Window window)
    {
        if (window.IsClaimed)
        {
            return false;
        }

        var isSuccess = SDL_ClaimWindowForGPUDevice((SDL_GPUDevice*)Handle, (SDL_Window*)window.Handle);
        if (!isSuccess)
        {
            var errorMessage = Error.GetMessage();
            LogNativeFunctionFailed(nameof(SDL_ClaimWindowForGPUDevice), errorMessage);
            return false;
        }

        var swapchain = new Swapchain(this, window);
        window.Swapchain = swapchain;
        return true;
    }

    /// <summary>
    ///     Releases the specified <see cref="Window" /> instance from the device, destroying the
    ///     <see cref="Swapchain" /> instance associated with the device and window.
    /// </summary>
    /// <param name="window">The <see cref="Window" /> instance.</param>
    /// <exception cref="InvalidOperationException">The window is not claimed by a device.</exception>
    public void ReleaseWindow(Window window)
    {
        if (!window.IsClaimed)
        {
            return;
        }

        window.Swapchain!.Dispose();
        window.Swapchain = null;
    }

    /// <summary>
    ///     Acquires a <see cref="CommandBuffer" /> pooled instance associated with the device.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         In multi-threading scenarios, calling <see cref="GetCommandBuffer" /> is safe. However, the returned
    ///         <see cref="CommandBuffer" /> is not thread-safe; it must only be used and submitted on
    ///         the thread it was acquired on.
    ///     </para>
    ///     <para>
    ///         It is valid to acquire multiple <see cref="CommandBuffer" /> instances on the same thread at once.
    ///         It's even a common design pattern to acquire two command buffers per frame: one for render and compute
    ///         passes and the other for copy passes and other preparatory work such as generating mipmaps. Interleaving
    ///         commands between the two command buffers reduces the total amount of passes overall which improves
    ///         rendering performance.
    ///     </para>
    /// </remarks>
    /// <returns>A pooled <see cref="CommandBuffer" /> instance.</returns>
    public CommandBuffer GetCommandBuffer()
    {
        var handle = SDL_AcquireGPUCommandBuffer((SDL_GPUDevice*)Handle);
        if (handle == null)
        {
            var errorMessage = Error.GetMessage();
            LogNativeFunctionFailed(nameof(SDL_AcquireGPUCommandBuffer), errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        var commandBuffer = _poolCommandBuffer.GetOrCreate()!;
        commandBuffer.Set(handle);
        return commandBuffer;
    }

    /// <summary>
    ///     Attempts to create a new <see cref="GraphicsShader" /> instance.
    /// </summary>
    /// <param name="descriptor">The parameters for creating the shader.</param>
    /// <param name="shader">If successful, a new <see cref="GraphicsShader" /> instance; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the shader was successfully created; otherwise, <c>false</c>.</returns>
    public bool TryCreateShader(GraphicsShaderDescriptor descriptor, out GraphicsShader? shader)
    {
        SDL_GPUShaderCreateInfo info = default;
        info.code = (byte*)descriptor.DataPointer;
        info.code_size = (ulong)descriptor.DataSize;
        info.entrypoint = descriptor.Allocator.AllocateCString(descriptor.EntryPoint);
        var shaderFormat = (SDL_GPUShaderFormat)(uint)descriptor.Format;
        info.format = shaderFormat;
        info.stage = (SDL_GPUShaderStage)descriptor.Stage;
        info.num_samplers = (uint)Math.Max(descriptor.SamplerCount, 0);
        info.num_uniform_buffers = (uint)Math.Max(descriptor.UniformBufferCount, 0);
        info.num_storage_buffers = (uint)Math.Max(descriptor.StorageBufferCount, 0);
        info.num_storage_textures = (uint)Math.Max(descriptor.StorageTextureCount, 0);

        var handle = SDL_CreateGPUShader((SDL_GPUDevice*)Handle, &info);
        if (handle == null)
        {
            var errorMessage = Error.GetMessage();
            LogNativeFunctionFailed(nameof(SDL_CreateGPUShader), errorMessage);
            shader = null;
            return false;
        }

        shader = new GraphicsShader(
            this, (IntPtr)handle, descriptor.Format, descriptor.Stage);
        return true;
    }

    /// <summary>
    ///     Attempts to create a new <see cref="GraphicsShader" /> instance using the specified file path to the
    ///     shader file.
    /// </summary>
    /// <param name="fileSystem">The <see cref="FileSystem" /> instance.</param>
    /// <param name="filePath">
    ///     The file path to the shader file. If the path is relative, it is assumed to be relative to
    ///     <see cref="AppContext.BaseDirectory" />.
    /// </param>
    /// <param name="shader">If successful, a new <see cref="GraphicsShader" /> instance; otherwise, <c>null</c>.</param>
    /// <param name="samplerCount">The number of samplers used in the shader.</param>
    /// <param name="uniformBufferCount">The number of uniform buffers used in the shader.</param>
    /// <returns><c>true</c> if the shader was successfully created; otherwise, <c>false</c>.</returns>
    public bool TryCreateShaderFromFile(
        FileSystem fileSystem,
        string filePath,
        out GraphicsShader? shader,
        int samplerCount = 0,
        int uniformBufferCount = 0)
    {
#pragma warning disable CA2000
        if (!fileSystem.TryLoadFile(filePath, out var file))
#pragma warning restore CA2000
        {
            shader = null;
            return false;
        }

        using var descriptor = new GraphicsShaderDescriptor();
        descriptor.SamplerCount = samplerCount;
        descriptor.UniformBufferCount = uniformBufferCount;
        if (!descriptor.TrySetFromFile(file))
        {
            shader = null;
            file.Dispose();
            return false;
        }

        if (!TryCreateShader(descriptor, out shader))
        {
            shader = null;
            file.Dispose();
            return false;
        }

        file.Dispose();
        return true;
    }

    /// <summary>
    ///     Attempts to create a new <see cref="GraphicsPipeline" /> instance.
    /// </summary>
    /// <param name="descriptor">The parameters for creating the pipeline.</param>
    /// <param name="pipeline">
    ///     If successful, a new <see cref="GraphicsPipeline" /> instance; otherwise, <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if the pipeline was successfully created; otherwise, <c>false</c>.</returns>
    public bool TryCreatePipeline(GraphicsPipelineDescriptor descriptor, out GraphicsPipeline? pipeline)
    {
        var info = default(SDL_GPUGraphicsPipelineCreateInfo);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (descriptor.VertexShader == null)
        {
            throw new InvalidOperationException("Vertex shader can not be null.");
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (descriptor.FragmentShader == null)
        {
            throw new InvalidOperationException("Fragment shader can not be null.");
        }

        info.vertex_shader = (SDL_GPUShader*)descriptor.VertexShader.Handle;
        info.fragment_shader = (SDL_GPUShader*)descriptor.FragmentShader.Handle;

        ref var vertexInputState = ref info.vertex_input_state;
        vertexInputState.vertex_buffer_descriptions =
            (SDL_GPUVertexBufferDescription*)descriptor.VertexInputState.VertexBufferDescriptionsArrayPointer;
        vertexInputState.num_vertex_buffers = (uint)descriptor.VertexInputState.VertexBufferDescriptionCount;
        vertexInputState.vertex_attributes = (SDL_GPUVertexAttribute*)descriptor.VertexInputState.VertexAttributesArrayPointer;
        vertexInputState.num_vertex_attributes = (uint)descriptor.VertexInputState.VertexAttributeCount;

        info.primitive_type = (SDL_GPUPrimitiveType)descriptor.PrimitiveType;

        ref var rasterizerState = ref info.rasterizer_state;
        {
            rasterizerState.fill_mode = (SDL_GPUFillMode)descriptor.RasterizerState.FillMode;
            rasterizerState.cull_mode = (SDL_GPUCullMode)descriptor.RasterizerState.CullMode;
            rasterizerState.front_face = (SDL_GPUFrontFace)descriptor.RasterizerState.FrontFace;
            rasterizerState.depth_bias_constant_factor = descriptor.RasterizerState.DepthBiasConstantFactor;
            rasterizerState.depth_bias_clamp = descriptor.RasterizerState.DepthBiasClamp;
            rasterizerState.depth_bias_slope_factor = descriptor.RasterizerState.DepthBiasSlopeFactor;
            rasterizerState.enable_depth_bias = descriptor.RasterizerState.IsEnabledDepthBias;
            rasterizerState.enable_depth_clip = descriptor.RasterizerState.IsEnabledDepthClamp;
        }

        // TODO: multi-sample state

        ref var depthStencil = ref info.depth_stencil_state;
        {
            ref var stencilBack = ref depthStencil.back_stencil_state;
            stencilBack.fail_op = (SDL_GPUStencilOp)descriptor.DepthStencilState.BackStencilState.FailOp;
            stencilBack.pass_op = (SDL_GPUStencilOp)descriptor.DepthStencilState.BackStencilState.PassOp;
            stencilBack.depth_fail_op = (SDL_GPUStencilOp)descriptor.DepthStencilState.BackStencilState.DepthFailOp;
            stencilBack.compare_op = (SDL_GPUCompareOp)descriptor.DepthStencilState.BackStencilState.CompareOp;

            ref var stencilFront = ref depthStencil.front_stencil_state;
            stencilFront.fail_op = (SDL_GPUStencilOp)descriptor.DepthStencilState.FrontStencilState.FailOp;
            stencilFront.pass_op = (SDL_GPUStencilOp)descriptor.DepthStencilState.FrontStencilState.PassOp;
            stencilFront.depth_fail_op = (SDL_GPUStencilOp)descriptor.DepthStencilState.FrontStencilState.DepthFailOp;
            stencilFront.compare_op = (SDL_GPUCompareOp)descriptor.DepthStencilState.FrontStencilState.CompareOp;

            depthStencil.compare_op = (SDL_GPUCompareOp)descriptor.DepthStencilState.CompareOp;
            depthStencil.compare_mask = descriptor.DepthStencilState.ReadMask;
            depthStencil.write_mask = descriptor.DepthStencilState.WriteMask;
            depthStencil.enable_depth_test = descriptor.DepthStencilState.IsEnabledDepthTest;
            depthStencil.enable_depth_write = descriptor.DepthStencilState.IsEnabledDepthWrite;
            depthStencil.enable_stencil_test = descriptor.DepthStencilState.IsEnabledStencilTest;
        }

        ref var targetInfo = ref info.target_info;
        {
            targetInfo.num_color_targets = (uint)descriptor.ColorRenderTargets.Length;
            var colorTargetDescriptions =
                stackalloc SDL_GPUColorTargetDescription[descriptor.ColorRenderTargets.Length];
            for (var i = 0; i < descriptor.ColorRenderTargets.Length; i++)
            {
                ref var colorTargetTo = ref colorTargetDescriptions[i];
                var colorTargetFrom = descriptor.ColorRenderTargets[i];
                colorTargetTo.format = (SDL_GPUTextureFormat)colorTargetFrom.Format;

                ref var blendStateTo = ref colorTargetTo.blend_state;
                var blendStateFrom = colorTargetFrom.BlendState;
                blendStateTo.src_color_blendfactor = (SDL_GPUBlendFactor)blendStateFrom.SourceColorBlendFactor;
                blendStateTo.dst_color_blendfactor = (SDL_GPUBlendFactor)blendStateFrom.DestinationColorBlendFactor;
                blendStateTo.color_blend_op = blendStateFrom.ColorBlendOp;
                blendStateTo.src_alpha_blendfactor = (SDL_GPUBlendFactor)blendStateFrom.SourceAlphaBlendFactor;
                blendStateTo.dst_alpha_blendfactor = (SDL_GPUBlendFactor)blendStateFrom.DestinationAlphaBlendFactor;
                blendStateTo.alpha_blend_op = blendStateFrom.AlphaBlendOp;
                blendStateTo.color_write_mask = blendStateFrom.ColorWriteMask;
                blendStateTo.enable_blend = blendStateFrom.IsEnabledBlend;
                blendStateTo.enable_color_write_mask = blendStateFrom.IsEnabledColorWriteMask;
            }

            targetInfo.color_target_descriptions = colorTargetDescriptions;
            targetInfo.num_color_targets = (uint)descriptor.ColorRenderTargets.Length;
            targetInfo.depth_stencil_format = (SDL_GPUTextureFormat)descriptor.DepthStencilRenderTargetFormat;
            targetInfo.has_depth_stencil_target = descriptor.IsEnabledDepthStencilRenderTarget;
        }

        var handle = SDL_CreateGPUGraphicsPipeline(
            (SDL_GPUDevice*)Handle, &info);
        if (handle == null)
        {
            var errorMessage = Error.GetMessage();
            LogNativeFunctionFailed(nameof(SDL_CreateGPUShader), errorMessage);
            pipeline = null;
            return false;
        }

        pipeline = new GraphicsPipeline(this, (IntPtr)handle);
        return true;
    }

    /// <summary>
    ///     Attempts to create a new <see cref="DataBuffer" /> instance.
    /// </summary>
    /// <param name="elementCount">
    ///     The maximum number of <typeparamref name="TElement" /> elements the buffer can hold.
    /// </param>
    /// <param name="buffer">
    ///     If successful, a new <see cref="DataBuffer" /> instance; otherwise, <c>null</c>.
    /// </param>
    /// <param name="name">The optional name of the data buffer.</param>
    /// <typeparam name="TElement">The type of data buffer element.</typeparam>
    /// <returns><c>true</c> if the data buffer was successfully created; otherwise, <c>false</c>.</returns>
    public bool TryCreateDataBuffer<TElement>(int elementCount, out DataBuffer? buffer, string? name = null)
        where TElement : unmanaged
    {
        var bufferCreateInfo = default(SDL_GPUBufferCreateInfo);
        bufferCreateInfo.usage = SDL_GPU_BUFFERUSAGE_VERTEX;
        bufferCreateInfo.size = (uint)(sizeof(TElement) * elementCount);
        var handle = SDL_CreateGPUBuffer((SDL_GPUDevice*)Handle, &bufferCreateInfo);
        if (handle == null)
        {
            var errorMessage = Error.GetMessage();
            LogNativeFunctionFailed(nameof(SDL_CreateGPUBuffer), errorMessage);
            buffer = null;
            return false;
        }

        buffer = new DataBuffer(this, (IntPtr)handle);

        if (name != null)
        {
            var allocator = Allocator;
            allocator.Reset();
            var nameCString = allocator.AllocateCString(name);
            SDL_SetGPUBufferName((SDL_GPUDevice*)Handle, handle, nameCString);
            allocator.Reset();
        }

        return true;
    }

    /// <summary>
    ///     Attempts to create a new transfer <see cref="TransferBuffer" /> instance.
    /// </summary>
    /// <param name="size">
    ///     The size of the transfer buffer in bytes.
    /// </param>
    /// <param name="transferBuffer">
    ///     If successful, a new <see cref="TransferBuffer" /> instance; otherwise, <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if the transfer buffer was successfully created; otherwise, <c>false</c>.</returns>
    public bool TryCreateTransferBuffer(int size, out TransferBuffer? transferBuffer)
    {
        var transferBufferCreateInfo = default(SDL_GPUTransferBufferCreateInfo);
        transferBufferCreateInfo.usage = SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD;
        transferBufferCreateInfo.size = (uint)size;
        var handle = SDL_CreateGPUTransferBuffer(
            (SDL_GPUDevice*)Handle, &transferBufferCreateInfo);
        if (handle == null)
        {
            var errorMessage = Error.GetMessage();
            LogNativeFunctionFailed(nameof(SDL_CreateGPUTransferBuffer), errorMessage);
            transferBuffer = null;
            return false;
        }

        transferBuffer = new TransferBuffer(
            this, (IntPtr)handle, (int)transferBufferCreateInfo.size);

        return true;
    }

    /// <summary>
    ///     Attempts to create a new <see cref="Texture" /> instance.
    /// </summary>
    /// <param name="descriptor">The parameters for creating a texture.</param>
    /// <param name="texture">
    ///     If successful, a new <see cref="Texture" /> instance; otherwise, <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if the texture was successfully created; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentException">Sample count must be 1, 2, 4, 8.</exception>
    public bool TryCreateTexture(TextureDescriptor descriptor, out Texture? texture)
    {
        var textureCreateInfo = default(SDL_GPUTextureCreateInfo);
        textureCreateInfo.type = (SDL_GPUTextureType)descriptor.Type;
        textureCreateInfo.width = (uint)descriptor.Width;
        textureCreateInfo.height = (uint)descriptor.Height;
        textureCreateInfo.layer_count_or_depth = (uint)descriptor.LayerCountOrDepth;
        textureCreateInfo.num_levels = (uint)descriptor.MipmapLevelCount;
        textureCreateInfo.format = (SDL_GPUTextureFormat)descriptor.Format;
        textureCreateInfo.usage = (uint)descriptor.Usage;

        textureCreateInfo.sample_count = descriptor.SampleCount switch
        {
            0 => SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1,
            1 => SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1,
            2 => SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_2,
            4 => SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_4,
            8 => SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_8,
            _ => throw new ArgumentException("Sample count must be 1, 2, 4, or 8.")
        };

        var handle = SDL_CreateGPUTexture((SDL_GPUDevice*)Handle, &textureCreateInfo);
        if (handle == null)
        {
            var errorMessage = Error.GetMessage();
            LogNativeFunctionFailed(nameof(SDL_CreateGPUTexture), errorMessage);
            texture = null;
            return false;
        }

        texture = new Texture(
            this,
            (IntPtr)handle,
            descriptor.Type,
            descriptor.Format,
            descriptor.Width,
            descriptor.Height,
            descriptor.LayerCountOrDepth,
            descriptor.MipmapLevelCount,
            descriptor.SampleCount,
            descriptor.Usage);

        var nameCString = descriptor.Allocator.AllocateCString(descriptor.Name);
        SDL_SetGPUTextureName((SDL_GPUDevice*)Handle, handle, nameCString);

        return true;
    }

    /// <summary>
    ///     Attempts to create a new <see cref="Sampler" /> instance.
    /// </summary>
    /// <param name="descriptor">The parameters for creating a sampler.</param>
    /// <param name="sampler">
    ///     If successful, a new <see cref="Sampler" /> instance; otherwise, <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if the sampler was successfully created; otherwise, <c>false</c>.</returns>
    public bool TryCreateSampler(SamplerDescriptor descriptor, out Sampler? sampler)
    {
        var samplerCreateInfo = default(SDL_GPUSamplerCreateInfo);
        samplerCreateInfo.min_filter = (SDL_GPUFilter)descriptor.MinificationFilter;
        samplerCreateInfo.mag_filter = (SDL_GPUFilter)descriptor.MagnificationFilter;
        samplerCreateInfo.mipmap_mode = (SDL_GPUSamplerMipmapMode)descriptor.MipMapMode;
        samplerCreateInfo.address_mode_u = (SDL_GPUSamplerAddressMode)descriptor.AddressModeU;
        samplerCreateInfo.address_mode_v = (SDL_GPUSamplerAddressMode)descriptor.AddressModeV;
        samplerCreateInfo.address_mode_w = (SDL_GPUSamplerAddressMode)descriptor.AddressModeW;
        samplerCreateInfo.mip_lod_bias = 0.0f; // FIXME: This is only used in DirectX12/Vulkan and not cross-platform?
        samplerCreateInfo.max_anisotropy = descriptor.MaximumAnisotropy;
        samplerCreateInfo.compare_op = (SDL_GPUCompareOp)descriptor.DepthCompareOp;
        samplerCreateInfo.min_lod = descriptor.MinimumLevelOfDetailClamp;
        samplerCreateInfo.max_lod = descriptor.MaximumLevelOfDetailClamp;
        samplerCreateInfo.enable_anisotropy = descriptor.IsEnabledAnisotropy;
        samplerCreateInfo.enable_compare = descriptor.IsEnabledDepthCompare;

        var handle = SDL_CreateGPUSampler((SDL_GPUDevice*)Handle, &samplerCreateInfo);
        if (handle == null)
        {
            var errorMessage = Error.GetMessage();
            LogNativeFunctionFailed(nameof(SDL_CreateGPUSampler), errorMessage);
            sampler = null;
            return false;
        }

        sampler = new Sampler(this, (IntPtr)handle);

        return true;
    }

    internal void EndRenderPassTryInternal(RenderPass renderPass)
    {
        if (renderPass.Handle == null)
        {
            return;
        }

        renderPass.CommandBuffer.ThrowIfSubmitted();

        var handle = renderPass.Handle;
        renderPass.Handle = null;
        renderPass.CommandBuffer = null!;
        SDL_EndGPURenderPass(handle);
    }

    [LoggerMessage(LogEventId.NativeFunctionFailed, LogLevel.Error, "Native function failed: {FunctionName}. {ErrorMessage}")]
    internal partial void LogNativeFunctionFailed(string functionName, string errorMessage);

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        PoolRenderPass.Dispose();
        _poolCommandBuffer.Dispose();

        SDL_DestroyGPUDevice((SDL_GPUDevice*)Handle);

        base.Dispose(isDisposing);

        Allocator.Dispose();
    }
}
