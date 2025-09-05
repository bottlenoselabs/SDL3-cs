// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Immutable;

namespace Games.MinecraftClone.Engine.ECS;

#pragma warning disable CA1724

public abstract class System
{
    public World World { get; internal set; } = null!;

    internal ImmutableArray<Type> ComponentTypes = [];

    protected internal ImmutableArray<ComponentId> ComponentIds = [];

    public SystemKind Kind { get; }

    protected System(SystemKind kind = SystemKind.Update)
    {
        if (kind == SystemKind.Unknown)
        {
            throw new ArgumentOutOfRangeException(nameof(kind));
        }

        Kind = kind;
    }

    public abstract void OnStart();

    public abstract void OnStop();

    public abstract void OnExecute(TimeSpan deltaTime, in Entity entity);
}
