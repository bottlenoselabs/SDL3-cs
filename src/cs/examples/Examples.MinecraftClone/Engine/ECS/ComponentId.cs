// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace Examples.MinecraftClone.Engine.ECS;

public readonly record struct ComponentId
{
    public readonly int Index;

    public ComponentId(int index)
    {
        Index = index;
    }
}
