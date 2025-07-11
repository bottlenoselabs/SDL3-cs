// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Reflection;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace bottlenoselabs.Interop;

// ReSharper disable once InconsistentNaming
public static partial class SDL_ttf
{
    private static bool _isInitialized;

    /// <summary>
    ///     Initializes SDL native interoperability.
    /// </summary>
    public static void Initialize()
    {
        var isInitialized = Interlocked.CompareExchange(ref _isInitialized, true, false);
        if (isInitialized)
        {
            return;
        }

        NativeLibrary.SetDllImportResolver(
            Assembly.GetExecutingAssembly(),
            static (libraryName, assembly, searchPath) =>
                SDL.ResolveNativeLibrary(libraryName, assembly, searchPath, "SDL3_ttf"));
    }
}
