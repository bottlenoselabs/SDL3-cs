// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E007_InstancedIndex : ExampleGpu
{
    private GpuGraphicsPipeline? _pipeline;
    private GpuDataBuffer? _vertexBuffer;
    private GpuDataBuffer? _indexBuffer;

    private bool _isEnabledVertexOffset;
    private bool _isEnabledIndexOffset;
    private bool _isEnabledIndexBuffer = true;

    public override bool OnStart()
    {
        if (!base.OnStart())
        {
            return false;
        }

        if (!FileSystem.TryLoadGraphicsShader(
                GetShaderFilePath("PositionColorInstanced.vert"),  Device, out var vertexShader))
        {
            return false;
        }

        if (!FileSystem.TryLoadGraphicsShader(
                GetShaderFilePath("SolidColor.frag"), Device, out var fragmentShader))
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

        if (!Device.TryCreateGraphicsPipeline(pipelineDescriptor, out _pipeline))
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
                (VertexPositionColor.SizeOf * 9) + (sizeof(ushort) * 6), out var transferBuffer))
        {
            return false;
        }

        var transferBufferSpan = transferBuffer!.MapAsSpan();
        var vertexData = MemoryMarshal.Cast<byte, VertexPositionColor>(
            transferBufferSpan[..(VertexPositionColor.SizeOf * 9)]);

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
            transferBufferSpan[(VertexPositionColor.SizeOf * 9)..]);

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
            VertexPositionColor.SizeOf * 9);

        copyPass.UploadToDataBuffer(
            transferBuffer,
            VertexPositionColor.SizeOf * 9,
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

    public override void OnExit()
    {
        _pipeline?.Dispose();
        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();
        base.OnExit();
    }

    public override void OnKeyDown(in KeyboardEvent e)
    {
        switch (e.Button)
        {
            case KeyboardButton.Left:
            {
                _isEnabledVertexOffset = !_isEnabledVertexOffset;
                Console.WriteLine("Using vertex offset: {0}", _isEnabledVertexOffset ? "true" : "false");
                break;
            }

            case KeyboardButton.Right:
            {
                _isEnabledIndexOffset = !_isEnabledIndexOffset;
                Console.WriteLine("Using index offset: {0}", _isEnabledIndexOffset ? "true" : "false");
                break;
            }

            case KeyboardButton.Up:
            {
                _isEnabledIndexBuffer = !_isEnabledIndexBuffer;
                Console.WriteLine("Using index buffer: {0}", _isEnabledIndexBuffer ? "true" : "false");
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
        renderTargetInfoColor.ClearColor = Rgba32F.Black;
        renderTargetInfoColor.LoadOperation = GpuRenderTargetLoadOperation.Clear;
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
