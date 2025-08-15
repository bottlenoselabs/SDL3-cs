// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Numerics;

namespace Examples.MinecraftClone;

public sealed class CameraArcBall
{
    private const float Sensitivity = 0.01f;
    private const float Distance = 5.0f;

    private const float MinimumPitch = -(MathF.PI / 2) + 0.01f;
    private const float MaximumPitch = (MathF.PI / 2) - 0.01f;

    private float _yaw;
    private float _pitch;

    public void OnMouseMove(Vector2 mousePositionDelta)
    {
        _yaw += mousePositionDelta.X * Sensitivity;
        _pitch += mousePositionDelta.Y * Sensitivity;
        // NOTE: Clamp pitch to prevent flipping
        _pitch = Math.Clamp(_pitch, MinimumPitch, MaximumPitch);
    }

    public Matrix4x4 GetViewMatrix()
    {
        var cameraTarget = Vector3.Zero;
        var cameraArcBallMatrix = Matrix4x4.CreateScale(0, 0, Distance) *
                                  Matrix4x4.CreateFromYawPitchRoll(_yaw, _pitch, 0) *
                                  Matrix4x4.CreateTranslation(cameraTarget);
        var cameraPosition = Vector3.Transform(Vector3.UnitZ, cameraArcBallMatrix);

        var viewMatrix = Matrix4x4.CreateLookAt(
            cameraPosition,
            cameraTarget,
            Vector3.UnitY);

        return viewMatrix;
    }

    public Matrix4x4 GetProjectionMatrix(float width, float height)
    {
        const float fieldOfViewDegrees = 60;
        const float fieldOfViewRadians = fieldOfViewDegrees * MathF.PI / 180.0f;
        const float nearPlaneDistance = 0.1f;
        const float farPlaneDistance = 10.0f;

        var aspectRatio = width / height;
        var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
            fieldOfViewRadians,
            aspectRatio,
            nearPlaneDistance,
            farPlaneDistance);
        return projectionMatrix;
    }
}
