// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

#pragma warning disable CA1032

/// <summary>
///     The exception that is thrown when a native function fails.
/// </summary>
[PublicAPI]
public sealed class NativeFunctionFailedException : Exception
{
    /// <summary>
    ///     Gets the failed function name.
    /// </summary>
    public string FunctionName { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NativeFunctionFailedException" /> class.
    /// </summary>
    /// <param name="functionName">Failed function name.</param>
    /// <param name="message">The exception message.</param>
    public NativeFunctionFailedException(string functionName, string message)
        : base(message)
    {
        FunctionName = functionName;
    }
}
