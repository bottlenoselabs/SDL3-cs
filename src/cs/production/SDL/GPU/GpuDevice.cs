// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Represents a context for rendering 3D graphics and running computations.
/// </summary>
[PublicAPI]
public sealed unsafe class GpuDevice : NativeHandleTyped<SDL_GPUDevice>
{
    internal readonly Pool<GpuRenderPass> PoolRenderPass;
    internal readonly ArenaNativeAllocator Allocator;

    private readonly ILogger<GpuDevice> _logger;
    private readonly Pool<GpuCommandBuffer> _poolCommandBuffer;

    /// <summary>
    ///     Gets the <see cref="GpuDriver " />.
    /// </summary>
    public GpuDriver Driver { get; }

    /// <summary>
    ///     Gets the supported <see cref="GpuShaderFormats" />.
    /// </summary>
    /// <returns>The supported <see cref="GpuShaderFormats" />.</returns>
    public GpuShaderFormats SupportedShaderFormats { get; }

    /// <summary>
    ///     Gets the supported depth-stencil <see cref="GpuTextureFormat" />.
    /// </summary>
    /// <returns>The supported depth-stencil <see cref="GpuTextureFormat" />.</returns>
    public GpuTextureFormat SupportedDepthStencilTargetFormat { get; }

    internal GpuDevice(ILogger<GpuDevice> logger, GpuDeviceOptions? options)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;

        options ??= new GpuDeviceOptions
        {
            ShaderFormats = GpuShaderFormats.None,
            Driver = null,
            IsDebugMode = true,
            IsLowPowerMode = false
        };

        var driverName = options.Driver switch
        {
            GpuDriver.Vulkan => "vulkan",
            GpuDriver.DirectX12 => "direct3d12",
            GpuDriver.Metal => "metal",
            _ => string.Empty
        };

        Allocator = new ArenaNativeAllocator(1024);
        var properties = SDL_CreateProperties();
        SDL_SetBooleanProperty(
            properties, SDL_PROP_GPU_DEVICE_CREATE_DEBUGMODE_BOOLEAN, options.IsDebugMode);
        SDL_SetBooleanProperty(
            properties, SDL_PROP_GPU_DEVICE_CREATE_PREFERLOWPOWER_BOOLEAN, options.IsLowPowerMode);

        if (!string.IsNullOrEmpty(driverName))
        {
            var driverNameC = Allocator.AllocateCString(driverName);
            SDL_SetStringProperty(properties, SDL_PROP_GPU_DEVICE_CREATE_NAME_STRING, driverNameC);
        }

        var shaderFormats = options.ShaderFormats;
        if (shaderFormats == GpuShaderFormats.None)
        {
            var platform = Application.Current.Platform;
            switch (platform)
            {
                case Platform.Windows:
                    shaderFormats |= GpuShaderFormats.DXIL;
                    break;
                case Platform.macOS:
                case Platform.iOS:
                    shaderFormats |= GpuShaderFormats.MetalLib;
                    break;
                case Platform.Linux:
                case Platform.Android:
                    shaderFormats |= GpuShaderFormats.SPIRV;
                    break;
                case Platform.Unknown:
                default:
                    throw new NotImplementedException($"SDL platform '{platform}'.");
            }
        }

        if ((shaderFormats & GpuShaderFormats.Private) != 0)
        {
            SDL_SetBooleanProperty(properties, SDL_PROP_GPU_DEVICE_CREATE_SHADERS_PRIVATE_BOOLEAN, true);
        }

        if ((shaderFormats & GpuShaderFormats.SPIRV) != 0)
        {
            SDL_SetBooleanProperty(properties, SDL_PROP_GPU_DEVICE_CREATE_SHADERS_SPIRV_BOOLEAN, true);
        }

        if ((shaderFormats & GpuShaderFormats.DXBC) != 0)
        {
            SDL_SetBooleanProperty(properties, SDL_PROP_GPU_DEVICE_CREATE_SHADERS_DXBC_BOOLEAN, true);
        }

        if ((shaderFormats & GpuShaderFormats.DXIL) != 0)
        {
            SDL_SetBooleanProperty(properties, SDL_PROP_GPU_DEVICE_CREATE_SHADERS_DXIL_BOOLEAN, true);
        }

