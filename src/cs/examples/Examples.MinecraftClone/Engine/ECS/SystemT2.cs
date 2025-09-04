// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace Examples.MinecraftClone.Engine.ECS;

public abstract class System<T1, T2> : System
{
    public override void OnExecute(in Entity entity)
    {
        // TODO: Get component from store.
        var x = default(T1)!;
        var y = default(T2)!;
        OnExecute(entity, ref x, ref y);
    }

    protected abstract void OnExecute(in Entity entity, ref T1 component1, ref T2 component2);
}
