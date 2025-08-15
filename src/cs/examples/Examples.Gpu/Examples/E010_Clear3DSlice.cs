// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E010_Clear3DSlice : ExampleGpu
{
    private GpuTexture? _texture3D;

    public override bool OnStart()
    {
        if (!base.OnStart())
        {
            return false;
        }

        var textureOptions = new GpuTextureOptions();
        textureOptions.Type = GpuTextureType.ThreeDimensional;
        textureOptions.Format = Window.Swapchain!.TextureFormat;
        textureOptions.Width = 64;
        textureOptions.Height = 64;
        textureOptions.LayerCountOrDepth = 4;
        textureOptions.MipmapLevelCount = 1;
        textureOptions.Usage = GpuTextureUsages.ColorRenderTarget | GpuTextureUsages.Sampler;
        if (!Device.TryCreateTexture(textureOptions, out _texture3D))
        {
            return false;
        }

        return true;
    }

    public override void OnExit()
    {
        _texture3D?.Dispose();
        _texture3D = null;

        base.OnExit();
    }

    public override void OnDraw(TimeSpan deltaTime)
    {
        var commandBuffer = Device.GetCommandBuffer();
        if (!commandBuffer.TryGetSwapchainTexture(Window, out var swapchainTexture))
        {
            commandBuffer.Cancel();
            return;
        }

        var renderTargetInfoColor1 = default(GpuRenderTargetInfoColor);
        renderTargetInfoColor1.Texture = _texture3D;
        renderTargetInfoColor1.IsTextureCycled = true;
        renderTargetInfoColor1.ClearColor = Rgba32F.Red;
        renderTargetInfoColor1.LoadOperation = GpuRenderTargetLoadOperation.Clear;
        renderTargetInfoColor1.StoreOp = GpuRenderTargetStoreOp.Store;
        renderTargetInfoColor1.LayerOrDepthPlane = 0;
        var renderPass1 = commandBuffer.BeginRenderPass(null, renderTargetInfoColor1);
        renderPass1.End();

        var renderTargetInfoColor2 = default(GpuRenderTargetInfoColor);
        renderTargetInfoColor2.Texture = _texture3D;
        renderTargetInfoColor2.IsTextureCycled = false;
        renderTargetInfoColor2.ClearColor = Rgba32F.Lime;
        renderTargetInfoColor2.LoadOperation = GpuRenderTargetLoadOperation.Clear;
        renderTargetInfoColor2.StoreOp = GpuRenderTargetStoreOp.Store;
        renderTargetInfoColor2.LayerOrDepthPlane = 1;
        var renderPass2 = commandBuffer.BeginRenderPass(null, renderTargetInfoColor2);
        renderPass2.End();

        var renderTargetInfoColor3 = default(GpuRenderTargetInfoColor);
        renderTargetInfoColor3.Texture = _texture3D;
        renderTargetInfoColor3.IsTextureCycled = false;
        renderTargetInfoColor3.ClearColor = Rgba32F.Blue;
        renderTargetInfoColor3.LoadOperation = GpuRenderTargetLoadOperation.Clear;
        renderTargetInfoColor3.StoreOp = GpuRenderTargetStoreOp.Store;
        renderTargetInfoColor3.LayerOrDepthPlane = 2;
        var renderPass3 = commandBuffer.BeginRenderPass(null, renderTargetInfoColor3);
        renderPass3.End();

        var renderTargetInfoColor4 = default(GpuRenderTargetInfoColor);
        renderTargetInfoColor4.Texture = _texture3D;
        renderTargetInfoColor4.IsTextureCycled = false;
        renderTargetInfoColor4.ClearColor = Rgba32F.Magenta;
        renderTargetInfoColor4.LoadOperation = GpuRenderTargetLoadOperation.Clear;
        renderTargetInfoColor4.StoreOp = GpuRenderTargetStoreOp.Store;
        renderTargetInfoColor4.LayerOrDepthPlane = 3;
        var renderPass4 = commandBuffer.BeginRenderPass(null, renderTargetInfoColor4);
        renderPass4.End();

        for (var i = 0; i < 4; i += 1)
        {
            var destinationX = (i % 2) * (swapchainTexture!.Width / 2);
            var destinationY = (i > 1) ? (swapchainTexture.Height / 2) : 0;

            var blitInfo = default(GpuBlitInfo);
            blitInfo.Source.Texture = _texture3D!;
            blitInfo.Source.LayerOrDepthPlane = i;
            blitInfo.Source.Bounds = new Rectangle
            {
                X = 0,
                Y = 0,
                Width = 64,
                Height = 64
            };
            blitInfo.Destination.Texture = swapchainTexture;
            blitInfo.Destination.Bounds = new Rectangle
            {
                X = destinationX,
                Y = destinationY,
                Width = swapchainTexture.Width / 2,
                Height = swapchainTexture.Height / 2
            };
            blitInfo.LoadOperation = GpuRenderTargetLoadOperation.Load;
            blitInfo.FilterMode = GpuSamplerFilterMode.Nearest;

            commandBuffer.BlitTexture(blitInfo);
        }

        commandBuffer.Submit();
    }
}
