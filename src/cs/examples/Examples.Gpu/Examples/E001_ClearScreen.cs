// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E001_ClearScreen : ExampleGpu
{
    private Rgba32F _clearColor = Rgba32F.Red;

    public override void OnUpdate(TimeSpan deltaTime)
    {
        _clearColor.G += 0.01f;
        if (_clearColor.G > 1.0f)
        {
            _clearColor.G = 0.0f;
        }
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
        renderTargetInfoColor.ClearColor = _clearColor;
        var renderPass = commandBuffer.BeginRenderPass(null, renderTargetInfoColor);
        // No rendering in this example!
        renderPass.End();

        commandBuffer.Submit();
    }
}
