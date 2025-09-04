// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E008_TexturedQuad : ExampleGpu
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

        using var graphicsPipelineOptions = new GpuGraphicsPipelineOptions();
        graphicsPipelineOptions.PrimitiveType = GpuGraphicsPipelineVertexPrimitiveType.TriangleList;
        graphicsPipelineOptions.VertexShader = vertexShader;
        graphicsPipelineOptions.FragmentShader = fragmentShader;
        graphicsPipelineOptions.SetVertexAttributes<VertexPositionTexture>();
        graphicsPipelineOptions.SetVertexBufferDescription<VertexPositionTexture>();
        graphicsPipelineOptions.SetRenderTargetColor(Window.Swapchain!);

        if (!Device.TryCreateGraphicsPipeline(graphicsPipelineOptions, out _pipeline))
        {
            return false;
        }

        vertexShader.Dispose();
        fragmentShader.Dispose();

        var imageFilePath = Path.Combine(AssetsDirectory, "Images", "ravioli.bmp");
        if (!FileSystem.TryLoadImage(
                imageFilePath, out var surface, PixelFormat.Abgr8888))
        {
            return false;
        }

        var textureOptions = new GpuTextureOptions();
        textureOptions.Name = "Ravioli Texture 🖼️";
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
        using var samplerOptions = new GpuSamplerOptions();
        samplerOptions.MinificationFilterMode = GpuSamplerFilterMode.Nearest;
        samplerOptions.MagnificationFilterMode = GpuSamplerFilterMode.Nearest;
        samplerOptions.MipMapMode = GpuSamplerMipmapMode.Nearest;
        samplerOptions.AddressModeU = GpuSamplerAddressMode.ClampToEdge;
        samplerOptions.AddressModeV = GpuSamplerAddressMode.ClampToEdge;
        samplerOptions.AddressModeW = GpuSamplerAddressMode.ClampToEdge;
        if (!Device.TryCreateSampler(samplerOptions, out _samplers[0]))
        {
            return false;
        }

        // PointWrap
        samplerOptions.Reset();
        samplerOptions.MinificationFilterMode = GpuSamplerFilterMode.Nearest;
        samplerOptions.MagnificationFilterMode = GpuSamplerFilterMode.Nearest;
        samplerOptions.MipMapMode = GpuSamplerMipmapMode.Nearest;
        samplerOptions.AddressModeU = GpuSamplerAddressMode.Repeat;
        samplerOptions.AddressModeV = GpuSamplerAddressMode.Repeat;
        samplerOptions.AddressModeW = GpuSamplerAddressMode.Repeat;
        if (!Device.TryCreateSampler(samplerOptions, out _samplers[1]))
        {
            return false;
        }

        // LinearClamp
        samplerOptions.Reset();
        samplerOptions.MinificationFilterMode = GpuSamplerFilterMode.Linear;
        samplerOptions.MagnificationFilterMode = GpuSamplerFilterMode.Linear;
        samplerOptions.MipMapMode = GpuSamplerMipmapMode.Linear;
        samplerOptions.AddressModeU = GpuSamplerAddressMode.ClampToEdge;
        samplerOptions.AddressModeV = GpuSamplerAddressMode.ClampToEdge;
        samplerOptions.AddressModeW = GpuSamplerAddressMode.ClampToEdge;
        if (!Device.TryCreateSampler(samplerOptions, out _samplers[2]))
        {
            return false;
        }

        // LinearWrap
        samplerOptions.Reset();
        samplerOptions.MinificationFilterMode = GpuSamplerFilterMode.Linear;
        samplerOptions.MagnificationFilterMode = GpuSamplerFilterMode.Linear;
        samplerOptions.MipMapMode = GpuSamplerMipmapMode.Linear;
        samplerOptions.AddressModeU = GpuSamplerAddressMode.Repeat;
        samplerOptions.AddressModeV = GpuSamplerAddressMode.Repeat;
        samplerOptions.AddressModeW = GpuSamplerAddressMode.Repeat;
        if (!Device.TryCreateSampler(samplerOptions, out _samplers[3]))
        {
            return false;
        }

        // AnisotropicClamp
        samplerOptions.Reset();
        samplerOptions.MinificationFilterMode = GpuSamplerFilterMode.Linear;
        samplerOptions.MagnificationFilterMode = GpuSamplerFilterMode.Linear;
        samplerOptions.MipMapMode = GpuSamplerMipmapMode.Linear;
        samplerOptions.AddressModeU = GpuSamplerAddressMode.ClampToEdge;
        samplerOptions.AddressModeV = GpuSamplerAddressMode.ClampToEdge;
        samplerOptions.AddressModeW = GpuSamplerAddressMode.ClampToEdge;
        samplerOptions.IsEnabledAnisotropy = true;
        samplerOptions.MaximumAnisotropy = 4;
        if (!Device.TryCreateSampler(samplerOptions, out _samplers[4]))
        {
            return false;
        }

        // AnisotropicWrap
        samplerOptions.Reset();
        samplerOptions.MinificationFilterMode = GpuSamplerFilterMode.Linear;
        samplerOptions.MagnificationFilterMode = GpuSamplerFilterMode.Linear;
        samplerOptions.MipMapMode = GpuSamplerMipmapMode.Linear;
        samplerOptions.AddressModeU = GpuSamplerAddressMode.Repeat;
        samplerOptions.AddressModeV = GpuSamplerAddressMode.Repeat;
        samplerOptions.AddressModeW = GpuSamplerAddressMode.Repeat;
        samplerOptions.IsEnabledAnisotropy = true;
        samplerOptions.MaximumAnisotropy = 4;
        if (!Device.TryCreateSampler(samplerOptions, out _samplers[5]))
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

        if (!Device.TryCreateUploadTransferBuffer(
                (VertexPositionTexture.SizeOf * 4) + (sizeof(ushort) * 6),
                out var transferBufferVerticesAndIndices))
        {
            return false;
        }

        var transferBufferVerticesAndIndicesSpan = transferBufferVerticesAndIndices.MapAsSpan();
        var vertexData = MemoryMarshal.Cast<byte, VertexPositionTexture>(
            transferBufferVerticesAndIndicesSpan[..(VertexPositionTexture.SizeOf * 4)]);

        vertexData[0].Position = new Vector3(-1f, 1f, 0); // top-left
        vertexData[0].TextureCoordinates = new Vector2(0, 0);

        vertexData[1].Position = new Vector3(1f, 1f, 0); // top-right
        vertexData[1].TextureCoordinates = new Vector2(4, 0);

        vertexData[2].Position = new Vector3(1, -1f, 0); // bottom-right
        vertexData[2].TextureCoordinates = new Vector2(4, 4);

        vertexData[3].Position = new Vector3(-1, -1, 0); // bottom-left
        vertexData[3].TextureCoordinates = new Vector2(0, 4);

        var indexData = MemoryMarshal.Cast<byte, ushort>(
            transferBufferVerticesAndIndicesSpan[(VertexPositionTexture.SizeOf * 4)..]);

        indexData[0] = 0;
        indexData[1] = 1;
        indexData[2] = 2;
        indexData[3] = 0;
        indexData[4] = 2;
        indexData[5] = 3;

        transferBufferVerticesAndIndices.Unmap();

        // Set up texture data
        var textureByteCount = surface.Width * surface.Height * 4;
        if (!Device.TryCreateUploadTransferBuffer(textureByteCount, out var transferBufferTexture))
        {
            return false;
        }

        var transferBufferTexturePointer = transferBufferTexture.MapAsPointer();
        unsafe
        {
            NativeMemory.Copy(
                (void*)surface.DataPointer,
                (void*)transferBufferTexturePointer,
                (UIntPtr)textureByteCount);
        }

        transferBufferTexture.Unmap();

        var uploadCommandBuffer = Device.GetCommandBuffer();
        var copyPass = uploadCommandBuffer.BeginCopyPass();

        copyPass.UploadToDataBuffer(
            transferBufferVerticesAndIndices,
            0,
            _vertexBuffer,
            0,
            VertexPositionTexture.SizeOf * 4);

        copyPass.UploadToDataBuffer(
            transferBufferVerticesAndIndices,
            VertexPositionTexture.SizeOf * 4,
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
