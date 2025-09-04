// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E003_BasicTriangle : ExampleGpu
{
    private GpuGraphicsPipeline? _pipelineFill;
    private GpuGraphicsPipeline? _pipelineLine;

    private GpuViewport _viewportSmall;
    private Rectangle _rectangleScissor;

    private bool _isEnabledWireframeMode;
    private bool _isEnabledSmallViewport;
    private bool _isEnabledScissorRectangle;

    public override bool OnStart()
    {
        if (!base.OnStart())
        {
            return false;
        }

        var vertexShaderOptions = new GpuGraphicsShaderOptions();
        if (!FileSystem.TryLoadGraphicsShader(
                GetShaderFilePath("RawTriangle.vert"), Device, vertexShaderOptions, out var vertexShader))
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
        graphicsPipelineOptions.SetRenderTargetColor(Window.Swapchain!);

        if (!Device.TryCreateGraphicsPipeline(graphicsPipelineOptions, out _pipelineFill))
        {
            Console.Error.WriteLine("Failed to create fill pipeline!");
            return false;
        }

        graphicsPipelineOptions.RasterizerState.FillMode = GpuGraphicsPipelineFillMode.Line;
        if (!Device.TryCreateGraphicsPipeline(graphicsPipelineOptions, out _pipelineLine))
        {
            Console.Error.WriteLine("Failed to create line pipeline!");
            return false;
        }

        // Clean up shader resources
        vertexShader.Dispose();
        fragmentShader.Dispose();

        _viewportSmall = new()
        {
            X = Window.Size.Width / 4.0f,
            Y = Window.Size.Height / 4.0f,
            Width = Window.Size.Width / 2.0f,
            Height = Window.Size.Height / 2.0f,
            MinDepth = 0.1f,
            MaxDepth = 1.0f
        };

        _rectangleScissor = new()
        {
            X = Window.Size.Width / 2,
            Y = Window.Size.Height / 2,
            Width = Window.Size.Width / 2,
            Height = Window.Size.Height / 2
        };

        // Finally, print instructions!
        Console.WriteLine("Press Left to toggle wireframe mode");
        Console.WriteLine("Press Down to toggle small viewport");
        Console.WriteLine("Press Right to toggle scissor rect");

        return true;
    }

    public override void OnExit()
    {
        _pipelineFill?.Dispose();
        _pipelineLine?.Dispose();

        base.OnExit();
    }

    public override void OnKeyDown(in KeyboardEvent e)
    {
        switch (e.Button)
        {
            case KeyboardButton.Left:
            {
                _isEnabledWireframeMode = !_isEnabledWireframeMode;
                break;
            }

            case KeyboardButton.Down:
            {
                _isEnabledSmallViewport = !_isEnabledSmallViewport;
                break;
            }

            case KeyboardButton.Right:
            {
                _isEnabledScissorRectangle = !_isEnabledScissorRectangle;
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
        renderTargetInfoColor.LoadOperation = GpuRenderTargetLoadOperation.Clear;
        renderTargetInfoColor.StoreOp = GpuRenderTargetStoreOp.Store;
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
