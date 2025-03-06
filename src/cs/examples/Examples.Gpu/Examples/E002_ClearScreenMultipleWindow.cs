// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using SDL.GPU;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E002_ClearScreenMultipleWindow : ExampleGpu
{
    private readonly Window _secondWindow = new();

    public override bool Initialize(INativeAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        _ = _secondWindow.TrySetPosition(0, 0);
        _ = Device.TryClaimWindow(_secondWindow);
        return true;
    }

    public override void Quit()
    {
        Device.ReleaseWindow(_secondWindow);
        _secondWindow.Dispose();

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
        if (commandBuffer.TryGetSwapchainTexture(Window, out var swapchainTextureMainWindow))
        {
            var renderTargetInfoColor = default(RenderTargetInfoColor);
            renderTargetInfoColor.Texture = swapchainTextureMainWindow!;
            renderTargetInfoColor.LoadOp = RenderTargetLoadOp.Clear;
            renderTargetInfoColor.StoreOp = RenderTargetStoreOp.Store;
            renderTargetInfoColor.ClearColor = Rgba32F.CornflowerBlue;
            var renderPass = commandBuffer.BeginRenderPass(null, renderTargetInfoColor);
            // No rendering in this example!
            renderPass.End();
        }

        if (commandBuffer.TryGetSwapchainTexture(_secondWindow, out var swapchainTextureSecondWindow))
        {
            var renderTargetInfoColor = default(RenderTargetInfoColor);
            renderTargetInfoColor.Texture = swapchainTextureSecondWindow!;
            renderTargetInfoColor.LoadOp = RenderTargetLoadOp.Clear;
            renderTargetInfoColor.StoreOp = RenderTargetStoreOp.Store;
            renderTargetInfoColor.ClearColor = Rgba32F.Indigo;
            var renderPass = commandBuffer.BeginRenderPass(null, renderTargetInfoColor);
            // No rendering in this example!
            renderPass.End();
        }

        commandBuffer.Submit();
    }
}
