// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Utility for dealing with SDL errors.
/// </summary>
[PublicAPI]
public static partial class Error
{
    internal static ILogger LoggerNativeFunction = null!;

    /// <summary>
    ///     Gets the last SDL error message.
    /// </summary>
    /// <returns>An error message.</returns>
    public static string GetMessage()
    {
        var errorMessageC = SDL_GetError();
        var errorMessage = CString.ToString(errorMessageC);
        return errorMessage;
    }

    internal static void NativeFunctionFailed(string functionName, bool isExceptionThrown = false)
    {
        var errorMessage = GetMessage();
        LogNativeFunctionFailed(LoggerNativeFunction, functionName, errorMessage);

        if (isExceptionThrown)
        {
            throw new NativeFunctionFailedException(functionName, $"Native function failed: '{functionName}'. Message: {errorMessage}");
        }
    }

    [LoggerMessage(LogEventId.NativeFunctionFailed, LogLevel.Error, "Native function failed: '{FunctionName}'. Message: {ErrorMessage}")]
    internal static partial void LogNativeFunctionFailed(ILogger logger, string functionName, string errorMessage);
}
