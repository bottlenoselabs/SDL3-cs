// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL.GPU;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E007_InstancedIndex : ExampleGpu
{
    private GpuGraphicsPipeline? _pipeline;
    private GpuDataBuffer? _vertexBuffer;
    private GpuDataBuffer? _indexBuffer;

    private bool _isEnabledVertexOffset;
    private bool _isEnabledIndexOffset;
    private bool _isEnabledIndexBuffer = true;

    public override bool Initialize(INativeAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        if (!Device.TryCreateShaderFromFile(
                FileSystem, GetShaderFilePath("PositionColorInstanced.vert"), out var vertexShader))
        {
            return false;
        }

        if (!Device.TryCreateShaderFromFile(
                FileSystem, GetShaderFilePath("SolidColor.frag"), out var fragmentShader))
        {
            return false;
        }

        using var pipelineDescriptor = new GpuGraphicsPipelineOptions();
        pipelineDescriptor.PrimitiveType = GpuGraphicsPipelineVertexPrimitiveType.TriangleList;
        pipelineDescriptor.VertexShader = vertexShader;
        pipelineDescriptor.FragmentShader = fragmentShader;
        pipelineDescriptor.SetVertexAttributes<VertexPositionColor>();
        pipelineDescriptor.SetVertexBufferDescription<VertexPositionColor>();
        pipelineDescriptor.SetRenderTargetColor(Window.Swapchain!);

        if (!Device.TryCreatePipeline(pipelineDescriptor, out _pipeline))
        {
            return false;
        }

        vertexShader?.Dispose();
        fragmentShader?.Dispose();

        if (!Device.TryCreateDataBuffer<VertexPositionColor>(9, out _vertexBuffer))
        {
            return false;
        }

        if (!Device.TryCreateDataBuffer<ushort>(6, out _indexBuffer))
        {
            return false;
        }

        if (!Device.TryCreateTransferBuffer(
                (sizeof(VertexPositionColor) * 9) + (sizeof(ushort) * 6), out var transferBuffer))
        {
            return false;
        }

        var transferBufferSpan = transferBuffer!.MapAsSpan();
        var vertexData = MemoryMarshal.Cast<byte, VertexPositionColor>(
            transferBufferSpan[..(sizeof(VertexPositionColor) * 9)]);

        vertexData[0].Position = new Vector3(-1f, -1f, 0);
        vertexData[0].Color = Rgba8U.Red;

        vertexData[1].Position = new Vector3(1f, -1f, 0);
        vertexData[1].Color = Rgba8U.Lime;

        vertexData[2].Position = new Vector3(0, 1f, 0);
        vertexData[2].Color = Rgba8U.Blue;

        vertexData[3].Position = new Vector3(-1, -1, 0);
        vertexData[3].Color = new Rgba8U(255, 165, 0, 255);

        vertexData[4].Position = new Vector3(1, -1, 0);
        vertexData[4].Color = new Rgba8U(0, 128, 0, 255);

        vertexData[5].Position = new Vector3(0, 1, 0);
        vertexData[5].Color = Rgba8U.Cyan;

        vertexData[6].Position = new Vector3(-1, -1, 0);
        vertexData[6].Color = Rgba8U.White;

        vertexData[7].Position = new Vector3(1, -1, 0);
        vertexData[7].Color = Rgba8U.White;

        vertexData[8].Position = new Vector3(0, 1, 0);
        vertexData[8].Color = Rgba8U.White;

        var indexData = MemoryMarshal.Cast<byte, ushort>(
            transferBufferSpan[(sizeof(VertexPositionColor) * 9)..]);

        for (var i = 0; i < 6; i += 1)
        {
            indexData[i] = (ushort)i;
        }

        transferBuffer.Unmap();

        var uploadCommandBuffer = Device.GetCommandBuffer();
        var copyPass = uploadCommandBuffer.BeginCopyPass();

        copyPass.UploadToDataBuffer(
            transferBuffer,
            0,
            _vertexBuffer,
            0,
            sizeof(VertexPositionColor) * 9);

        copyPass.UploadToDataBuffer(
            transferBuffer,
            sizeof(VertexPositionColor) * 9,
            _indexBuffer,
            0,
            sizeof(ushort) * 6);

        copyPass.End();
        uploadCommandBuffer.Submit();
        transferBuffer.Dispose();

        // Finally, print instructions!
        Console.WriteLine("Press LEFT to enable/disable vertex offset");
        Console.WriteLine("Press RIGHT to enable/disable index offset");
        Console.WriteLine("Press UP to enable/disable index buffer");

        return true;
    }

    public override void Quit()
    {
        _pipeline?.Dispose();
        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();
        base.Quit();
    }

    public override void KeyboardEvent(in SDL_KeyboardEvent e)
    {
        switch (e.scancode)
        {
            case SDL_Scancode.SDL_SCANCODE_LEFT:
                _isEnabledVertexOffset = !_isEnabledVertexOffset;
                Console.WriteLine("Using vertex offset: {0}", _isEnabledVertexOffset ? "true" : "false");
                break;
            case SDL_Scancode.SDL_SCANCODE_RIGHT:
                _isEnabledIndexOffset = !_isEnabledIndexOffset;
                Console.WriteLine("Using index offset: {0}", _isEnabledIndexOffset ? "true" : "false");
                break;
            case SDL_Scancode.SDL_SCANCODE_UP:
                _isEnabledIndexBuffer = !_isEnabledIndexBuffer;
                Console.WriteLine("Using index buffer: {0}", _isEnabledIndexBuffer ? "true" : "false");
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

        var renderTargetInfoColor = default(GpuRenderTargetInfoColor);
        renderTargetInfoColor.Texture = swapchainTexture;
        renderTargetInfoColor.ClearColor = Rgba32F.Black;
        renderTargetInfoColor.LoadOp = GpuRenderTargetLoadOp.Clear;
        renderTargetInfoColor.StoreOp = GpuRenderTargetStoreOp.Store;
        var renderPass = commandBuffer.BeginRenderPass(null, renderTargetInfoColor);

        renderPass.BindPipeline(_pipeline);
        renderPass.BindVertexBuffer(_vertexBuffer);

        var vertexOffset = (ushort)(_isEnabledVertexOffset ? 3 : 0);
        var indexOffset = (ushort)(_isEnabledIndexOffset ? 3 : 0);
        if (_isEnabledIndexBuffer)
        {
            renderPass.BindIndexBuffer(_indexBuffer);
            renderPass.DrawPrimitivesIndexed(3, 16, indexOffset, vertexOffset, 0);
        }
        else
        {
            renderPass.DrawPrimitives(3, 16, vertexOffset, 0);
        }

        renderPass.End();
        commandBuffer.Submit();
    }
}