        if ((shaderFormats & GpuShaderFormats.MSL) != 0)
        {
            SDL_SetBooleanProperty(properties, SDL_PROP_GPU_DEVICE_CREATE_SHADERS_MSL_BOOLEAN, true);
        }

        if ((shaderFormats & GpuShaderFormats.MetalLib) != 0)
        {
            SDL_SetBooleanProperty(properties, SDL_PROP_GPU_DEVICE_CREATE_SHADERS_METALLIB_BOOLEAN, true);
        }

        HandleTyped = SDL_CreateGPUDeviceWithProperties(properties);
        Handle = (IntPtr)HandleTyped;
        SDL_DestroyProperties(properties);
        Allocator.Reset();

        if (Handle == IntPtr.Zero)
        {
            Error.NativeFunctionFailed(nameof(SDL_CreateGPUDeviceWithProperties), isExceptionThrown: true);
        }

        var driverNameCActual = SDL_GetGPUDeviceDriver(HandleTyped);
        if (driverNameCActual.IsNull)
        {
            Error.NativeFunctionFailed(nameof(SDL_GetGPUDeviceDriver), true);
        }

        var driverNameActual = CString.ToString(driverNameCActual);
        Driver = driverNameActual switch
        {
            "vulkan" => GpuDriver.Vulkan,
            "direct3d12" => GpuDriver.DirectX12,
            "metal" => GpuDriver.Metal,
            _ => throw new NotImplementedException($"GPU driver '{driverNameActual}'.")
        };

        _poolCommandBuffer = new Pool<GpuCommandBuffer>(_logger, () => new GpuCommandBuffer(this), "GpuCommandBuffers");
        PoolRenderPass = new Pool<GpuRenderPass>(_logger, () => new GpuRenderPass(this), "GpuRenderPasses");

        SupportedShaderFormats = (GpuShaderFormats)(uint)SDL_GetGPUShaderFormats(HandleTyped);

