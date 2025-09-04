// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Numerics;

namespace Examples.MinecraftClone.ECS.Components;

public record struct CamerDrawComponent
{
    public Matrix4x4 ViewProjectionMatrix;
}
