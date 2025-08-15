// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E002_ClearScreenMultipleWindow : ExampleGpu
{
    private Window _secondWindow = null!;

    public override bool OnStart()
    {
        if (!base.OnStart())
        {
            return false;
        }

        using var windowOptions = new WindowOptions();
        _secondWindow = Application.CreateWindow(windowOptions);
        _ = _secondWindow.TrySetPosition(0, 0);
        _ = Device.TryClaimWindow(_secondWindow);
        return true;
    }

    public override void OnExit()
    {
        Device.ReleaseWindow(_secondWindow);
        _secondWindow.Dispose();
        _secondWindow = null!;

        base.OnExit();
    }

    public override void OnUpdate(TimeSpan deltaTime)
    {
    }

    public override void OnDraw(TimeSpan deltaTime)
    {
        var commandBuffer = Device.GetCommandBuffer();
        if (commandBuffer.TryGetSwapchainTexture(Window, out var swapchainTextureMainWindow))
        {
            var renderTargetInfoColor = default(GpuRenderTargetInfoColor);
            renderTargetInfoColor.Texture = swapchainTextureMainWindow!;
            renderTargetInfoColor.LoadOperation = GpuRenderTargetLoadOperation.Clear;
            renderTargetInfoColor.StoreOp = GpuRenderTargetStoreOp.Store;
            renderTargetInfoColor.ClearColor = Rgba32F.CornflowerBlue;
            var renderPass = commandBuffer.BeginRenderPass(null, renderTargetInfoColor);
            // No rendering in this example!
            renderPass.End();
        }

        if (commandBuffer.TryGetSwapchainTexture(_secondWindow, out var swapchainTextureSecondWindow))
        {
            var renderTargetInfoColor = default(GpuRenderTargetInfoColor);
            renderTargetInfoColor.Texture = swapchainTextureSecondWindow!;
            renderTargetInfoColor.LoadOperation = GpuRenderTargetLoadOperation.Clear;
            renderTargetInfoColor.StoreOp = GpuRenderTargetStoreOp.Store;
            renderTargetInfoColor.ClearColor = Rgba32F.Indigo;
            var renderPass = commandBuffer.BeginRenderPass(null, renderTargetInfoColor);
            // No rendering in this example!
            renderPass.End();
        }

        commandBuffer.Submit();
    }
}
