// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace Games.MinecraftClone.Engine.ECS;

#pragma warning disable CA1724

public abstract class System<T1> : System
    where T1 : unmanaged
{
    protected System(SystemKind kind = SystemKind.Update)
        : base(kind)
    {
    }

    public override void OnExecute(
        TimeSpan deltaTime,
        in Entity entity)
    {
        // TODO: Get component from store.
        var x = default(T1);
        OnExecute(deltaTime, entity, ref x);
    }

    protected abstract void OnExecute(
        TimeSpan deltaTime,
        in Entity entity,
        ref T1 component);
}
