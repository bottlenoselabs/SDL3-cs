// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using SDL.GPU;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E005_CullMode : ExampleGpu
{
    private static readonly string[] ModeNames =
    {
        "CW_CullNone",
        "CW_CullFront",
        "CW_CullBack",
        "CCW_CullNone",
        "CCW_CullFront",
        "CCW_CullBack"
    };

    private GraphicsPipeline?[] _pipelines = new GraphicsPipeline?[ModeNames.Length];
    private int _currentModeIndex;
    private DataBuffer? _vertexBufferCw;
    private DataBuffer? _vertexBufferCcw;

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

        // Create pipelines
        using var pipelineDescriptor = new GraphicsPipelineDescriptor();
        pipelineDescriptor.PrimitiveType = GraphicsPipelineVertexPrimitiveType.TriangleList;
        pipelineDescriptor.VertexShader = vertexShader;
        pipelineDescriptor.FragmentShader = fragmentShader;
        pipelineDescriptor.RasterizerState.FillMode = GraphicsPipelineFillMode.Fill;
        pipelineDescriptor.SetVertexAttributes<VertexPositionColor>();
        pipelineDescriptor.SetVertexBufferDescription<VertexPositionColor>();
        pipelineDescriptor.SetRenderTargetColor(Window.Swapchain!);

        var pipelineCount = ModeNames.Length;
        _pipelines = new GraphicsPipeline[pipelineCount];
        for (var i = 0; i < pipelineCount; i += 1)
        {
            pipelineDescriptor.RasterizerState.CullMode = (GraphicsPipelineCullMode)(i % 3);
            pipelineDescriptor.RasterizerState.FrontFace = i > 2 ?
                GraphicsPipelineFrontFace.Clockwise :
                GraphicsPipelineFrontFace.CounterClockwise;

            if (!Device.TryCreatePipeline(pipelineDescriptor, out _pipelines[i]))
            {
                return false;
            }
        }

        // Clean up shader resources
        vertexShader?.Dispose();
        fragmentShader?.Dispose();

        // Create the vertex buffers. They're the same except for the vertex order.
        if (!Device.TryCreateDataBuffer<VertexPositionColor>(3, out _vertexBufferCw))
        {
            return false;
        }

        if (!Device.TryCreateDataBuffer<VertexPositionColor>(3, out _vertexBufferCcw))
        {
            return false;
        }

        // To get data into the vertex buffer, we have to use a transfer buffer
        if (!Device.TryCreateTransferBuffer(sizeof(VertexPositionColor) * 6, out var transferBuffer))
        {
            return false;
        }

        var transBufferSpan = transferBuffer!.MapAsSpan();
        var data = MemoryMarshal.Cast<byte, VertexPositionColor>(transBufferSpan);

        // clockwise vertices
        {
            data[0].Position = new Vector3(-1, -1, 0);
            data[0].Color = Rgba8U.Red;

            data[1].Position = new Vector3(1, -1, 0);
            data[1].Color = Rgba8U.Lime;

            data[2].Position = new Vector3(0, 1, 0);
            data[2].Color = Rgba8U.Blue;
        }

        // counter-clockwise vertices
        {
            data[3].Position = new Vector3(0, 1, 0);
            data[3].Color = Rgba8U.Red;

            data[4].Position = new Vector3(1, -1, 0);
            data[4].Color = Rgba8U.Lime;

            data[5].Position = new Vector3(-1, -1, 0);
            data[5].Color = Rgba8U.Blue;
        }

        // Upload the transfer data to the vertex buffers
        var uploadCommandBuffer = Device.GetCommandBuffer();
        var copyPass = uploadCommandBuffer.BeginCopyPass();

        copyPass.UploadToDataBuffer(
            transferBuffer,
            0,
            _vertexBufferCw,
            0,
            sizeof(VertexPositionColor) * 3);

        copyPass.UploadToDataBuffer(
            transferBuffer,
            sizeof(VertexPositionColor) * 3,
            _vertexBufferCcw,
            0,
            sizeof(VertexPositionColor) * 3);

        copyPass.End();
        uploadCommandBuffer.Submit();
        transferBuffer.Dispose();

        // Finally, print instructions!
        Console.WriteLine("Press Left/Right to switch between modes");
        Console.WriteLine("Current Mode: " + ModeNames[0]);

        return true;
    }

    public override void Quit()
    {
        foreach (var pipeline in _pipelines)
        {
            pipeline?.Dispose();
        }

        _vertexBufferCw?.Dispose();
        _vertexBufferCcw?.Dispose();

        base.Quit();
    }

    public override void KeyboardEvent(in SDL_KeyboardEvent e)
    {
        if (e.scancode == SDL_Scancode.SDL_SCANCODE_LEFT)
        {
            _currentModeIndex -= 1;
            if (_currentModeIndex < 0)
            {
                _currentModeIndex = ModeNames.Length - 1;
            }

            Console.WriteLine("Current Mode: " + ModeNames[_currentModeIndex]);
        }
        else if (e.scancode == SDL_Scancode.SDL_SCANCODE_RIGHT)
        {
            _currentModeIndex = (_currentModeIndex + 1) % ModeNames.Length;
            Console.WriteLine("Current Mode: " + ModeNames[_currentModeIndex]);
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
        renderTargetInfoColor.Texture = swapchainTexture!;
        renderTargetInfoColor.LoadOp = RenderTargetLoadOp.Clear;
        renderTargetInfoColor.StoreOp = RenderTargetStoreOp.Store;
        renderTargetInfoColor.ClearColor = Rgba32F.Black;
        var renderPass = commandBuffer.BeginRenderPass(null, renderTargetInfoColor);
        renderPass.BindPipeline(_pipelines[_currentModeIndex]);

        var swapchainTextureHalfWidth = swapchainTexture!.Width / 2;
        var viewport1 = new Viewport
        {
            X = 0, Y = 0, Width = swapchainTextureHalfWidth, Height = swapchainTexture.Height
        };
        renderPass.SetViewport(viewport1);
        renderPass.BindVertexBuffer(_vertexBufferCw);
        renderPass.DrawPrimitives(3, 1, 0, 0);

        var viewport2 = new Viewport
        {
            X = swapchainTextureHalfWidth, Y = 0, Width = swapchainTextureHalfWidth, Height = swapchainTexture.Height
        };
        renderPass.SetViewport(viewport2);
        renderPass.BindVertexBuffer(_vertexBufferCcw);
        renderPass.DrawPrimitives(3, 1, 0, 0);

        renderPass.End();
        commandBuffer.Submit();
    }
}
