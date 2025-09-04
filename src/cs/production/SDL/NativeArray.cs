// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

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
}
