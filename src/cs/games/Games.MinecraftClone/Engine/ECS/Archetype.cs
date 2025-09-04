// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace Games.MinecraftClone.Engine.ECS;

/// <summary>
///     Defines a data table where components are columns and entities are rows.
/// </summary>
/// <remarks><para>Each unique combination of components (order does not matter) are stored using an <see cref="Archetype" />.</para></remarks>
public record struct Archetype
{
    public ComponentId[] Components;
}
