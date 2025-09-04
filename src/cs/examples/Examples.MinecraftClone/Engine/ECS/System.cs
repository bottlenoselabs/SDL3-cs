// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Immutable;

namespace Examples.MinecraftClone.Engine.ECS;

public abstract class System
{
    public World World { get; internal set; } = null!;

    internal ImmutableArray<Type> ComponentTypes = [];

    protected internal ImmutableArray<ComponentId> ComponentIds = [];

    public abstract void OnStart();

    public abstract void OnStop();

    public abstract void OnExecute(in Entity entity);
}
