// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Reflection;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace bottlenoselabs.Interop;

// ReSharper disable once InconsistentNaming
public static partial class SDL
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
                ResolveNativeLibrary(libraryName, assembly, searchPath, "SDL3"));
    }

    internal static IntPtr ResolveNativeLibrary(
        string libraryName,
        Assembly assembly,
        DllImportSearchPath? searchPath,
        string libraryNameAssemblyDirectory)
    {
        if (libraryName != libraryNameAssemblyDirectory)
        {
            return NativeLibrary.Load(libraryName, assembly, searchPath);
        }

        // NOTE: SDL and SDL extensions use rpath.
        //  The searchPath is DllImportSearchPath.SafeDirectories by default on Windows and DllImportSearchPath.LegacyBehavior on other operating systems.
        //  This can be a problem when current directory is different from the assembly directory (install directory).

        return NativeLibrary.Load(libraryName, assembly, DllImportSearchPath.AssemblyDirectory);
    }
}
