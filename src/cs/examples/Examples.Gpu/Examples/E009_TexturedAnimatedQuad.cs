// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E009_TexturedAnimatedQuad : ExampleGpu
{
    private GpuGraphicsPipeline? _pipeline;
    private GpuDataBuffer? _vertexBuffer;
    private GpuDataBuffer? _indexBuffer;
    private GpuTexture? _texture;
    private GpuSampler? _sampler;

    private float _t;

    public override bool OnStart()
    {
        if (!base.OnStart())
        {
            return false;
        }

        var vertexShaderOptions = new GpuGraphicsShaderOptions();
        vertexShaderOptions.UniformBufferCount = 1;
        if (!FileSystem.TryLoadGraphicsShader(
                GetShaderFilePath("TexturedQuadWithMatrix.vert"), Device, vertexShaderOptions, out var vertexShader))
        {
            return false;
        }

        var fragmentShaderOptions = new GpuGraphicsShaderOptions();
        fragmentShaderOptions.SamplerCount = 1;
        fragmentShaderOptions.UniformBufferCount = 1;
        if (!FileSystem.TryLoadGraphicsShader(
                GetShaderFilePath("TexturedQuadWithMultiplyColor.frag"), Device, fragmentShaderOptions, out var fragmentShader))
        {
            return false;
        }

        var imageFilePath = Path.Combine(AssetsDirectory, "Images", "ravioli.bmp");
        if (!FileSystem.TryLoadImage(
                imageFilePath, out var surface, PixelFormat.Abgr8888))
        {
            return false;
        }

        using var graphicsPipelineOptions = new GpuGraphicsPipelineOptions();
        graphicsPipelineOptions.PrimitiveType = GpuGraphicsPipelineVertexPrimitiveType.TriangleList;
        graphicsPipelineOptions.VertexShader = vertexShader;
        graphicsPipelineOptions.FragmentShader = fragmentShader;
        graphicsPipelineOptions.SetVertexAttributes<VertexPositionTexture>();
        graphicsPipelineOptions.SetVertexBufferDescription<VertexPositionTexture>();
        graphicsPipelineOptions.SetRenderTargetColor(Window.Swapchain!);

        var blendState = graphicsPipelineOptions.ColorRenderTargets[0].BlendState;
        blendState.IsEnabledBlend = true;
        blendState.AlphaBlendOp = SDL_GPUBlendOp.SDL_GPU_BLENDOP_ADD;
        blendState.ColorBlendOp = SDL_GPUBlendOp.SDL_GPU_BLENDOP_ADD;
        blendState.SourceColorBlendFactor = GpuBlendFactor.SourceAlpha;
        blendState.SourceAlphaBlendFactor = GpuBlendFactor.SourceAlpha;
        blendState.DestinationColorBlendFactor = GpuBlendFactor.OneMinusSourceAlpha;
        blendState.DestinationAlphaBlendFactor = GpuBlendFactor.OneMinusSourceAlpha;
        if (!Device.TryCreateGraphicsPipeline(graphicsPipelineOptions, out _pipeline))
        {
            return false;
        }

        vertexShader.Dispose();
        fragmentShader.Dispose();

        using var textureOptions = new GpuTextureOptions();
        textureOptions.Type = GpuTextureType.TwoDimensional;
        textureOptions.Format = GpuTextureFormat.R8G8B8A8_UNORM;
        textureOptions.Width = surface.Width;
        textureOptions.Height = surface.Height;
        textureOptions.LayersCountOrDepth = 1;
        textureOptions.MipmapLevelsCount = 1;
        textureOptions.Usage = GpuTextureUsages.Sampler;
        if (!Device.TryCreateTexture(textureOptions, out _texture))
        {
            return false;
        }

        // PointClamp
        var samplerOptions = new GpuSamplerOptions();
        samplerOptions.MinificationFilterMode = GpuSamplerFilterMode.Nearest;
        samplerOptions.MagnificationFilterMode = GpuSamplerFilterMode.Nearest;
        samplerOptions.MipMapMode = GpuSamplerMipmapMode.Nearest;
        samplerOptions.AddressModeU = GpuSamplerAddressMode.ClampToEdge;
        samplerOptions.AddressModeV = GpuSamplerAddressMode.ClampToEdge;
        samplerOptions.AddressModeW = GpuSamplerAddressMode.ClampToEdge;
        if (!Device.TryCreateSampler(samplerOptions, out _sampler))
        {
            return false;
        }

        if (!Device.TryCreateDataBuffer<VertexPositionTexture>(GpuBufferUsageFlags.Vertex, 4, out _vertexBuffer))
        {
            return false;
        }

        if (!Device.TryCreateDataBuffer<ushort>(GpuBufferUsageFlags.Index, 6, out _indexBuffer))
        {
            return false;
        }

        if (!Device.TryCreateUploadTransferBuffer(
                (VertexPositionTexture.SizeOf * 4) + (sizeof(ushort) * 6),
                out var transferBufferVertexIndex))
        {
            return false;
        }

        var vertexIndexSpan = transferBufferVertexIndex.MapAsSpan();
        var vertexData = MemoryMarshal.Cast<byte, VertexPositionTexture>(
            vertexIndexSpan[..(VertexPositionTexture.SizeOf * 4)]);

        vertexData[0].Position = new Vector3(-0.5f, -0.5f, 0);
        vertexData[0].TextureCoordinates = new Vector2(0, 0);

        vertexData[1].Position = new Vector3(0.5f, -0.5f, 0);
        vertexData[1].TextureCoordinates = new Vector2(1, 0);

        vertexData[2].Position = new Vector3(0.5f, 0.5f, 0);
        vertexData[2].TextureCoordinates = new Vector2(1, 1);

        vertexData[3].Position = new Vector3(-0.5f, 0.5f, 0);
        vertexData[3].TextureCoordinates = new Vector2(0, 1);

        var indexData = MemoryMarshal.Cast<byte, ushort>(
            vertexIndexSpan[(VertexPositionTexture.SizeOf * 4)..]);
        indexData[0] = 0;
        indexData[1] = 1;
        indexData[2] = 2;
        indexData[3] = 0;
        indexData[4] = 2;
        indexData[5] = 3;

        transferBufferVertexIndex.Unmap();

        // Set up texture data
        if (!Device.TryCreateUploadTransferBuffer(surface.Width * surface.Height * 4, out var transferBufferTexture))
        {
            return false;
        }

        var dataTexturePointer = transferBufferTexture.MapAsPointer();

        unsafe
        {
            NativeMemory.Copy((void*)surface.DataPointer, (void*)dataTexturePointer, (UIntPtr)transferBufferTexture.Size);
        }

        transferBufferTexture.Unmap();

        var uploadCommandBuffer = Device.GetCommandBuffer();
        var copyPass = uploadCommandBuffer.BeginCopyPass();

        copyPass.UploadToDataBuffer(
            transferBufferVertexIndex,
            0,
            _vertexBuffer,
            0,
            VertexPositionTexture.SizeOf * 4);
        copyPass.UploadToDataBuffer(
            transferBufferVertexIndex,
            VertexPositionTexture.SizeOf * 4,
            _indexBuffer,
            0,
            sizeof(ushort) * 6);
        copyPass.UploadToTexture(
            transferBufferTexture, 0, _texture, surface.Width, surface.Height);

        copyPass.End();

        surface.Dispose();
        copyPass.Dispose();
        uploadCommandBuffer.Submit();
        transferBufferVertexIndex.Dispose();
        transferBufferTexture.Dispose();

        return true;
    }

    public override void OnExit()
    {
        _pipeline?.Dispose();
        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();
        _texture?.Dispose();
        _sampler?.Dispose();
        _t = 0;

        base.OnExit();
    }

    public override void OnUpdate(TimeSpan deltaTime)
    {
        _t += (float)deltaTime.TotalSeconds;
    }

    public override void OnDraw(TimeSpan deltaTime)
    {
        var commandBuffer = Device.GetCommandBuffer();
        if (!commandBuffer.TryGetSwapchainTexture(Window, out var swapchainTexture))
        {
            commandBuffer.Cancel();
            return;
        }

        var renderTargetInfoColor = default(GpuRenderTargetInfoColor);
        renderTargetInfoColor.Texture = swapchainTexture;
        renderTargetInfoColor.ClearColor = Rgba32F.CornflowerBlue;
        renderTargetInfoColor.LoadOperation = GpuRenderTargetLoadOperation.Clear;
        renderTargetInfoColor.StoreOp = GpuRenderTargetStoreOp.Store;
        var renderPass = commandBuffer.BeginRenderPass(null, renderTargetInfoColor);

        renderPass.BindPipeline(_pipeline);
        renderPass.BindVertexBuffer(_vertexBuffer);
        renderPass.BindIndexBuffer(_indexBuffer);
        renderPass.BindFragmentSampler(_texture, _sampler);

        Rgba32F color;

        // Bottom-left
        var transformMatrix = Matrix4x4.CreateRotationZ(_t) *
                              Matrix4x4.CreateTranslation(-0.5f, -0.5f, 0);
        commandBuffer.PushVertexShaderUniformMatrix(transformMatrix);
        color.R = 1.0f;
        color.G = 0.5f + ((float)Math.Sin(_t) * 0.5f);
        color.B = 1.0f;
        color.A = 1.0f;
        commandBuffer.PushFragmentShaderUniformColor(color);
        renderPass.DrawPrimitivesIndexed(6, 1, 0, 0, 0);

        // Bottom-right
        transformMatrix =
            Matrix4x4.CreateRotationZ((2.0f * SDL_PI_F) - _t) *
            Matrix4x4.CreateTranslation(0.5f, -0.5f, 0);
        commandBuffer.PushVertexShaderUniformMatrix(transformMatrix);
        color.R = 1.0f;
        color.G = 0.5f + ((float)Math.Cos(_t) * 0.5f);
        color.B = 1.0f;
        color.A = 1.0f;
        commandBuffer.PushFragmentShaderUniformColor(color);
        renderPass.DrawPrimitivesIndexed(6, 1, 0, 0, 0);

        // Top-left
        transformMatrix =
            Matrix4x4.CreateRotationZ(_t) *
            Matrix4x4.CreateTranslation(-0.5f, 0.5f, 0);
        commandBuffer.PushVertexShaderUniformMatrix(transformMatrix);
        color.R = 1.0f;
        color.G = 0.5f + ((float)Math.Cos(_t) * 0.2f);
        color.B = 1.0f;
        color.A = 1.0f;
        commandBuffer.PushFragmentShaderUniformColor(color);
        renderPass.DrawPrimitivesIndexed(6, 1, 0, 0, 0);

        // Top-right
        transformMatrix =
            Matrix4x4.CreateRotationZ(_t) *
            Matrix4x4.CreateTranslation(0.5f, 0.5f, 0);
        commandBuffer.PushVertexShaderUniformMatrix(transformMatrix);
        color.R = 1.0f;
        color.G = 0.5f + ((float)Math.Cos(_t) * 1.0f);
        color.B = 1.0f;
        color.A = 1.0f;
        commandBuffer.PushFragmentShaderUniformColor(color);
        renderPass.DrawPrimitivesIndexed(6, 1, 0, 0, 0);

        renderPass.End();
        commandBuffer.Submit();
    }
}
