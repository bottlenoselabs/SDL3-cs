// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

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

    private GpuGraphicsPipeline? _pipeline;
    private GpuDataBuffer? _vertexBuffer;
    private GpuDataBuffer? _indexBuffer;
    private GpuTexture? _texture;
    private readonly GpuSampler?[] _samplers = new GpuSampler[SamplerNames.Length];

    private int _currentSamplerIndex;

    public override bool OnStart()
    {
        if (!base.OnStart())
        {
            return false;
        }

        var vertexShaderOptions = new GpuGraphicsShaderOptions();
        if (!FileSystem.TryLoadGraphicsShader(
                GetShaderFilePath("TexturedQuad.vert"), Device, vertexShaderOptions, out var vertexShader))
        {
            return false;
        }

        var fragmentShaderOptions = new GpuGraphicsShaderOptions();
        fragmentShaderOptions.SamplerCount = 1;
        if (!FileSystem.TryLoadGraphicsShader(
                GetShaderFilePath("TexturedQuad.frag"), Device, fragmentShaderOptions, out var fragmentShader))
        {
            return false;
        }

        using var pipelineDescriptor = new GpuGraphicsPipelineOptions();
        pipelineDescriptor.PrimitiveType = GpuGraphicsPipelineVertexPrimitiveType.TriangleList;
        pipelineDescriptor.VertexShader = vertexShader;
        pipelineDescriptor.FragmentShader = fragmentShader;
        pipelineDescriptor.SetVertexAttributes<VertexPositionTexture>();
        pipelineDescriptor.SetVertexBufferDescription<VertexPositionTexture>();
        pipelineDescriptor.SetRenderTargetColor(Window.Swapchain!);

        if (!Device.TryCreateGraphicsPipeline(pipelineDescriptor, out _pipeline))
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

        var textureDescriptor = new GpuTextureOptions();
        textureDescriptor.Name = "Ravioli Texture 🖼️";
        textureDescriptor.Type = GpuTextureType.TwoDimensional;
        textureDescriptor.Format = GpuTextureFormat.R8G8B8A8_UNORM;
        textureDescriptor.Width = surface!.Width;
        textureDescriptor.Height = surface.Height;
        textureDescriptor.LayerCountOrDepth = 1;
        textureDescriptor.MipmapLevelCount = 1;
        textureDescriptor.Usage = GpuTextureUsages.Sampler;

        if (!Device.TryCreateTexture(textureDescriptor, out _texture))
        {
            return false;
        }

        // PointClamp
        using var samplerDescriptor = new GpuSamplerOptions();
        samplerDescriptor.MinificationFilterMode = GpuSamplerFilterMode.Nearest;
        samplerDescriptor.MagnificationFilterMode = GpuSamplerFilterMode.Nearest;
        samplerDescriptor.MipMapMode = GpuSamplerMipmapMode.Nearest;
        samplerDescriptor.AddressModeU = GpuSamplerAddressMode.ClampToEdge;
        samplerDescriptor.AddressModeV = GpuSamplerAddressMode.ClampToEdge;
        samplerDescriptor.AddressModeW = GpuSamplerAddressMode.ClampToEdge;
        if (!Device.TryCreateSampler(samplerDescriptor, out _samplers[0]))
        {
            return false;
        }

        // PointWrap
        samplerDescriptor.Reset();
        samplerDescriptor.MinificationFilterMode = GpuSamplerFilterMode.Nearest;
        samplerDescriptor.MagnificationFilterMode = GpuSamplerFilterMode.Nearest;
        samplerDescriptor.MipMapMode = GpuSamplerMipmapMode.Nearest;
        samplerDescriptor.AddressModeU = GpuSamplerAddressMode.Repeat;
        samplerDescriptor.AddressModeV = GpuSamplerAddressMode.Repeat;
        samplerDescriptor.AddressModeW = GpuSamplerAddressMode.Repeat;
        if (!Device.TryCreateSampler(samplerDescriptor, out _samplers[1]))
        {
            return false;
        }

        // LinearClamp
        samplerDescriptor.Reset();
        samplerDescriptor.MinificationFilterMode = GpuSamplerFilterMode.Linear;
        samplerDescriptor.MagnificationFilterMode = GpuSamplerFilterMode.Linear;
        samplerDescriptor.MipMapMode = GpuSamplerMipmapMode.Linear;
        samplerDescriptor.AddressModeU = GpuSamplerAddressMode.ClampToEdge;
        samplerDescriptor.AddressModeV = GpuSamplerAddressMode.ClampToEdge;
        samplerDescriptor.AddressModeW = GpuSamplerAddressMode.ClampToEdge;
        if (!Device.TryCreateSampler(samplerDescriptor, out _samplers[2]))
        {
            return false;
        }

        // LinearWrap
        samplerDescriptor.Reset();
        samplerDescriptor.MinificationFilterMode = GpuSamplerFilterMode.Linear;
        samplerDescriptor.MagnificationFilterMode = GpuSamplerFilterMode.Linear;
        samplerDescriptor.MipMapMode = GpuSamplerMipmapMode.Linear;
        samplerDescriptor.AddressModeU = GpuSamplerAddressMode.Repeat;
        samplerDescriptor.AddressModeV = GpuSamplerAddressMode.Repeat;
        samplerDescriptor.AddressModeW = GpuSamplerAddressMode.Repeat;
        if (!Device.TryCreateSampler(samplerDescriptor, out _samplers[3]))
        {
            return false;
        }

        // AnisotropicClamp
        samplerDescriptor.Reset();
        samplerDescriptor.MinificationFilterMode = GpuSamplerFilterMode.Linear;
        samplerDescriptor.MagnificationFilterMode = GpuSamplerFilterMode.Linear;
        samplerDescriptor.MipMapMode = GpuSamplerMipmapMode.Linear;
        samplerDescriptor.AddressModeU = GpuSamplerAddressMode.ClampToEdge;
        samplerDescriptor.AddressModeV = GpuSamplerAddressMode.ClampToEdge;
        samplerDescriptor.AddressModeW = GpuSamplerAddressMode.ClampToEdge;
        samplerDescriptor.IsEnabledAnisotropy = true;
        samplerDescriptor.MaximumAnisotropy = 4;
        if (!Device.TryCreateSampler(samplerDescriptor, out _samplers[4]))
        {
            return false;
        }

        // AnisotropicWrap
        samplerDescriptor.Reset();
        samplerDescriptor.MinificationFilterMode = GpuSamplerFilterMode.Linear;
        samplerDescriptor.MagnificationFilterMode = GpuSamplerFilterMode.Linear;
        samplerDescriptor.MipMapMode = GpuSamplerMipmapMode.Linear;
        samplerDescriptor.AddressModeU = GpuSamplerAddressMode.Repeat;
        samplerDescriptor.AddressModeV = GpuSamplerAddressMode.Repeat;
        samplerDescriptor.AddressModeW = GpuSamplerAddressMode.Repeat;
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

    public override void OnExit()
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

        base.OnExit();
    }

    public override void OnKeyDown(in KeyboardEvent e)
    {
        switch (e.Button)
        {
            case KeyboardButton.Left:
            {
                _currentSamplerIndex -= 1;
                if (_currentSamplerIndex < 0)
                {
                    _currentSamplerIndex = SamplerNames.Length - 1;
                }

                Console.WriteLine("Setting sampler state to: {0}", SamplerNames[_currentSamplerIndex]);
                break;
            }

            case KeyboardButton.Right:
            {
                _currentSamplerIndex = (_currentSamplerIndex + 1) % SamplerNames.Length;
                Console.WriteLine("Setting sampler state to: {0}", SamplerNames[_currentSamplerIndex]);
                break;
            }
        }
    }

    public override void OnUpdate(TimeSpan deltaTime)
    {
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
        renderPass.BindFragmentSampler(_texture!, _samplers[_currentSamplerIndex]!);

        renderPass.DrawPrimitivesIndexed(6, 1, 0, 0, 0);

        renderPass.End();
        commandBuffer.Submit();
    }
}
