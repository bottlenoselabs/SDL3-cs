// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Numerics;
using bottlenoselabs.SDL;
using Games.MinecraftClone.ECS.Components;
using Games.MinecraftClone.Engine;
using Games.MinecraftClone.Engine.ECS;

namespace Games.MinecraftClone.ECS.Systems.Update;

public sealed class ArcBallCameraSystem : System<CameraControlsComponent, CameraDrawComponent>
{
    private readonly Window _window;

    public ArcBallCameraSystem(Window window)
    {
        _window = window;
    }

    public override void OnStart()
    {
    }

    public override void OnStop()
    {
    }

    protected override void OnExecute(
        TimeSpan deltaTime,
        in Entity entity,
        ref CameraControlsComponent controls,
        ref CameraDrawComponent camera)
    {
        var cameraTarget = Vector3.Zero;
        var cameraTansformMatrix = Matrix4x4.CreateScale(0, 0, controls.Distance) *
                                  Matrix4x4.CreateFromYawPitchRoll(controls.Yaw, controls.Pitch, 0) *
                                  Matrix4x4.CreateTranslation(cameraTarget);
        var cameraPosition = Vector3.Transform(Vector3.UnitZ, cameraTansformMatrix);

        var viewMatrix = Matrix4x4.CreateLookAt(
            cameraPosition,
            cameraTarget,
            Vector3.UnitY);

        const float fieldOfViewDegrees = 60;
        const float fieldOfViewRadians = fieldOfViewDegrees * MathF.PI / 180.0f;
        const float nearPlaneDistance = 0.1f;
        const float farPlaneDistance = 10.0f;

        var windowSize = _window.SizeInPixels;
        var aspectRatio = windowSize.Width / windowSize.Height;
        var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
            fieldOfViewRadians,
            aspectRatio,
            nearPlaneDistance,
            farPlaneDistance);

        camera.ViewProjectionMatrix = viewMatrix * projectionMatrix;
    }
}
