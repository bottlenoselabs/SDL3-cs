// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using SDL.GPU;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E006_BasicStencil : ExampleGpu
{
    private bool _isSupported = true;
    private GraphicsPipeline? _pipelineMasker;
    private GraphicsPipeline? _pipelineMaskee;
    private DataBuffer? _vertexBuffer;
    private Texture? _textureDepthStencilTarget;

    public override bool Initialize(INativeAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        var depthStencilFormat = Device.SupportedDepthStencilTargetFormat;
        if (depthStencilFormat == TextureFormat.Invalid)
        {
            Console.WriteLine("Stencil formats not supported!");
            _isSupported = false;
            return true;
        }

        int width, height;
        SDL_GetWindowSizeInPixels((SDL_Window*)Window.Handle, &width, &height);

        var textureDescriptor = new TextureDescriptor();
        textureDescriptor.Type = TextureType.TwoDimensional;
        textureDescriptor.Width = width;
        textureDescriptor.Height = height;
        textureDescriptor.LayerCountOrDepth = 1;
        textureDescriptor.MipmapLevelCount = 1;
        textureDescriptor.SampleCount = 1;
        textureDescriptor.Format = depthStencilFormat;
        textureDescriptor.Usage = TextureUsages.DepthStencilRenderTarget;
        if (!Device.TryCreateTexture(textureDescriptor, out _textureDepthStencilTarget))
        {
            Console.Error.WriteLine("Failed to create texture!");
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

        using var pipelineDescriptor = new GraphicsPipelineDescriptor();
        pipelineDescriptor.PrimitiveType = GraphicsPipelineVertexPrimitiveType.TriangleList;
        pipelineDescriptor.VertexShader = vertexShader;
        pipelineDescriptor.FragmentShader = fragmentShader;
        pipelineDescriptor.SetVertexAttributes<VertexPositionColor>();
        pipelineDescriptor.SetVertexBufferDescription<VertexPositionColor>();
        pipelineDescriptor.SetRenderTargetColor(Window.Swapchain!);

        pipelineDescriptor.IsEnabledDepthStencilRenderTarget = true;
        pipelineDescriptor.DepthStencilRenderTargetFormat = depthStencilFormat;

        var depthStencilState = pipelineDescriptor.DepthStencilState;
        depthStencilState.IsEnabledStencilTest = true;
        depthStencilState.WriteMask = 0xFF;
        var frontStencilState = depthStencilState.FrontStencilState;
        frontStencilState.CompareOp = CompareOp.Never;
        frontStencilState.FailOp = StencilOp.Replace;
        frontStencilState.PassOp = StencilOp.Keep;
        frontStencilState.DepthFailOp = StencilOp.Keep;
        var backStencilState = depthStencilState.BackStencilState;
        backStencilState.CompareOp = CompareOp.Never;
        backStencilState.FailOp = StencilOp.Replace;
        backStencilState.PassOp = StencilOp.Keep;
        backStencilState.DepthFailOp = StencilOp.Keep;

        if (!Device.TryCreatePipeline(pipelineDescriptor, out _pipelineMasker))
        {
            return false;
        }

        depthStencilState.Reset();
        depthStencilState.IsEnabledStencilTest = true;
        depthStencilState.ReadMask = 0xFF;
        depthStencilState.WriteMask = 0;
        frontStencilState.CompareOp = CompareOp.Equal;
        frontStencilState.FailOp = StencilOp.Keep;
        frontStencilState.PassOp = StencilOp.Keep;
        frontStencilState.DepthFailOp = StencilOp.Keep;
        backStencilState.CompareOp = CompareOp.Never;
        backStencilState.FailOp = StencilOp.Keep;
        backStencilState.PassOp = StencilOp.Keep;
        backStencilState.DepthFailOp = StencilOp.Keep;

        if (!Device.TryCreatePipeline(pipelineDescriptor, out _pipelineMaskee))
        {
            return false;
        }

        vertexShader?.Dispose();
        fragmentShader?.Dispose();

        if (!Device.TryCreateDataBuffer<VertexPositionColor>(6, out _vertexBuffer))
        {
            return false;
        }

        if (!Device.TryCreateTransferBuffer(sizeof(VertexPositionColor) * 6, out var transferBuffer))
        {
            return false;
        }

        var transferBufferSpan = transferBuffer!.MapAsSpan();
        var data = MemoryMarshal.Cast<byte, VertexPositionColor>(transferBufferSpan);

        data[0].Position = new Vector3(-0.5f, -0.5f, 0);
        data[0].Color = Rgba8U.Yellow;

        data[1].Position = new Vector3(0.5f, -0.5f, 0);
        data[1].Color = Rgba8U.Yellow;

        data[2].Position = new Vector3(0, 0.5f, 0);
        data[2].Color = Rgba8U.Yellow;

        data[3].Position = new Vector3(-1, -1, 0);
        data[3].Color = Rgba8U.Red;

        data[4].Position = new Vector3(1, -1, 0);
        data[4].Color = Rgba8U.Lime;

        data[5].Position = new Vector3(0, 1, 0);
        data[5].Color = Rgba8U.Blue;

        // Upload transfer data to vertex buffer
        var uploadCommandBuffer = Device.GetCommandBuffer();
        var copyPass = uploadCommandBuffer.BeginCopyPass();

        copyPass.UploadToDataBuffer(
            transferBuffer,
            0,
            _vertexBuffer,
            0,
            sizeof(VertexPositionColor) * 6);

        copyPass.End();
        uploadCommandBuffer.Submit();
        transferBuffer.Dispose();

        return true;
    }

    public override void Quit()
    {
        _textureDepthStencilTarget?.Dispose();
        _pipelineMasker?.Dispose();
        _pipelineMaskee?.Dispose();
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
        if (!_isSupported)
        {
            return;
        }

        var commandBuffer = Device.GetCommandBuffer();
        if (!commandBuffer.TryGetSwapchainTexture(Window, out var swapchainTexture))
        {
            commandBuffer.Cancel();
            return;
        }

        var renderTargetInfoColor = default(RenderTargetInfoColor);
        renderTargetInfoColor.Texture = swapchainTexture;
        renderTargetInfoColor.ClearColor = Rgba32F.Black;
        renderTargetInfoColor.LoadOp = RenderTargetLoadOp.Clear;
        renderTargetInfoColor.StoreOp = RenderTargetStoreOp.Store;

        var depthStencil = default(RenderTargetInfoDepthStencil);
        depthStencil.Texture = _textureDepthStencilTarget;
        depthStencil.IsTextureCycled = true;
        depthStencil.ClearDepth = 0;
        depthStencil.ClearStencil = 0;
        depthStencil.LoadOp = RenderTargetLoadOp.Clear;
        depthStencil.StoreOp = RenderTargetStoreOp.DontCare;
        depthStencil.StencilLoadOp = RenderTargetLoadOp.Clear;
        depthStencil.StencilStoreOp = RenderTargetStoreOp.DontCare;

        var renderPass = commandBuffer.BeginRenderPass(depthStencil, renderTargetInfoColor);

        renderPass.BindVertexBuffer(_vertexBuffer);
        renderPass.SetStencilReference(1);

        renderPass.BindPipeline(_pipelineMasker);
        renderPass.DrawPrimitives(3, 1, 0, 0);

        renderPass.SetStencilReference(0);
        renderPass.BindPipeline(_pipelineMaskee);
        renderPass.DrawPrimitives(3, 1, 3, 0);

        renderPass.End();
        commandBuffer.Submit();
    }
}
