// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E004_BasicVertexBuffer : ExampleGpu
{
    private GpuGraphicsPipeline? _pipeline;
    private GpuDataBuffer? _vertexBuffer;

    public override bool OnStart()
    {
        if (!base.OnStart())
        {
            return false;
        }

        var vertexShaderOptions = new GpuGraphicsShaderOptions();
        if (!FileSystem.TryLoadGraphicsShader(
                GetShaderFilePath("PositionColor.vert"), Device, vertexShaderOptions, out var vertexShader))
        {
            return false;
        }

        var fragmentShaderOptions = new GpuGraphicsShaderOptions();
        if (!FileSystem.TryLoadGraphicsShader(
                GetShaderFilePath("SolidColor.frag"), Device, fragmentShaderOptions, out var fragmentShader))
        {
            return false;
        }

        // Create the pipeline
        using var graphicsPipelineOptions = new GpuGraphicsPipelineOptions();
        graphicsPipelineOptions.PrimitiveType = GpuGraphicsPipelineVertexPrimitiveType.TriangleList;
        graphicsPipelineOptions.VertexShader = vertexShader;
        graphicsPipelineOptions.FragmentShader = fragmentShader;
        graphicsPipelineOptions.RasterizerState.FillMode = GpuGraphicsPipelineFillMode.Fill;
        graphicsPipelineOptions.SetVertexAttributes<VertexPositionColor>();
        graphicsPipelineOptions.SetVertexBufferDescription<VertexPositionColor>();
        graphicsPipelineOptions.SetRenderTargetColor(Window.Swapchain!);

        if (!Device.TryCreateGraphicsPipeline(graphicsPipelineOptions, out _pipeline))
        {
            return false;
        }

        vertexShader.Dispose();
        fragmentShader.Dispose();

        if (!Device.TryCreateDataBuffer<VertexPositionColor>(GpuBufferUsageFlags.Vertex, 3, out _vertexBuffer))
        {
            return false;
        }

        // To get data into the vertex buffer, we have to use a transfer buffer
        if (!Device.TryCreateUploadTransferBuffer(sizeof(VertexPositionColor) * 3, out var transferBuffer))
        {
            return false;
        }

        var transferBufferSpan = transferBuffer.MapAsSpan();
        var data = MemoryMarshal.Cast<byte, VertexPositionColor>(transferBufferSpan);

        data[0].Position = new Vector3(-1, -1, 0);
        data[0].Color = Rgba8U.Red;

        data[1].Position = new Vector3(1, -1, 0);
        data[1].Color = Rgba8U.Lime;

        data[2].Position = new Vector3(0, 1, 0);
        data[2].Color = Rgba8U.Blue;

        transferBuffer.Unmap();

        // Upload transfer data to the vertex buffer
        var uploadCommandBuffer = Device.GetCommandBuffer();
        var copyPass = uploadCommandBuffer.BeginCopyPass();
        copyPass.UploadToDataBuffer(
            transferBuffer,
            0,
            _vertexBuffer,
            0,
            sizeof(VertexPositionColor) * 3);
        copyPass.End();
        uploadCommandBuffer.Submit();
        transferBuffer.Dispose();

        return true;
    }

    public override void OnExit()
    {
        _pipeline?.Dispose();
        _vertexBuffer?.Dispose();
        base.OnExit();
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
        renderTargetInfoColor.LoadOperation = GpuRenderTargetLoadOperation.Clear;
        renderTargetInfoColor.StoreOp = GpuRenderTargetStoreOp.Store;
        renderTargetInfoColor.ClearColor = Rgba32F.Black;
        var renderPass = commandBuffer.BeginRenderPass(null, renderTargetInfoColor);
        renderPass.BindPipeline(_pipeline!);
        renderPass.BindVertexBuffer(_vertexBuffer);

        renderPass.DrawPrimitives(3, 1, 0, 0);

        renderPass.End();
        commandBuffer.Submit();
    }
}
