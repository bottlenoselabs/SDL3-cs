// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Runtime.CompilerServices;
using Interop.Runtime;

namespace Examples.MinecraftClone.Engine.ECS;

public record struct Column
{
    public IntPtr Pointer;
    public int ElementSize;
    public int ElementCount;

    public readonly unsafe Span<T> AsSpan<T>()
        where T : unmanaged
    {
        return new Span<T>((void*)Pointer, ElementSize);
    }

    public static Column Allocate(INativeAllocator allocator, int elementSize, int elementCount)
    {
        var memorySize = elementSize * elementCount;
        var elementsPointer = allocator.Allocate(memorySize);
        return new Column
        {
            Pointer = elementsPointer,
            ElementSize = elementSize,
            ElementCount = elementCount
        };
    }

    public static Column Allocate<T>(INativeAllocator allocator, int elementCount)
        where T : unmanaged
    {
        var elementSize = Unsafe.SizeOf<T>();
        return Allocate(allocator, elementSize, elementCount);
    }
}
