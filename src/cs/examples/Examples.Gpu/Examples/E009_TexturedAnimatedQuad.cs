// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using SDL.GPU;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E009_TexturedAnimatedQuad : ExampleGpu
{
    private GraphicsPipeline? _pipeline;
    private DataBuffer? _vertexBuffer;
    private DataBuffer? _indexBuffer;
    private Texture? _texture;
    private Sampler? _sampler;

    private float _t;

    public override bool Initialize(INativeAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        if (!Device.TryCreateShaderFromFile(
                FileSystem,
                GetShaderFilePath("TexturedQuadWithMatrix.vert"),
                out var vertexShader,
                uniformBufferCount: 1))
        {
            return false;
        }

        if (!Device.TryCreateShaderFromFile(
                FileSystem,
                GetShaderFilePath("TexturedQuadWithMultiplyColor.frag"),
                out var fragmentShader,
                samplerCount: 1,
                uniformBufferCount: 1))
        {
            return false;
        }

        var imageFilePath = Path.Combine(AssetsDirectory, "Images", "ravioli.bmp");
        if (!FileSystem.TryLoadImage(
                imageFilePath, out var surface, PixelFormat.Abgr8888))
        {
            return false;
        }

        using var pipelineDescriptor = new GraphicsPipelineDescriptor();
        pipelineDescriptor.PrimitiveType = GraphicsPipelineVertexPrimitiveType.TriangleList;
        pipelineDescriptor.VertexShader = vertexShader;
        pipelineDescriptor.FragmentShader = fragmentShader;
        pipelineDescriptor.SetVertexAttributes<VertexPositionTexture>();
        pipelineDescriptor.SetVertexBufferDescription<VertexPositionTexture>();
        pipelineDescriptor.SetRenderTargetColor(Window.Swapchain!);

        var blendState = pipelineDescriptor.ColorRenderTargets[0].BlendState;
        blendState.IsEnabledBlend = true;
        blendState.AlphaBlendOp = SDL_GPUBlendOp.SDL_GPU_BLENDOP_ADD;
        blendState.ColorBlendOp = SDL_GPUBlendOp.SDL_GPU_BLENDOP_ADD;
        blendState.SourceColorBlendFactor = BlendFactor.SourceAlpha;
        blendState.SourceAlphaBlendFactor = BlendFactor.SourceAlpha;
        blendState.DestinationColorBlendFactor = BlendFactor.OneMinusSourceAlpha;
        blendState.DestinationAlphaBlendFactor = BlendFactor.OneMinusSourceAlpha;
        if (!Device.TryCreatePipeline(pipelineDescriptor, out _pipeline))
        {
            return false;
        }

        vertexShader?.Dispose();
        fragmentShader?.Dispose();

        using var textureDescriptor = new TextureDescriptor();
        textureDescriptor.Type = TextureType.TwoDimensional;
        textureDescriptor.Format = TextureFormat.R8G8B8A8_UNORM;
        textureDescriptor.Width = surface!.Width;
        textureDescriptor.Height = surface.Height;
        textureDescriptor.LayerCountOrDepth = 1;
        textureDescriptor.MipmapLevelCount = 1;
        textureDescriptor.Usage = TextureUsages.Sampler;
        if (!Device.TryCreateTexture(textureDescriptor, out _texture))
        {
            return false;
        }

        // PointClamp
        var samplerDescriptor = new SamplerDescriptor();
        samplerDescriptor.MinificationFilter = SamplerFilter.Nearest;
        samplerDescriptor.MagnificationFilter = SamplerFilter.Nearest;
        samplerDescriptor.MipMapMode = SamplerMipmapMode.Nearest;
        samplerDescriptor.AddressModeU = SamplerAddressMode.ClampToEdge;
        samplerDescriptor.AddressModeV = SamplerAddressMode.ClampToEdge;
        samplerDescriptor.AddressModeW = SamplerAddressMode.ClampToEdge;
        if (!Device.TryCreateSampler(samplerDescriptor, out _sampler))
        {
            return false;
        }

        if (!Device.TryCreateDataBuffer<VertexPositionTexture>(4, out _vertexBuffer))
        {
            return false;
        }

        if (!Device.TryCreateDataBuffer<ushort>(6, out _indexBuffer))
        {
            return false;
        }

        if (!Device.TryCreateTransferBuffer(
                (sizeof(VertexPositionTexture) * 4) + (sizeof(ushort) * 6),
                out var transferBufferVertexIndex))
        {
            return false;
        }

        var vertexIndexSpan = transferBufferVertexIndex!.MapAsSpan();
        var vertexData = MemoryMarshal.Cast<byte, VertexPositionTexture>(
            vertexIndexSpan[..(sizeof(VertexPositionTexture) * 4)]);

        vertexData[0].Position = new Vector3(-0.5f, -0.5f, 0);
        vertexData[0].TextureCoordinates = new Vector2(0, 0);

        vertexData[1].Position = new Vector3(0.5f, -0.5f, 0);
        vertexData[1].TextureCoordinates = new Vector2(1, 0);

        vertexData[2].Position = new Vector3(0.5f, 0.5f, 0);
        vertexData[2].TextureCoordinates = new Vector2(1, 1);

        vertexData[3].Position = new Vector3(-0.5f, 0.5f, 0);
        vertexData[3].TextureCoordinates = new Vector2(0, 1);

        var indexData = MemoryMarshal.Cast<byte, ushort>(
            vertexIndexSpan[(sizeof(VertexPositionTexture) * 4)..]);
        indexData[0] = 0;
        indexData[1] = 1;
        indexData[2] = 2;
        indexData[3] = 0;
        indexData[4] = 2;
        indexData[5] = 3;

        transferBufferVertexIndex.Unmap();

        // Set up texture data
        if (!Device.TryCreateTransferBuffer(surface.Width * surface.Height * 4, out var transferBufferTexture))
        {
            return false;
        }

        var dataTexturePointer = transferBufferTexture!.MapAsPointer();
        NativeMemory.Copy((void*)surface.DataPointer, (void*)dataTexturePointer, (UIntPtr)transferBufferTexture.Size);
        transferBufferTexture.Unmap();

        var uploadCommandBuffer = Device.GetCommandBuffer();
        var copyPass = uploadCommandBuffer.BeginCopyPass();

        copyPass.UploadToDataBuffer(
            transferBufferVertexIndex,
            0,
            _vertexBuffer,
            0,
            sizeof(VertexPositionTexture) * 4);
        copyPass.UploadToDataBuffer(
            transferBufferVertexIndex,
            sizeof(VertexPositionTexture) * 4,
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

    public override void Quit()
    {
        _pipeline?.Dispose();
        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();
        _texture?.Dispose();
        _sampler?.Dispose();
        _t = 0;

        base.Quit();
    }

    public override void KeyboardEvent(in SDL_KeyboardEvent e)
    {
    }

    public override void Update(float deltaTime)
    {
        _t += deltaTime;
    }

    public override void Draw(float deltaTime)
    {
        var commandBuffer = Device.GetCommandBuffer();
        if (!commandBuffer.TryGetSwapchainTexture(Window, out var swapchainTexture))
        {
            commandBuffer.Cancel();
            return;
        }

        var renderTargetInfoColor = default(RenderTargetInfoColor);
        renderTargetInfoColor.Texture = swapchainTexture;
        renderTargetInfoColor.ClearColor = Rgba32F.CornflowerBlue;
        renderTargetInfoColor.LoadOp = RenderTargetLoadOp.Clear;
        renderTargetInfoColor.StoreOp = RenderTargetStoreOp.Store;
        var renderPass = commandBuffer.BeginRenderPass(null, renderTargetInfoColor);

        renderPass.BindPipeline(_pipeline);
        renderPass.BindVertexBuffer(_vertexBuffer);
        renderPass.BindIndexBuffer(_indexBuffer);
        renderPass.BindFragmentSampler(_texture, _sampler);

        Matrix4x4 transformMatrix;
        Rgba32F color;

        // Bottom-left
        transformMatrix =
            Matrix4x4.CreateRotationZ(_t) *
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
