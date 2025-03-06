// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL;

/// <summary>
///     Utility for dealing with SDL errors.
/// </summary>
public static class Error
{
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
}
