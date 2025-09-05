// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Numerics;
using Games.MinecraftClone.Engine;

namespace Games.MinecraftClone.ECS.Components;

public record struct PositionComponent : IComponent
{
    public Vector2 Position;
}
