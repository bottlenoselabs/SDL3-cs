// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Provides a mechanism for an object to be pooled.
/// </summary>
/// <typeparam name="TSelf">The type of object being pooled.</typeparam>
[PublicAPI]
public interface IPoolable<TSelf>
    where TSelf : class, IPoolable<TSelf>
{
    /// <summary>
    ///     Gets the pool associated with the object.
    /// </summary>
    Pool<TSelf> Pool { get; }

    /// <summary>
    ///     Initializes the poolable object. Called by the <see cref="Pool" /> when a new object instance is needed.
    /// </summary>
    /// <param name="pool">
    ///     The pool associated with the poolable object. It is important that implementations of
    ///     <see cref="IPoolable{TSelf}" /> set <see cref="Pool" /> to the value of <paramref name="pool" /> so that
    ///     pooled object instances can be correctly returned.
    /// </param>
    /// <param name="poolReturn">
    ///     The method that attempts to return the poolable object to it's associated pool.
    /// </param>
    void Initialize(Pool<TSelf> pool, PoolReturnDelegate<TSelf> poolReturn);

    /// <summary>
    ///     Resets the object. Called by the <see cref="Pool" /> when an existing object instance is needed.
    /// </summary>
    void Reset();
}
