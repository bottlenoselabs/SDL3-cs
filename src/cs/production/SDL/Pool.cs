// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Concurrent;
using ObjectDisposedException = System.ObjectDisposedException;

namespace bottlenoselabs.SDL;

/// <summary>
///     Delegate that attempts to return an object instance back it's associated pool.
/// </summary>
/// <typeparam name="T">The type of poolable object.</typeparam>
/// <param name="obj">The object instance to return to the pool.</param>
/// <returns>
///     <c>true</c> if <paramref name="obj" /> was successfully returned to the pool. Otherwise,
///     <c>false</c>.
/// </returns>
public delegate bool PoolReturnDelegate<in T>(T obj);

/// <summary>
///     A thread-safe pool of object instances.
/// </summary>
/// <typeparam name="T">The type of object to pool.</typeparam>
[PublicAPI]
public partial class Pool<T> : Disposable
    where T : class, IPoolable<T>
{
    /// <summary>
    ///     The name of the type <typeparamref name="T" />.
    /// </summary>
    protected static readonly string TypeName = typeof(T).Name;

    /// <summary>
    ///     The name assigned to the pool for logging.
    /// </summary>
    protected readonly string? Name;

    /// <summary>
    ///     The logger associated with the pool.
    /// </summary>
    protected readonly ILogger Logger;

    private readonly PoolReturnDelegate<T> _poolReturnFunc;
    private readonly Func<T> _createFunc;
    private readonly BlockingCollection<T> _availableInstances;
    private readonly ConditionalWeakTable<T, Tracker> _trackers;
    private readonly bool _isEnabledTrackLeaks;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Pool{T}" /> class.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="createFunc">
    ///     The code to invoke when a new object instance is needed by the pool.
    /// </param>
    /// <param name="name">The name of the pool to use when logging.</param>
    /// <param name="initialCapacity">
    ///     The number of object instances initially available in the pool. Can not be less than <c>0</c>. Default value
    ///     is <c>0</c>.
    /// </param>
    /// <param name="isEnabledTrackLeaks">
    ///     If <c>true</c>, the pool will track objects instances that have been leased but have not been returned to
    ///     the pool before being garbage collected. If <c>false</c>, tracking is disabled. Default is <c>true</c>.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="logger" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialCapacity" /> is less than <c>0</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="createFunc" /> returned a <c>null</c> instance.</exception>
    public Pool(
        ILogger? logger,
        Func<T> createFunc,
        string? name,
        int initialCapacity = 0,
        bool isEnabledTrackLeaks = true)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentOutOfRangeException.ThrowIfLessThan(initialCapacity, 0);

        Name = string.IsNullOrEmpty(name) ? GetType().Name : name;
        Name = name;
        Logger = logger;
        _poolReturnFunc = TryReturn;
        _createFunc = createFunc;
        _trackers = new ConditionalWeakTable<T, Tracker>();
        _availableInstances = new BlockingCollection<T>(new ConcurrentQueue<T>());
        _isEnabledTrackLeaks = isEnabledTrackLeaks;

        for (var i = 0; i < initialCapacity; i++)
        {
            var newInstance = _createFunc();
            if (newInstance == null)
            {
                throw new InvalidOperationException("createFunc returned a null instance.");
            }

            newInstance.Initialize(this, _poolReturnFunc);
            _availableInstances.Add(newInstance);
        }
    }

    /// <summary>
    ///     Gets an object instance of type <typeparamref name="T" /> from the pool, blocking for the specified amount
    ///     of time if there are no available instances. If there is an available instance, returns that instance
    ///     immediately without blocking.
    /// </summary>
    /// <param name="timeSpan">
    ///     The amount of time to block before timing out if the pool has no available instances. Use
    ///     <see cref="Timeout.InfiniteTimeSpan" /> or <c>new TimeSpan(ticks: -1);</c> to block indefinitely.
    /// </param>
    /// <returns>
    ///     An object instance of type <typeparamref name="T" /> returned from pool. If timed out or the pool is
    ///     disposed, returns <c>null</c>.
    /// </returns>
    public T? GetOrWait(TimeSpan? timeSpan = null)
    {
        if (IsDisposed)
        {
            return null;
        }

        if (_availableInstances.TryTake(out var availableNonBlockingInstance))
        {
            OnLeasedCore(availableNonBlockingInstance);
            return availableNonBlockingInstance;
        }

        var timeSpan2 = timeSpan ?? new TimeSpan(-1);

        T? availableInstanceBlocking;
        try
        {
            if (!_availableInstances.TryTake(out availableInstanceBlocking, timeSpan2))
            {
                return null;
            }
        }
        catch (ObjectDisposedException)
        {
            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }

        OnLeasedCore(availableInstanceBlocking);
        return availableInstanceBlocking;
    }

    /// <summary>
    ///     Gets an object instance of type <typeparamref name="T" /> from the pool, creating a new instance if there
    ///     are no available instances.
    /// </summary>
    /// <returns>
    ///     The object instance of type <typeparamref name="T" /> returned from pool. If the pool is disposed, returns
    ///     <c>null</c>.
    /// </returns>
    /// <exception cref="InvalidOperationException">createFunc returned a null instance.</exception>
    public T? GetOrCreate()
    {
        if (IsDisposed)
        {
            return null;
        }

        try
        {
            if (_availableInstances.TryTake(out var availableNonBlockingInstance))
            {
                OnLeasedCore(availableNonBlockingInstance);
                return availableNonBlockingInstance;
            }
        }
        catch (ObjectDisposedException)
        {
            return null;
        }

        if (IsDisposed)
        {
            return null;
        }

        var newInstance = _createFunc();
        if (newInstance == null)
        {
            throw new InvalidOperationException("createFunc returned a null instance.");
        }

        LogPoolObjectCreated(Name, TypeName);
        newInstance.Initialize(this, _poolReturnFunc);
        OnLeasedCore(newInstance);

        return newInstance;
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        LogPoolDisposed(Name);

        // NOTE: By calling `CompleteAdding` here, any concurrent threads that call `TryAdd` or `Add` will throw, which is what we want!
        _availableInstances.CompleteAdding();

        // NOTE: Make sure we empty the pool upon disposal so that the garbage collector can make a more informed decision of what is going on.
        while (_availableInstances.TryTake(out var instance))
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (instance is IDisposable disposable)
            {
                disposable.Dispose();
            }

            OnDisposedCore(instance);
        }

        _availableInstances.Dispose();
    }

    /// <summary>
    ///     Called when an object instance is leased from the pool.
    /// </summary>
    /// <param name="instance">The object instance of type <typeparamref name="T" /> being leased from the pool.</param>
    protected virtual void OnLeased(T instance)
    {
    }

    /// <summary>
    ///     Called when an object instance is returned to the pool.
    /// </summary>
    /// <param name="instance">The object instance of type <typeparamref name="T" /> being returned to the pool.</param>
    protected virtual void OnReturned(T instance)
    {
    }

    /// <summary>
    ///     Called when an object instance is to be disposed by the pool.
    /// </summary>
    /// <param name="instance">The object instance of type <typeparamref name="T" /> being disposed by the pool.</param>
    protected virtual void OnDisposed(T instance)
    {
    }

    private void OnLeasedCore(T instance)
    {
        if (_isEnabledTrackLeaks)
        {
            if (_trackers.TryGetValue(instance, out var tracker))
            {
                tracker.UnDispose(Environment.StackTrace);
            }
            else
            {
#pragma warning disable CA2000
                _trackers.Add(instance, new Tracker(this, Environment.StackTrace));
#pragma warning restore CA2000
            }
        }

        LogPoolObjectLeased(Name, TypeName);
        OnLeased(instance);
    }

    private void OnReturnedCore(T instance)
    {
        if (_isEnabledTrackLeaks)
        {
            if (_trackers.TryGetValue(instance, out var tracker))
            {
                _trackers.Remove(instance);
                tracker.Dispose();
            }
        }

        LogPoolObjectReturned(Name, TypeName);
        OnReturned(instance);
    }

    private void OnDisposedCore(T instance)
    {
        if (_isEnabledTrackLeaks)
        {
            _trackers.Remove(instance);
        }

        OnDisposed(instance);
    }

    private bool TryReturn(T? instance)
    {
        if (IsDisposed || instance == null || instance.Pool != this)
        {
            return false;
        }

        instance.Reset();

        try
        {
            _availableInstances.Add(instance);
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        OnReturnedCore(instance);
        return true;
    }

    private sealed class Tracker : IDisposable
    {
        private string _stackTraceString;
        private bool _isDisposed;
        private readonly Pool<T> _pool;

        public Tracker(Pool<T> pool, string stackTraceString)
        {
            _pool = pool;
            _stackTraceString = stackTraceString;
        }

        public void Dispose()
        {
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }

        public void UnDispose(string stackTraceString)
        {
            _stackTraceString = stackTraceString;
            _isDisposed = false;
            GC.ReRegisterForFinalize(this);
        }

        ~Tracker()
        {
            var isLeaked = !_pool.IsDisposed && !_isDisposed && !Environment.HasShutdownStarted;
            if (isLeaked)
            {
                _pool.LogPoolObjectLeaked(_pool.Name, TypeName, _stackTraceString);
            }
        }
    }

    [LoggerMessage(LogEventId.PoolDisposed, LogLevel.Debug, "Pool '{Name}' disposed.")]
    private partial void LogPoolDisposed(string? name);

    [LoggerMessage(LogEventId.PoolObjectCreated, LogLevel.Debug, "'{TypeName}' instance created by pool '{Name}'.")]
    private partial void LogPoolObjectCreated(string? name, string typeName);

    [LoggerMessage(LogEventId.PoolObjectLeased, LogLevel.Debug, "'{TypeName}' instance leased from pool '{Name}'.")]
    private partial void LogPoolObjectLeased(string? name, string typeName);

    [LoggerMessage(LogEventId.PoolObjectReturned, LogLevel.Debug, "'{TypeName}' instance returned to pool '{Name}'.")]
    private partial void LogPoolObjectReturned(string? name, string typeName);

    [LoggerMessage(LogEventId.PoolObjectLeaked, LogLevel.Warning, "'{TypeName}' instance leaked from pool '{Name}'! Leased from:\n{StackTraceString}")]
    private partial void LogPoolObjectLeaked(string? name, string typeName, string stackTraceString);
}
