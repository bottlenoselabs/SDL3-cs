// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Represents the parameters for creating another object instance.
/// </summary>
[PublicAPI]
public abstract class Descriptor : Disposable
{
    private readonly INativeAllocator? _ownedAllocator;

    /// <summary>
    ///     Gets the allocator used for temporary interoperability allocations.
    /// </summary>
    protected internal INativeAllocator Allocator { get; private set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Descriptor" /> class.
    /// </summary>
    /// <param name="allocator">
    ///     The allocator to use for temporary interoperability allocations. If <c>null</c>, a
    ///     <see cref="ArenaNativeAllocator" /> is used with a capacity of 1 KB.
    /// </param>
    protected Descriptor(INativeAllocator? allocator = null)
    {
        if (allocator == null)
        {
            Allocator = new ArenaNativeAllocator(1024);
            _ownedAllocator = Allocator;
        }
        else
        {
            Allocator = allocator;
            _ownedAllocator = null;
        }

        Reset();
    }

    /// <summary>
    ///     Resets the descriptor to default values.
    /// </summary>
    public void Reset()
    {
        OnReset();
        var allocator = Allocator as ArenaNativeAllocator; // FIXME: Expand `INativeAllocator` to have `Reset` method.
        allocator?.Reset();
    }

    /// <summary>
    ///     Called when the descriptor should reset to default values.
    /// </summary>
    protected abstract void OnReset();

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        Reset();
        var allocatorDisposable = _ownedAllocator as IDisposable;
        allocatorDisposable?.Dispose();
    }
}
