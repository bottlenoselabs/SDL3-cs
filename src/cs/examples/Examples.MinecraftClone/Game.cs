// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

namespace Examples.MinecraftClone;

public sealed class Game : Application
{
    private Window _window = null!;
    private GpuDevice _device = null!;

    private CameraArcBall _camera = null!;
    private Renderer _renderer = null!;

    protected override void OnStart()
    {
        var windowOptions = new WindowOptions();
        _window = CreateWindow(windowOptions);

        var gpuDeviceOptions = new GpuDeviceOptions();
        gpuDeviceOptions.IsDebugMode = true;
        _device = CreateGpuDevice(gpuDeviceOptions);
        _ = _device.TryClaimWindow(_window);

        _camera = new CameraArcBall();
        _renderer = new Renderer(_device, _window.Swapchain!, FileSystem, _camera);
    }

    protected override void OnExit()
    {
        _device.Dispose();
        _device = null!;
    }

    protected override void OnMouseMove(in MouseMoveEvent e)
    {
        var isDownLeftButton = (e.ButtonState & MouseButtonState.Left) != 0;
        if (isDownLeftButton)
        {
            _camera.OnMouseMove(-e.PositionDelta);
        }
    }

    protected override void OnDraw(TimeSpan deltaTime)
    {
        var commandBuffer = _device.GetCommandBuffer();
        if (!commandBuffer.TryGetSwapchainTexture(_window, out var swapchainTexture))
        {
            commandBuffer.Cancel();
            return;
        }

        _renderer.Render(commandBuffer, swapchainTexture!);

        commandBuffer.Submit();
    }
}