        if (SDL_GPUTextureSupportsFormat(
                HandleTyped,
                SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D24_UNORM_S8_UINT,
                SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_2D,
                SDL_GPU_TEXTUREUSAGE_DEPTH_STENCIL_TARGET))
        {
            SupportedDepthStencilTargetFormat = (GpuTextureFormat)SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D24_UNORM_S8_UINT;
        }
        else if (SDL_GPUTextureSupportsFormat(
                     HandleTyped,
                     SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D32_FLOAT_S8_UINT,
                     SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_2D,
                     SDL_GPU_TEXTUREUSAGE_DEPTH_STENCIL_TARGET))
        {
            SupportedDepthStencilTargetFormat = (GpuTextureFormat)SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D32_FLOAT_S8_UINT;
        }
        else
        {
            SupportedDepthStencilTargetFormat = GpuTextureFormat.Invalid;
        }
    }

    /// <summary>
    ///     Attempts to claim the specified <see cref="Window" /> instance and associate it with the device. If
    ///     successful, creates a <see cref="GpuSwapchain" /> instance associated with the device and the window.
    /// </summary>
    /// <param name="window">The <see cref="Window" /> instance to claim.</param>
    /// <returns><c>true</c> if the window was successfully claimed; otherwise, <c>false</c>.</returns>
    public bool TryClaimWindow(Window window)
    {
        if (window.IsClaimed)
        {
            return false;
        }

        var isSuccess = SDL_ClaimWindowForGPUDevice(HandleTyped, (SDL_Window*)window.Handle);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_ClaimWindowForGPUDevice));
            return false;
        }

        var swapchain = new GpuSwapchain(this, window);
        window.Swapchain = swapchain;
        return true;
    }

    /// <summary>
    ///     Releases the specified <see cref="Window" /> instance from the device, destroying the
    ///     <see cref="GpuSwapchain" /> instance associated with the device and window.
    /// </summary>
    /// <param name="window">The <see cref="Window" /> instance.</param>
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
    ///     Acquires a <see cref="GpuCommandBuffer" /> pooled instance associated with the device.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         In multi-threading scenarios, calling <see cref="GetCommandBuffer" /> is safe. However, the returned
    ///         <see cref="GpuCommandBuffer" /> is not thread-safe; it must only be used and submitted on
    ///         the thread it was acquired on.
    ///     </para>
    ///     <para>
    ///         It is valid to acquire multiple <see cref="GpuCommandBuffer" /> instances on the same thread at once.
    ///         It's even a common design pattern to acquire two command buffers per frame: one for render and compute
    ///         passes and the other for copy passes and other preparatory work such as generating mipmaps. Interleaving
    ///         commands between the two command buffers reduces the total amount of passes overall which improves
    ///         rendering performance.
    ///     </para>
    /// </remarks>
    /// <returns>A pooled <see cref="GpuCommandBuffer" /> instance.</returns>
    public GpuCommandBuffer GetCommandBuffer()
    {
        var handle = SDL_AcquireGPUCommandBuffer(HandleTyped);
        if (handle == null)
        {
            Error.NativeFunctionFailed(nameof(SDL_AcquireGPUCommandBuffer), isExceptionThrown: true);
        }

        var commandBuffer = _poolCommandBuffer.GetOrCreate()!;
        commandBuffer.Set(handle);
        return commandBuffer;
    }

    /// <summary>
    ///     Attempts to create a new <see cref="GpuGraphicsShader" /> instance.
    /// </summary>
    /// <param name="options">The parameters for creating the shader.</param>
    /// <param name="shader">If successful, a new <see cref="GpuGraphicsShader" /> instance; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the shader was successfully created; otherwise, <c>false</c>.</returns>
    public bool TryCreateShader(GpuGraphicsShaderOptions options, out GpuGraphicsShader? shader)
    {
        SDL_GPUShaderCreateInfo info = default;
        info.code = (byte*)options.DataPointer;
        info.code_size = (ulong)options.DataSize;
        info.entrypoint = options.Allocator.AllocateCString(options.EntryPoint);
        var shaderFormat = (SDL_GPUShaderFormat)(uint)options.Format;
        info.format = shaderFormat;
        info.stage = (SDL_GPUShaderStage)options.Stage;
        info.num_samplers = (uint)Math.Max(options.SamplerCount, 0);
        info.num_uniform_buffers = (uint)Math.Max(options.UniformBufferCount, 0);
        info.num_storage_buffers = (uint)Math.Max(options.StorageBufferCount, 0);
        info.num_storage_textures = (uint)Math.Max(options.StorageTextureCount, 0);

        var handle = SDL_CreateGPUShader(HandleTyped, &info);
        if (handle == null)
        {
            Error.NativeFunctionFailed(nameof(SDL_CreateGPUShader));
            shader = null;
            return false;
        }

        shader = new GpuGraphicsShader(
            this, (IntPtr)handle, options.Format, options.Stage);
        return true;
    }

    /// <summary>
    ///     Attempts to create a new <see cref="GpuGraphicsShader" /> instance using the specified file path to the
    ///     shader file.
    /// </summary>
    /// <param name="fileSystem">The <see cref="FileSystem" /> instance.</param>
    /// <param name="filePath">
    ///     The file path to the shader file. If the path is relative, it is assumed to be relative to
    ///     <see cref="AppContext.BaseDirectory" />.
    /// </param>
    /// <param name="shader">If successful, a new <see cref="GpuGraphicsShader" /> instance; otherwise, <c>null</c>.</param>
    /// <param name="samplerCount">The number of samplers used in the shader.</param>
    /// <param name="uniformBufferCount">The number of uniform buffers used in the shader.</param>
    /// <returns><c>true</c> if the shader was successfully created; otherwise, <c>false</c>.</returns>
    public bool TryCreateShaderFromFile(
        FileSystem fileSystem,
        string filePath,
        out GpuGraphicsShader? shader,
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

        using var descriptor = new GpuGraphicsShaderOptions();
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
    ///     Attempts to create a new <see cref="GpuGraphicsPipeline" /> instance.
    /// </summary>
    /// <param name="options">The parameters for creating the pipeline.</param>
    /// <param name="pipeline">
    ///     If successful, a new <see cref="GpuGraphicsPipeline" /> instance; otherwise, <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if the pipeline was successfully created; otherwise, <c>false</c>.</returns>
    /// <exception cref="InvalidOperationException">The vertex shader or fragment shader is <c>null</c>.</exception>
    public bool TryCreatePipeline(GpuGraphicsPipelineOptions options, out GpuGraphicsPipeline? pipeline)
    {
        var info = default(SDL_GPUGraphicsPipelineCreateInfo);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (options.VertexShader == null)
        {
            throw new InvalidOperationException("Vertex shader can not be null.");
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (options.FragmentShader == null)
        {
            throw new InvalidOperationException("Fragment shader can not be null.");
        }

        info.vertex_shader = (SDL_GPUShader*)options.VertexShader.Handle;
        info.fragment_shader = (SDL_GPUShader*)options.FragmentShader.Handle;

        ref var vertexInputState = ref info.vertex_input_state;
        vertexInputState.vertex_buffer_descriptions =
            (SDL_GPUVertexBufferDescription*)options.VertexInputState.VertexBufferDescriptionsArrayPointer;
        vertexInputState.num_vertex_buffers = (uint)options.VertexInputState.VertexBufferDescriptionCount;
        vertexInputState.vertex_attributes = (SDL_GPUVertexAttribute*)options.VertexInputState.VertexAttributesArrayPointer;
        vertexInputState.num_vertex_attributes = (uint)options.VertexInputState.VertexAttributeCount;

        info.primitive_type = (SDL_GPUPrimitiveType)options.PrimitiveType;

        ref var rasterizerState = ref info.rasterizer_state;
        {
            rasterizerState.fill_mode = (SDL_GPUFillMode)options.RasterizerState.FillMode;
            rasterizerState.cull_mode = (SDL_GPUCullMode)options.RasterizerState.CullMode;
            rasterizerState.front_face = (SDL_GPUFrontFace)options.RasterizerState.FrontFace;
            rasterizerState.depth_bias_constant_factor = options.RasterizerState.DepthBiasConstantFactor;
            rasterizerState.depth_bias_clamp = options.RasterizerState.DepthBiasClamp;
            rasterizerState.depth_bias_slope_factor = options.RasterizerState.DepthBiasSlopeFactor;
            rasterizerState.enable_depth_bias = options.RasterizerState.IsEnabledDepthBias;
            rasterizerState.enable_depth_clip = options.RasterizerState.IsEnabledDepthClamp;
        }

        // TODO: multi-sample state

        ref var depthStencil = ref info.depth_stencil_state;
        {
            ref var stencilBack = ref depthStencil.back_stencil_state;
            stencilBack.fail_op = (SDL_GPUStencilOp)options.DepthStencilState.BackStencilState.FailOp;
            stencilBack.pass_op = (SDL_GPUStencilOp)options.DepthStencilState.BackStencilState.PassOp;
            stencilBack.depth_fail_op = (SDL_GPUStencilOp)options.DepthStencilState.BackStencilState.DepthFailOp;
            stencilBack.compare_op = (SDL_GPUCompareOp)options.DepthStencilState.BackStencilState.CompareOp;

            ref var stencilFront = ref depthStencil.front_stencil_state;
            stencilFront.fail_op = (SDL_GPUStencilOp)options.DepthStencilState.FrontStencilState.FailOp;
            stencilFront.pass_op = (SDL_GPUStencilOp)options.DepthStencilState.FrontStencilState.PassOp;
            stencilFront.depth_fail_op = (SDL_GPUStencilOp)options.DepthStencilState.FrontStencilState.DepthFailOp;
            stencilFront.compare_op = (SDL_GPUCompareOp)options.DepthStencilState.FrontStencilState.CompareOp;

            depthStencil.compare_op = (SDL_GPUCompareOp)options.DepthStencilState.CompareOp;
            depthStencil.compare_mask = options.DepthStencilState.ReadMask;
            depthStencil.write_mask = options.DepthStencilState.WriteMask;
            depthStencil.enable_depth_test = options.DepthStencilState.IsEnabledDepthTest;
            depthStencil.enable_depth_write = options.DepthStencilState.IsEnabledDepthWrite;
            depthStencil.enable_stencil_test = options.DepthStencilState.IsEnabledStencilTest;
        }

        ref var targetInfo = ref info.target_info;
        {
            targetInfo.num_color_targets = (uint)options.ColorRenderTargets.Length;
            var colorTargetDescriptions =
                stackalloc SDL_GPUColorTargetDescription[options.ColorRenderTargets.Length];
            for (var i = 0; i < options.ColorRenderTargets.Length; i++)
            {
                ref var colorTargetTo = ref colorTargetDescriptions[i];
                var colorTargetFrom = options.ColorRenderTargets[i];
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
            targetInfo.num_color_targets = (uint)options.ColorRenderTargets.Length;
            targetInfo.depth_stencil_format = (SDL_GPUTextureFormat)options.DepthStencilRenderTargetFormat;
            targetInfo.has_depth_stencil_target = options.IsEnabledDepthStencilRenderTarget;
        }

        var handle = SDL_CreateGPUGraphicsPipeline(HandleTyped, &info);
        if (handle == null)
        {
            Error.NativeFunctionFailed(nameof(SDL_CreateGPUShader));
            pipeline = null;
            return false;
        }

        pipeline = new GpuGraphicsPipeline(this, (IntPtr)handle);
        return true;
    }

    /// <summary>
    ///     Attempts to create a new <see cref="GpuDataBuffer" /> instance.
    /// </summary>
    /// <param name="elementCount">
    ///     The maximum number of <typeparamref name="TElement" /> elements the buffer can hold.
    /// </param>
    /// <param name="buffer">
    ///     If successful, a new <see cref="GpuDataBuffer" /> instance; otherwise, <c>null</c>.
    /// </param>
    /// <param name="name">The optional name of the data buffer.</param>
    /// <typeparam name="TElement">The type of data buffer element.</typeparam>
    /// <returns><c>true</c> if the data buffer was successfully created; otherwise, <c>false</c>.</returns>
    public bool TryCreateDataBuffer<TElement>(int elementCount, out GpuDataBuffer? buffer, string? name = null)
        where TElement : unmanaged
    {
        var bufferCreateInfo = default(SDL_GPUBufferCreateInfo);
        bufferCreateInfo.usage = SDL_GPU_BUFFERUSAGE_VERTEX;
        bufferCreateInfo.size = (uint)(sizeof(TElement) * elementCount);
        var handle = SDL_CreateGPUBuffer(HandleTyped, &bufferCreateInfo);
        if (handle == null)
        {
            Error.NativeFunctionFailed(nameof(SDL_CreateGPUBuffer));
            buffer = null;
            return false;
        }

        buffer = new GpuDataBuffer(this, (IntPtr)handle);

        if (name != null)
        {
            var allocator = Allocator;
            allocator.Reset();
            var nameCString = allocator.AllocateCString(name);
            SDL_SetGPUBufferName(HandleTyped, handle, nameCString);
            allocator.Reset();
        }

        return true;
    }

    /// <summary>
    ///     Attempts to create a new transfer <see cref="GpuTransferBuffer" /> instance.
    /// </summary>
    /// <param name="size">
    ///     The size of the transfer buffer in bytes.
    /// </param>
    /// <param name="transferBuffer">
    ///     If successful, a new <see cref="GpuTransferBuffer" /> instance; otherwise, <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if the transfer buffer was successfully created; otherwise, <c>false</c>.</returns>
    public bool TryCreateTransferBuffer(int size, out GpuTransferBuffer? transferBuffer)
    {
        var transferBufferCreateInfo = default(SDL_GPUTransferBufferCreateInfo);
        transferBufferCreateInfo.usage = SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD;
        transferBufferCreateInfo.size = (uint)size;
        var handle = SDL_CreateGPUTransferBuffer(
            HandleTyped, &transferBufferCreateInfo);
        if (handle == null)
        {
            Error.NativeFunctionFailed(nameof(SDL_CreateGPUTransferBuffer));
            transferBuffer = null;
            return false;
        }

        transferBuffer = new GpuTransferBuffer(
            this, (IntPtr)handle, (int)transferBufferCreateInfo.size);

        return true;
    }

    /// <summary>
    ///     Attempts to create a new <see cref="GpuTexture" /> instance.
    /// </summary>
    /// <param name="options">The parameters for creating a texture.</param>
    /// <param name="texture">
    ///     If successful, a new <see cref="GpuTexture" /> instance; otherwise, <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if the texture was successfully created; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentException">Sample count must be 1, 2, 4, 8.</exception>
    public bool TryCreateTexture(GpuTextureOptions options, out GpuTexture? texture)
    {
        var textureCreateInfo = default(SDL_GPUTextureCreateInfo);
        textureCreateInfo.type = (SDL_GPUTextureType)options.Type;
        textureCreateInfo.width = (uint)options.Width;
        textureCreateInfo.height = (uint)options.Height;
        textureCreateInfo.layer_count_or_depth = (uint)options.LayerCountOrDepth;
        textureCreateInfo.num_levels = (uint)options.MipmapLevelCount;
        textureCreateInfo.format = (SDL_GPUTextureFormat)options.Format;
        textureCreateInfo.usage = (uint)options.Usage;

        textureCreateInfo.sample_count = options.SampleCount switch
        {
            0 => SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1,
            1 => SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1,
            2 => SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_2,
            4 => SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_4,
            8 => SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_8,
            _ => throw new ArgumentException("Sample count must be 1, 2, 4, or 8.")
        };

        var handle = SDL_CreateGPUTexture(HandleTyped, &textureCreateInfo);
        if (handle == null)
        {
            Error.NativeFunctionFailed(nameof(SDL_CreateGPUTexture));
            texture = null;
            return false;
        }

        texture = new GpuTexture(
            this,
            (IntPtr)handle,
            options.Type,
            options.Format,
            options.Width,
            options.Height,
            options.LayerCountOrDepth,
            options.MipmapLevelCount,
            options.SampleCount,
            options.Usage);

        var nameCString = options.Allocator.AllocateCString(options.Name);
        SDL_SetGPUTextureName(HandleTyped, handle, nameCString);

        return true;
    }

    /// <summary>
    ///     Attempts to create a new <see cref="GpuSampler" /> instance.
    /// </summary>
    /// <param name="options">The parameters for creating a sampler.</param>
    /// <param name="sampler">
    ///     If successful, a new <see cref="GpuSampler" /> instance; otherwise, <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if the sampler was successfully created; otherwise, <c>false</c>.</returns>
    public bool TryCreateSampler(GpuSamplerOptions options, out GpuSampler? sampler)
    {
        var samplerCreateInfo = default(SDL_GPUSamplerCreateInfo);
        samplerCreateInfo.min_filter = (SDL_GPUFilter)options.MinificationFilterMode;
        samplerCreateInfo.mag_filter = (SDL_GPUFilter)options.MagnificationFilterMode;
        samplerCreateInfo.mipmap_mode = (SDL_GPUSamplerMipmapMode)options.MipMapMode;
        samplerCreateInfo.address_mode_u = (SDL_GPUSamplerAddressMode)options.AddressModeU;
        samplerCreateInfo.address_mode_v = (SDL_GPUSamplerAddressMode)options.AddressModeV;
        samplerCreateInfo.address_mode_w = (SDL_GPUSamplerAddressMode)options.AddressModeW;
        samplerCreateInfo.mip_lod_bias = 0.0f; // FIXME: This is only used in DirectX12/Vulkan and not cross-platform?
        samplerCreateInfo.max_anisotropy = options.MaximumAnisotropy;
        samplerCreateInfo.compare_op = (SDL_GPUCompareOp)options.DepthCompareOp;
        samplerCreateInfo.min_lod = options.MinimumLevelOfDetailClamp;
        samplerCreateInfo.max_lod = options.MaximumLevelOfDetailClamp;
        samplerCreateInfo.enable_anisotropy = options.IsEnabledAnisotropy;
        samplerCreateInfo.enable_compare = options.IsEnabledDepthCompare;

        var handle = SDL_CreateGPUSampler(HandleTyped, &samplerCreateInfo);
        if (handle == null)
        {
            Error.NativeFunctionFailed(nameof(SDL_CreateGPUSampler));
            sampler = null;
            return false;
        }

        sampler = new GpuSampler(this, (IntPtr)handle);

        return true;
    }

    internal void EndRenderPassTryInternal(GpuRenderPass renderPass)
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

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        PoolRenderPass.Dispose();
        _poolCommandBuffer.Dispose();

        SDL_DestroyGPUDevice(HandleTyped);

        base.Dispose(isDisposing);

        Allocator.Dispose();
    }
}
