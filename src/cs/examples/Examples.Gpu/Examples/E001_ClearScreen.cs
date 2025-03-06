// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using SDL.GPU;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E001_ClearScreen : ExampleGpu
{
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

        var renderTargetInfoColor = default(RenderTargetInfoColor);
        renderTargetInfoColor.Texture = swapchainTexture!;
        renderTargetInfoColor.LoadOp = RenderTargetLoadOp.Clear;
        renderTargetInfoColor.StoreOp = RenderTargetStoreOp.Store;
        renderTargetInfoColor.ClearColor = Rgba32F.CornflowerBlue;
        var renderPass = commandBuffer.BeginRenderPass(null, renderTargetInfoColor);
        // No rendering in this example!
        renderPass.End();

        commandBuffer.Submit();
    }
}
