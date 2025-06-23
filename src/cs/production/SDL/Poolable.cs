// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Represent a poolable object.
/// </summary>
/// <typeparam name="TSelf">The type of it self.</typeparam>
[PublicAPI]
public abstract class Poolable<TSelf> : IPoolable<TSelf>, IDisposable
    where TSelf : class, IPoolable<TSelf>
{
    private bool _isDisposed;

    /// <inheritdoc />
    Pool<TSelf> IPoolable<TSelf>.Pool => Pool;

    /// <summary>
    ///     Gets a value indicating whether the pooled object has been disposed of.
    /// </summary>
    protected bool IsDisposed { get; }

    /// <summary>
    ///     Gets the pool associated with the poolable object.
    /// </summary>
    protected Pool<TSelf> Pool { get; private set; } = null!;

    /// <summary>
    ///     Gets the method that attempts to return the poolable object to it's associated pool.
    /// </summary>
    protected PoolReturnDelegate<TSelf>? ReturnToPool { get; private set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Poolable{TSelf}" /> class.
    /// </summary>
    protected Poolable()
    {
        if (this is not TSelf)
        {
            var typeName = GetType().Name;
            throw new InvalidOperationException($"'{typeName}' is not a type of IPoolable<'{typeName}'>.");
        }
    }

    /// <inheritdoc />
#pragma warning disable CA1033
    void IPoolable<TSelf>.Initialize(
        Pool<TSelf> pool,
        PoolReturnDelegate<TSelf> poolReturn)
    {
        Pool = pool;
        ReturnToPool = poolReturn;
    }
#pragma warning restore CA1033

    /// <inheritdoc />
    void IPoolable<TSelf>.Reset()
    {
        Reset();
    }

    /// <summary>
    ///     Returns the poolable object back to the <see cref="Pool" /> if the <see cref="Pool" /> is not disposed.
    ///     Otherwise, disposes the poolable object if not already disposed. If the poolable object is already disposed,
    ///     does nothing.
    /// </summary>
#pragma warning disable CA1063
    public void Dispose()
#pragma warning restore CA1063
    {
        if (_isDisposed)
        {
            return;
        }

        if (TryToReturnPrivate())
        {
            return;
        }

        var isDisposed = Interlocked.CompareExchange(ref _isDisposed, true, false);
        if (isDisposed)
        {
            return;
        }

        Dispose(true);
        GC.SuppressFinalize(this);
    }

    internal bool TryToReturnToPool()
    {
        return TryToReturnPrivate();
    }

    /// <summary>
    ///     Performs tasks related to freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="isDisposing">
    ///     <c>true</c> if <see cref="Dispose()" /> was called explicitly. <c>false</c> if
    ///     <see cref="Dispose()" /> was called implicitly by the garbage collector finalizer.
    /// </param>
    protected virtual void Dispose(bool isDisposing)
    {
        Pool = null!;
        ReturnToPool = null;
    }

    /// <summary>
    ///     Resets the object. Called by the <see cref="Pool" /> when an existing object instance is needed.
    /// </summary>
    protected abstract void Reset();

    private bool TryToReturnPrivate()
    {
        var poolableInstance = this as TSelf;
        return ReturnToPool?.Invoke(poolableInstance!) ?? false;
    }
}
