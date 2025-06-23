// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL.GPU;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E004_BasicVertexBuffer : ExampleGpu
{
    private GpuGraphicsPipeline? _pipeline;
    private GpuDataBuffer? _vertexBuffer;

    public override bool Initialize(INativeAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        if (!Device.TryCreateShaderFromFile(
                FileSystem, GetShaderFilePath("PositionColor.vert"), out var vertexShader))
        {
            return false;
        }

        if (!Device.TryCreateShaderFromFile(
                FileSystem, GetShaderFilePath("SolidColor.frag"), out var fragmentShader))
        {
            return false;
        }

        // Create the pipeline
        using var pipelineDescriptor = new GpuGraphicsPipelineOptions();
        pipelineDescriptor.PrimitiveType = GpuGraphicsPipelineVertexPrimitiveType.TriangleList;
        pipelineDescriptor.VertexShader = vertexShader;
        pipelineDescriptor.FragmentShader = fragmentShader;
        pipelineDescriptor.RasterizerState.FillMode = GpuGraphicsPipelineFillMode.Fill;
        pipelineDescriptor.SetVertexAttributes<VertexPositionColor>();
        pipelineDescriptor.SetVertexBufferDescription<VertexPositionColor>();
        pipelineDescriptor.SetRenderTargetColor(Window.Swapchain!);

        if (!Device.TryCreatePipeline(pipelineDescriptor, out _pipeline))
        {
            return false;
        }

        vertexShader?.Dispose();
        fragmentShader?.Dispose();

        if (!Device.TryCreateDataBuffer<VertexPositionColor>(
                3, out _vertexBuffer))
        {
            return false;
        }

        // To get data into the vertex buffer, we have to use a transfer buffer
        if (!Device.TryCreateTransferBuffer(sizeof(VertexPositionColor) * 3, out var transferBuffer))
        {
            return false;
        }

        var transferBufferSpan = transferBuffer!.MapAsSpan();
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

    public override void Quit()
    {
        _pipeline?.Dispose();
        _vertexBuffer?.Dispose();
        base.Quit();
    }

    public override void KeyboardEvent(in SDL_KeyboardEvent e)
    {
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

        var renderTargetInfoColor = default(GpuRenderTargetInfoColor);
        renderTargetInfoColor.Texture = swapchainTexture!;
        renderTargetInfoColor.LoadOp = GpuRenderTargetLoadOp.Clear;
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
