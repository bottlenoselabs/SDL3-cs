// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using SDL.GPU;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E008_TexturedQuad : ExampleGpu
{
    private static readonly string[] SamplerNames =
    [
        "PointClamp",
        "PointWrap",
        "LinearClamp",
        "LinearWrap",
        "AnisotropicClamp",
        "AnisotropicWrap"
    ];

    private GraphicsPipeline? _pipeline;
    private DataBuffer? _vertexBuffer;
    private DataBuffer? _indexBuffer;
    private Texture? _texture;
    private readonly Sampler?[] _samplers = new Sampler[SamplerNames.Length];

    private int _currentSamplerIndex;

    public override bool Initialize(INativeAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        if (!Device.TryCreateShaderFromFile(
                FileSystem, GetShaderFilePath("TexturedQuad.vert"), out var vertexShader))
        {
            return false;
        }

        if (!Device.TryCreateShaderFromFile(
                FileSystem,
                GetShaderFilePath("TexturedQuad.frag"),
                out var fragmentShader,
                samplerCount: 1))
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

        if (!Device.TryCreatePipeline(pipelineDescriptor, out _pipeline))
        {
            return false;
        }

        vertexShader?.Dispose();
        fragmentShader?.Dispose();

        var imageFilePath = Path.Combine(AssetsDirectory, "Images", "ravioli.bmp");
        if (!FileSystem.TryLoadImage(
                imageFilePath, out var surface, PixelFormat.Abgr8888))
        {
            return false;
        }

        var textureDescriptor = new TextureDescriptor();
        textureDescriptor.Name = "Ravioli Texture 🖼️";
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
        using var samplerDescriptor = new SamplerDescriptor();
        samplerDescriptor.MinificationFilter = SamplerFilter.Nearest;
        samplerDescriptor.MagnificationFilter = SamplerFilter.Nearest;
        samplerDescriptor.MipMapMode = SamplerMipmapMode.Nearest;
        samplerDescriptor.AddressModeU = SamplerAddressMode.ClampToEdge;
        samplerDescriptor.AddressModeV = SamplerAddressMode.ClampToEdge;
        samplerDescriptor.AddressModeW = SamplerAddressMode.ClampToEdge;
        if (!Device.TryCreateSampler(samplerDescriptor, out _samplers[0]))
        {
            return false;
        }

        // PointWrap
        samplerDescriptor.Reset();
        samplerDescriptor.MinificationFilter = SamplerFilter.Nearest;
        samplerDescriptor.MagnificationFilter = SamplerFilter.Nearest;
        samplerDescriptor.MipMapMode = SamplerMipmapMode.Nearest;
        samplerDescriptor.AddressModeU = SamplerAddressMode.Repeat;
        samplerDescriptor.AddressModeV = SamplerAddressMode.Repeat;
        samplerDescriptor.AddressModeW = SamplerAddressMode.Repeat;
        if (!Device.TryCreateSampler(samplerDescriptor, out _samplers[1]))
        {
            return false;
        }

        // LinearClamp
        samplerDescriptor.Reset();
        samplerDescriptor.MinificationFilter = SamplerFilter.Linear;
        samplerDescriptor.MagnificationFilter = SamplerFilter.Linear;
        samplerDescriptor.MipMapMode = SamplerMipmapMode.Linear;
        samplerDescriptor.AddressModeU = SamplerAddressMode.ClampToEdge;
        samplerDescriptor.AddressModeV = SamplerAddressMode.ClampToEdge;
        samplerDescriptor.AddressModeW = SamplerAddressMode.ClampToEdge;
        if (!Device.TryCreateSampler(samplerDescriptor, out _samplers[2]))
        {
            return false;
        }

        // LinearWrap
        samplerDescriptor.Reset();
        samplerDescriptor.MinificationFilter = SamplerFilter.Linear;
        samplerDescriptor.MagnificationFilter = SamplerFilter.Linear;
        samplerDescriptor.MipMapMode = SamplerMipmapMode.Linear;
        samplerDescriptor.AddressModeU = SamplerAddressMode.Repeat;
        samplerDescriptor.AddressModeV = SamplerAddressMode.Repeat;
        samplerDescriptor.AddressModeW = SamplerAddressMode.Repeat;
        if (!Device.TryCreateSampler(samplerDescriptor, out _samplers[3]))
        {
            return false;
        }

        // AnisotropicClamp
        samplerDescriptor.Reset();
        samplerDescriptor.MinificationFilter = SamplerFilter.Linear;
        samplerDescriptor.MagnificationFilter = SamplerFilter.Linear;
        samplerDescriptor.MipMapMode = SamplerMipmapMode.Linear;
        samplerDescriptor.AddressModeU = SamplerAddressMode.ClampToEdge;
        samplerDescriptor.AddressModeV = SamplerAddressMode.ClampToEdge;
        samplerDescriptor.AddressModeW = SamplerAddressMode.ClampToEdge;
        samplerDescriptor.IsEnabledAnisotropy = true;
        samplerDescriptor.MaximumAnisotropy = 4;
        if (!Device.TryCreateSampler(samplerDescriptor, out _samplers[4]))
        {
            return false;
        }

        // AnisotropicWrap
        samplerDescriptor.Reset();
        samplerDescriptor.MinificationFilter = SamplerFilter.Linear;
        samplerDescriptor.MagnificationFilter = SamplerFilter.Linear;
        samplerDescriptor.MipMapMode = SamplerMipmapMode.Linear;
        samplerDescriptor.AddressModeU = SamplerAddressMode.Repeat;
        samplerDescriptor.AddressModeV = SamplerAddressMode.Repeat;
        samplerDescriptor.AddressModeW = SamplerAddressMode.Repeat;
        samplerDescriptor.IsEnabledAnisotropy = true;
        samplerDescriptor.MaximumAnisotropy = 4;
        if (!Device.TryCreateSampler(samplerDescriptor, out _samplers[5]))
        {
            return false;
        }

        if (!Device.TryCreateDataBuffer<VertexPositionTexture>(
                4, out _vertexBuffer, "Ravioli Vertex Buffer 🥣"))
        {
            return false;
        }

        if (!Device.TryCreateDataBuffer<ushort>(6, out _indexBuffer))
        {
            return false;
        }

        if (!Device.TryCreateTransferBuffer(
                (sizeof(VertexPositionTexture) * 4) + (sizeof(ushort) * 6),
                out var transferBufferVerticesAndIndices))
        {
            return false;
        }

        var transferBufferVerticesAndIndicesSpan = transferBufferVerticesAndIndices!.MapAsSpan();
        var vertexData = MemoryMarshal.Cast<byte, VertexPositionTexture>(
            transferBufferVerticesAndIndicesSpan[..(sizeof(VertexPositionTexture) * 4)]);

        vertexData[0].Position = new Vector3(-1f, 1f, 0); // top-left
        vertexData[0].TextureCoordinates = new Vector2(0, 0);

        vertexData[1].Position = new Vector3(1f, 1f, 0); // top-right
        vertexData[1].TextureCoordinates = new Vector2(4, 0);

        vertexData[2].Position = new Vector3(1, -1f, 0); // bottom-right
        vertexData[2].TextureCoordinates = new Vector2(4, 4);

        vertexData[3].Position = new Vector3(-1, -1, 0); // bottom-left
        vertexData[3].TextureCoordinates = new Vector2(0, 4);

        var indexData = MemoryMarshal.Cast<byte, ushort>(
            transferBufferVerticesAndIndicesSpan[(sizeof(VertexPositionTexture) * 4)..]);

        indexData[0] = 0;
        indexData[1] = 1;
        indexData[2] = 2;
        indexData[3] = 0;
        indexData[4] = 2;
        indexData[5] = 3;

        transferBufferVerticesAndIndices.Unmap();

        // Set up texture data
        var textureByteCount = surface.Width * surface.Height * 4;
        if (!Device.TryCreateTransferBuffer(textureByteCount, out var transferBufferTexture))
        {
            return false;
        }

        var transferBufferTexturePointer = transferBufferTexture!.MapAsPointer();
        NativeMemory.Copy(
            (void*)surface.DataPointer,
            (void*)transferBufferTexturePointer,
            (UIntPtr)textureByteCount);
        transferBufferTexture.Unmap();

        var uploadCommandBuffer = Device.GetCommandBuffer();
        var copyPass = uploadCommandBuffer.BeginCopyPass();

        copyPass.UploadToDataBuffer(
            transferBufferVerticesAndIndices,
            0,
            _vertexBuffer,
            0,
            sizeof(VertexPositionTexture) * 4);

        copyPass.UploadToDataBuffer(
            transferBufferVerticesAndIndices,
            sizeof(VertexPositionTexture) * 4,
            _indexBuffer,
            0,
            sizeof(ushort) * 6);

        copyPass.UploadToTexture(
            transferBufferTexture,
            0,
            _texture,
            surface.Width,
            surface.Height);

        copyPass.End();
        uploadCommandBuffer.Submit();
        surface.Dispose();
        transferBufferVerticesAndIndices.Dispose();
        transferBufferTexture.Dispose();

        // Finally, print instructions!
        Console.WriteLine("Press LEFT/RIGHT to switch between sampler states");
        Console.WriteLine("Setting sampler state to: {0}", SamplerNames[_currentSamplerIndex]);

        return true;
    }

    public override void Quit()
    {
        _pipeline?.Dispose();
        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();
        _texture?.Dispose();

        for (var i = 0; i < SamplerNames.Length; i += 1)
        {
            _samplers[i]?.Dispose();
        }

        _currentSamplerIndex = 0;

        base.Quit();
    }

    public override void KeyboardEvent(in SDL_KeyboardEvent e)
    {
        switch (e.scancode)
        {
            case SDL_Scancode.SDL_SCANCODE_LEFT:
                _currentSamplerIndex -= 1;
                if (_currentSamplerIndex < 0)
                {
                    _currentSamplerIndex = SamplerNames.Length - 1;
                }

                Console.WriteLine("Setting sampler state to: {0}", SamplerNames[_currentSamplerIndex]);
                break;
            case SDL_Scancode.SDL_SCANCODE_RIGHT:
                _currentSamplerIndex = (_currentSamplerIndex + 1) % SamplerNames.Length;
                Console.WriteLine("Setting sampler state to: {0}", SamplerNames[_currentSamplerIndex]);
                break;
        }
    }

    public override void Update(float deltaTime)
    {
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
        renderPass.BindFragmentSampler(_texture!, _samplers[_currentSamplerIndex]!);

        renderPass.DrawPrimitivesIndexed(6, 1, 0, 0, 0);

        renderPass.End();
        commandBuffer.Submit();
    }
}
