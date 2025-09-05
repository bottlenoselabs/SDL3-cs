// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using Interop.Runtime;

namespace Games.MinecraftClone.Engine.ECS;

/// <summary>
///     Defines a type erased native array of items.
/// </summary>
public record struct NativeArray
{
    /// <summary>
    ///     The pointer to the block of memory representing the elements.
    /// </summary>
    public nint ElementsPointer;

    /// <summary>
    ///     The byte size of an element.
    /// </summary>
    public int ElementStride;

    /// <summary>
    ///     The number of elements.
    /// </summary>
    public int ElementsCount;

    /// <summary>
    ///     Creates a <see cref="NativeArray" /> from a <see cref="Span{T}" />.
    /// </summary>
    /// <param name="span">The span.</param>
    /// <typeparam name="T">The unmanaged type.</typeparam>
    /// <returns>A <see cref="NativeArray" /> created from <paramref name="span" />.</returns>
    public static unsafe NativeArray CreateFromSpan<T>(Span<T> span)
        where T : unmanaged
    {
        var result = default(NativeArray);
        result.ElementsCount = span.Length;

        fixed (T* pointer = span)
        {
            result.ElementsPointer = (IntPtr)pointer;
            result.ElementStride = sizeof(T);
        }

        return result;
    }

    /// <summary>
    ///     Allocates memory for a <see cref="NativeArray" />.
    /// </summary>
    /// <param name="allocator">The allocator.</param>
    /// <param name="stride">The element byte size.</param>
    /// <param name="count">The number of elements.</param>
    /// <returns>A <see cref="NativeArray" />.</returns>
    public static NativeArray Allocate(
        INativeAllocator allocator,
        int stride,
        int count)
    {
        var result = default(NativeArray);
        result.ElementsPointer = allocator.Allocate(stride * count);
        result.ElementStride = stride;
        result.ElementsCount = count;
        return result;
    }
}
