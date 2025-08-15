// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

#pragma warning disable CA1815

/// <summary>
///     Parameters for creating an <see cref="Application" />.
/// </summary>
[PublicAPI]
public sealed class ApplicationOptions : BaseOptions
{
    /// <summary>
    ///     Gets or sets the <see cref="ILoggerFactory" /> instance used for creating loggers. If <c>null</c>, a console
    ///     logger factory is used with minimum log level of <see cref="LogLevel.Warning" />.
    /// </summary>
    public ILoggerFactory? LoggerFactory { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApplicationOptions" /> class.
    /// </summary>
    /// <param name="allocator">
    ///     The allocator to use for temporary interoperability allocations. If <c>null</c>, a
    ///     <see cref="ArenaNativeAllocator" /> is used with a capacity of 1 KB.
    /// </param>
    public ApplicationOptions(INativeAllocator? allocator = null)
        : base(allocator)
    {
        LoggerFactory ??= Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Warning);
        });
    }

    /// <inheritdoc />
    protected override void OnReset()
    {
    }
}
