// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E011_BasicCompute : ExampleGpu
{
    private GpuGraphicsPipeline _graphicsPipeline = null!;
    private GpuTexture _texture = null!;
    private GpuSampler _sampler = null!;
    private GpuDataBuffer _vertexBuffer = null!;

    public override bool OnStart()
    {
        if (!base.OnStart())
        {
            return false;
        }

        var vertexShaderOptions = new GpuGraphicsShaderOptions();
        if (!FileSystem.TryLoadGraphicsShader(
                GetShaderFilePath("TexturedQuad.vert"),
                Device,
                vertexShaderOptions,
                out var vertexShader))
        {
            return false;
        }

        var fragmentShaderOptions = new GpuGraphicsShaderOptions();
        fragmentShaderOptions.SamplerCount = 1;
        if (!FileSystem.TryLoadGraphicsShader(
                GetShaderFilePath("TexturedQuad.frag"),
                Device,
                fragmentShaderOptions,
                out var fragmentShader))
        {
            return false;
        }

        var computeShaderOptions = new GpuComputeShaderOptions();
        computeShaderOptions.ReadWriteStorageTexturesCount = 1;
        computeShaderOptions.ThreadsXCount = 8;
        computeShaderOptions.ThreadsYCount = 8;
        computeShaderOptions.ThreadsZCount = 1;
        if (!FileSystem.TryLoadComputeShader(
                GetShaderFilePath("FillTexture.comp"),
                Device,
                computeShaderOptions,
                out var computeShader))
        {
            return false;
        }

        var graphicsPipelineOptions = new GpuGraphicsPipelineOptions();
        graphicsPipelineOptions.SetRenderTargetColor(Window.Swapchain!);
        graphicsPipelineOptions.SetVertexAttributes<VertexPositionTexture>();
        graphicsPipelineOptions.SetVertexBufferDescription<VertexPositionTexture>();
        graphicsPipelineOptions.PrimitiveType = GpuGraphicsPipelineVertexPrimitiveType.TriangleList;
        graphicsPipelineOptions.VertexShader = vertexShader;
        graphicsPipelineOptions.FragmentShader = fragmentShader;

        if (!Device.TryCreateGraphicsPipeline(graphicsPipelineOptions, out _graphicsPipeline!))
        {
            return false;
        }

        vertexShader.Dispose();
        fragmentShader.Dispose();

        var textureOptions = new GpuTextureOptions();
        textureOptions.Type = GpuTextureType.TwoDimensional;
        textureOptions.Format = GpuTextureFormat.R8G8B8A8_UNORM;
        textureOptions.Width = Window.SizeInPixels.Width;
        textureOptions.Height = Window.SizeInPixels.Height;
        textureOptions.LayersCountOrDepth = 1;
        textureOptions.MipmapLevelsCount = 1;
        textureOptions.Usage = GpuTextureUsages.ComputeStorageWrite | GpuTextureUsages.Sampler;
        if (!Device.TryCreateTexture(textureOptions, out _texture!))
        {
            return false;
        }

        var samplerOptions = new GpuSamplerOptions();
        samplerOptions.AddressModeU = GpuSamplerAddressMode.Repeat;
        samplerOptions.AddressModeV = GpuSamplerAddressMode.Repeat;
        if (!Device.TryCreateSampler(samplerOptions, out _sampler!))
        {
            return false;
        }

        var verticesCount = 6;
        var verticesBytesCount = VertexPositionTexture.SizeOf * verticesCount;
        if (!Device.TryCreateDataBuffer<VertexPositionTexture>(GpuBufferUsageFlags.Vertex, verticesCount, out _vertexBuffer!))
        {
            return false;
        }

        if (!Device.TryCreateUploadTransferBuffer(verticesBytesCount, out var uploadBuffer))
        {
            return false;
        }

        var uploadBufferSpan = uploadBuffer.MapAsSpan();
        var data = MemoryMarshal.Cast<byte, VertexPositionTexture>(uploadBufferSpan);

        VertexPositionTexture.RectangleNonIndexed(data);

        uploadBuffer.Unmap();

        var commandBuffer = Device.GetCommandBuffer();
        var copyPass = commandBuffer.BeginCopyPass();

        copyPass.UploadToDataBuffer(
            uploadBuffer,
            0,
            _vertexBuffer,
            0,
            verticesBytesCount);

        copyPass.End();

        var computePass = commandBuffer.BeginComputePass(
            new GpuComputePassParameters
            {
                TextureWriteBindings =
                [
                    new GpuComputePassBindingTextureReadWrite { Texture = _texture }
                ],
            });

        computePass.BindShader(computeShader);
        computePass.Dispatch(
            Window.SizeInPixels.Width / 8,
            Window.SizeInPixels.Height / 8,
            1);

        computePass.End();

        commandBuffer.Submit();

        computeShader.Dispose();
        uploadBuffer.Dispose();

        return true;
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
        renderTargetInfoColor.ClearColor = Rgba32F.Transparent;
        renderTargetInfoColor.LoadOperation = GpuRenderTargetLoadOperation.Clear;
        renderTargetInfoColor.StoreOp = GpuRenderTargetStoreOp.Store;
        var renderPass = commandBuffer.BeginRenderPass(null, renderTargetInfoColor);

        renderPass.BindPipeline(_graphicsPipeline);
        renderPass.BindVertexBuffer(_vertexBuffer);
        renderPass.BindFragmentSampler(_texture, _sampler);
        renderPass.DrawPrimitives(6, 1, 0, 0);

        renderPass.End();
        commandBuffer.Submit();
    }
}
