// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL;

public partial class Application
{
    // ReSharper disable once InconsistentNaming
    internal static Application? _current;

    /// <summary>
    ///     Gets the current <see cref="Application" />.
    /// </summary>
    public static Application Current => _current!;

    internal static void InternalInitialize(Application application)
    {
        var app = Interlocked.CompareExchange(ref _current, application, null);
        if (app != null)
        {
            throw new InvalidOperationException("Only one SDL application can be instantiated.");
        }

        _current = application;

        bottlenoselabs.Interop.SDL.Initialize();
        application.Platform = GetNativePlatform();

        SDL_image.Initialize();
    }

    private static Platform GetNativePlatform()
    {
        var platformCString = SDL_GetPlatform();
        if (platformCString.IsNull)
        {
            throw new NativeFunctionFailedException(nameof(SDL_GetPlatform), "Platform is a null C string.");
        }

        var platformString = CString.ToString(platformCString);
        if (platformString.StartsWith("unknown", StringComparison.OrdinalIgnoreCase))
        {
            return Platform.Unknown;
        }

        if (platformString.Equals("windows", StringComparison.OrdinalIgnoreCase))
        {
            return Platform.Windows;
        }

        if (platformString.Equals("macos", StringComparison.OrdinalIgnoreCase))
        {
            return Platform.macOS;
        }

        if (platformString.Equals("linux", StringComparison.OrdinalIgnoreCase))
        {
            return Platform.Linux;
        }

        if (platformString.Equals("ios", StringComparison.OrdinalIgnoreCase))
        {
            return Platform.iOS;
        }

        if (platformString.Equals("android", StringComparison.OrdinalIgnoreCase))
        {
            return Platform.Android;
        }

        throw new NotImplementedException(
            $"The SDL platform '{platformString}' is not yet implemented for C#! Please submit a ticket on GitHub: https://github.com/bottlenoselabs/SDL3-cs.");
    }
}
