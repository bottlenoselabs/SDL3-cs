// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Numerics;
using bottlenoselabs.SDL;
using Examples.MinecraftClone.ECS.Components;
using Examples.MinecraftClone.ECS.Systems;
using Examples.MinecraftClone.ECS.Systems.Draw;
using Examples.MinecraftClone.ECS.Systems.Update;
using Examples.MinecraftClone.Engine;
using Examples.MinecraftClone.Engine.ECS;

namespace Examples.MinecraftClone;

public sealed class Game : Application
{
    private Window _window = null!;
    private GpuDevice _device = null!;

    private CameraArcBall _arcBallCamera = null!;

    private World _world = null!;

    private Entity _camera;
    private Entity _player;

    protected override void OnStart()
    {
        var windowOptions = new WindowOptions();
        _window = CreateWindow(windowOptions);

        var gpuDeviceOptions = new GpuDeviceOptions();
        gpuDeviceOptions.IsDebugMode = true;
        _device = CreateGpuDevice(gpuDeviceOptions);
        _ = _device.TryClaimWindow(_window);

        _arcBallCamera = new CameraArcBall();

        _world = new World();

        _world.AddSystem(new ArcBallCameraSystem(_window));
        _world.AddSystem(new RenderSystem(_world, _window, FileSystem, _device));

        _camera = _world.CreateEntity();
        _camera.SetComponent(new CameraControlsComponent { Pitch = 0, Yaw = 0 });

        _player = _world.CreateEntity();
        _player.SetComponent(new PositionComponent { Position = new Vector2(0, 0) });
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
            ref var cameraControls = ref _player.GetComponent<CameraControlsComponent>();
            cameraControls.Yaw -= e.PositionDelta.X * cameraControls.Sensitivity;
            cameraControls.Pitch -= e.PositionDelta.Y * cameraControls.Sensitivity;
            // NOte: Clamp pitch to [-90.1, 89.9] degrees to prevent flipping at exactly 90 degrees.
            cameraControls.Pitch = Math.Clamp(cameraControls.Pitch, -(MathF.PI / 2) + 0.01f, (MathF.PI / 2) - 0.01f);
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
