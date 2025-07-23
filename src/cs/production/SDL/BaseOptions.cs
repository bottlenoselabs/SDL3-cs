// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.ComponentModel;

namespace bottlenoselabs.SDL;

/// <summary>
///     Parameters for creating an object.
/// </summary>
[PublicAPI]
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class BaseOptions : Disposable
{
    private INativeAllocator? _ownedAllocator;

    /// <summary>
    ///     Gets the allocator used for temporary interoperability allocations.
    /// </summary>
    protected internal INativeAllocator Allocator { get; private set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseOptions" /> class.
    /// </summary>
    /// <param name="allocator">
    ///     The allocator to use for temporary interoperability allocations. If <c>null</c>, a
    ///     <see cref="ArenaNativeAllocator" /> is used with a capacity of 1 KB.
    /// </param>
    protected BaseOptions(INativeAllocator? allocator = null)
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
    ///     Resets the parameters to default values.
    /// </summary>
    public void Reset()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        OnReset();
        Allocator.Reset();
    }

    /// <summary>
    ///     Called when the parameters should be reset to default values.
    /// </summary>
    protected abstract void OnReset();

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        Reset();
        var allocatorDisposable = _ownedAllocator as IDisposable;
        allocatorDisposable?.Dispose();
        _ownedAllocator = null;
    }
}
