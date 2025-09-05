// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Immutable;
using System.Runtime.InteropServices;
using bottlenoselabs.SDL;
using Interop.Runtime;

namespace Games.MinecraftClone.Engine.ECS;

public sealed class World : IDisposable
{
    internal readonly List<NativeArray> Columns = new();

    private const int MaximumEntityCount = 100;
    private int _entitiesCount;

    private readonly List<System> _updateSystems = new();
    private readonly List<System> _drawSystems = new();

    private readonly INativeAllocator _allocator = new ArenaNativeAllocator(1024 * 1024 * 100);

    private readonly Dictionary<Type, ComponentId> _registeredComponentsByType = [];
    private readonly HashSet<Type> _registeredSystemTypes = [];

    public void Dispose()
    {
        foreach (var system in _drawSystems)
        {
            system.OnStop();
        }

        foreach (var system in _updateSystems)
        {
            system.OnStop();
        }
    }

    public void Update(TimeSpan deltaTime)
    {
        foreach (var system in _updateSystems)
        {
            for (var i = 0; i <= _entitiesCount; i++)
            {
                var entity = new Entity(this, i);
                system.OnExecute(deltaTime, entity);
            }
        }
    }

    public void Draw(TimeSpan deltaTime)
    {
        foreach (var system in _drawSystems)
        {
            for (var i = 0; i <= _entitiesCount; i++)
            {
                var entity = new Entity(this, i);
                system.OnExecute(deltaTime, entity);
            }
        }
    }

    public Entity CreateEntity()
    {
        if (_entitiesCount == 100)
        {
            throw new InvalidOperationException();
        }

        var index = ++_entitiesCount;
        var entity = new Entity(this, index);
        return entity;
    }

    public void AddSystem<T1>(System<T1> system)
        where T1 : unmanaged
    {
        var systemType = system.GetType();
        ImmutableArray<Type> componentTypes = [typeof(T1)];
        AddSystem(system, systemType, componentTypes);
    }

    public void AddSystem<T1, T2>(System<T1, T2> system)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        var systemType = system.GetType();
        ImmutableArray<Type> componentTypes = [typeof(T1), typeof(T2)];
        AddSystem(system, systemType, componentTypes);
    }

    private void AddSystem(System system, Type systemType, ImmutableArray<Type> componentTypes)
    {
        RegisterSystem(system, systemType, componentTypes);
    }

    private ComponentId RegisterComponentType(Type componentType)
    {
        if (_registeredComponentsByType.TryGetValue(componentType, out var componentId))
        {
            return componentId;
        }

        var componentIndex = Columns.Count;
        var componentSize = Marshal.SizeOf(componentType);
        componentId = new ComponentId(componentIndex);
        _registeredComponentsByType.Add(componentType, componentId);

        var column = NativeArray.Allocate(_allocator, componentSize, MaximumEntityCount);
        Columns.Add(column);

        return componentId;
    }

    private void RegisterSystem(
        System system, Type systemType, ImmutableArray<Type> componentTypes)
    {
        if (_registeredSystemTypes.Contains(systemType))
        {
            return;
        }

        var componentIds = new ComponentId[componentTypes.Length];

        for (var i = 0; i < componentTypes.Length; i++)
        {
            var componentType = componentTypes[i];
            componentIds[i] = RegisterComponentType(componentType);
        }

        Array.Sort(componentIds, (a, b) => a.Index.CompareTo(b.Index));
        system.ComponentIds = [..componentIds];
        system.World = this;

        _registeredSystemTypes.Add(systemType);
        switch (system.Kind)
        {
            case SystemKind.Update:
                _updateSystems.Add(system);
                break;
            case SystemKind.Render:
                _drawSystems.Add(system);
                break;
        }

        system.OnStart();
    }
}
