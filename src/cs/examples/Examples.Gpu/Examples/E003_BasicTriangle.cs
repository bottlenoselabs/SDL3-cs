// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using SDL.GPU;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E003_BasicTriangle : ExampleGpu
{
    private GraphicsPipeline? _pipelineFill;
    private GraphicsPipeline? _pipelineLine;

    private Viewport _viewportSmall;
    private Rectangle _rectangleScissor;

    private bool _isEnabledWireframeMode;
    private bool _isEnabledSmallViewport;
    private bool _isEnabledScissorRectangle;

    public override bool Initialize(INativeAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        if (!Device.TryCreateShaderFromFile(
                FileSystem, GetShaderFilePath("RawTriangle.vert"), out var vertexShader))
        {
            return false;
        }

        if (!Device.TryCreateShaderFromFile(
                FileSystem, GetShaderFilePath("SolidColor.frag"), out var fragmentShader))
        {
            return false;
        }

        // Create the pipeline
        using var pipelineDescriptor = new GraphicsPipelineDescriptor();
        pipelineDescriptor.PrimitiveType = GraphicsPipelineVertexPrimitiveType.TriangleList;
        pipelineDescriptor.VertexShader = vertexShader;
        pipelineDescriptor.FragmentShader = fragmentShader;
        pipelineDescriptor.RasterizerState.FillMode = GraphicsPipelineFillMode.Fill;
        pipelineDescriptor.SetRenderTargetColor(Window.Swapchain!);

        if (!Device.TryCreatePipeline(pipelineDescriptor, out _pipelineFill))
        {
            Console.Error.WriteLine("Failed to create fill pipeline!");
            return false;
        }

        pipelineDescriptor.RasterizerState.FillMode = GraphicsPipelineFillMode.Line;
        if (!Device.TryCreatePipeline(pipelineDescriptor, out _pipelineLine))
        {
            Console.Error.WriteLine("Failed to create line pipeline!");
            return false;
        }

        // Clean up shader resources
        vertexShader?.Dispose();
        fragmentShader?.Dispose();

        _viewportSmall = new()
        {
            X = Window.Width / 4.0f,
            Y = Window.Height / 4.0f,
            Width = Window.Width / 2.0f,
            Height = Window.Height / 2.0f,
            MinDepth = 0.1f,
            MaxDepth = 1.0f
        };

        _rectangleScissor = new()
        {
            X = Window.Width / 2,
            Y = Window.Height / 2,
            Width = Window.Width / 2,
            Height = Window.Height / 2
        };

        // Finally, print instructions!
        Console.WriteLine("Press Left to toggle wireframe mode");
        Console.WriteLine("Press Down to toggle small viewport");
        Console.WriteLine("Press Right to toggle scissor rect");

        return true;
    }

    public override void Quit()
    {
        _pipelineFill?.Dispose();
        _pipelineLine?.Dispose();

        base.Quit();
    }

    public override void KeyboardEvent(in SDL_KeyboardEvent e)
    {
        if (e.scancode == SDL_Scancode.SDL_SCANCODE_LEFT)
        {
            _isEnabledWireframeMode = !_isEnabledWireframeMode;
        }

        if (e.scancode == SDL_Scancode.SDL_SCANCODE_DOWN)
        {
            _isEnabledSmallViewport = !_isEnabledSmallViewport;
        }

        if (e.scancode == SDL_Scancode.SDL_SCANCODE_RIGHT)
        {
            _isEnabledScissorRectangle = !_isEnabledScissorRectangle;
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
        renderPass.BindPipeline(_isEnabledWireframeMode ? _pipelineLine! : _pipelineFill!);

        if (_isEnabledSmallViewport)
        {
            renderPass.SetViewport(_viewportSmall);
        }

        if (_isEnabledScissorRectangle)
        {
            renderPass.SetScissorRectangle(_rectangleScissor);
        }

        renderPass.DrawPrimitives(3, 1, 0, 0);
        renderPass.End();

        commandBuffer.Submit();
    }
}
