// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using Examples.MinecraftClone.Engine.ECS;

namespace Examples.MinecraftClone.Engine;

public readonly record struct Entity
{
    public readonly World World;
    public readonly int Id;

    internal Entity(World world, int id)
    {
        World = world;
    }

    public void SetComponent<TComponent>(TComponent component)
        where TComponent : unmanaged
    {
    }

    public ref TComponent GetComponent<TComponent>()
    {
        throw new NotImplementedException();
    }
}
