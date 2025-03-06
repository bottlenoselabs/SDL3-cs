// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

#pragma warning disable CA1815

/// <summary>
///     A mapped pointer from a <see cref="TransferBuffer" /> along with it's size in bytes.
/// </summary>
[PublicAPI]
public readonly ref struct TransferBufferPointer
{
    /// <summary>
    ///     The pointer to the block of data.
    /// </summary>
    public readonly IntPtr Pointer;

    /// <summary>
    ///     The number of bytes in the block of data.
    /// </summary>
    public readonly int ByteCount;

    internal TransferBufferPointer(IntPtr pointer, int byteCount)
    {
        Pointer = pointer;
        ByteCount = byteCount;
    }

    /// <summary>
    ///     Gets a span over the transfer buffer pointer.
    /// </summary>
    /// <param name="offsetInBytes">The byte offset in the buffer where the span starts.</param>
    /// <param name="elementCount">
    ///     The number of elements in the span. Use <c>0</c> to use the all the buffer's remaining data after
    ///     <paramref name="offsetInBytes" />.
    /// </param>
    /// <typeparam name="TElement">The type of element being mapped.</typeparam>
    /// <returns>The span over the transfer buffer pointer.</returns>
    /// <exception cref="ArgumentException"><typeparamref name="TElement" /> is a reference type or contains pointers and therefore cannot be stored in unmanaged memory.</exception>
    public unsafe Span<TElement> AsSpan<TElement>(int offsetInBytes = 0, int elementCount = 0)
        where TElement : unmanaged
    {
        var sizeOfElement = sizeof(TElement);
        var spanPointer = (void*)(Pointer + offsetInBytes);
        if (elementCount == 0)
        {
            elementCount = (ByteCount - offsetInBytes) / sizeOfElement;
        }

        var span = new Span<TElement>(spanPointer, elementCount);
        return span;
    }
}
