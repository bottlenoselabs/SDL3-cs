// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;
using bottlenoselabs.SDL.IO;

namespace Common;

public abstract class ExampleBase
{
    public Application Application { get; private set; }

    public FileSystem FileSystem { get; private set; }

    public Window Window { get; private set; }

    public string AssetsDirectory { get; set; }

    private bool _hasQuit;

    public string Name { get; set; } = string.Empty;

    public int ScreenWidth => Window.Width;

    public int ScreenHeight => Window.Height;

    protected ExampleBase(
        bool isEnabledCreateSurface = false,
        bool isEnabledCreateRenderer2D = false)
    {
        Application = Application.Current;
        FileSystem = Application.FileSystem;
        AssetsDirectory = AppContext.BaseDirectory;

        using var windowOptions = new WindowOptions();
        windowOptions.Title = Name;
        windowOptions.Width = 640;
        windowOptions.Height = 480;
        windowOptions.IsResizable = true;
        windowOptions.IsEnabledCreateSurface = isEnabledCreateSurface;
        windowOptions.IsEnabledCreateRenderer = isEnabledCreateRenderer2D;
        Window = Application.CreateWindow(windowOptions);
    }

    public abstract bool Initialize(INativeAllocator allocator);

    public abstract void Quit();

    public abstract void KeyboardEvent(in SDL_KeyboardEvent e);

    public abstract void Update(float deltaTime);

    public abstract void Draw(float deltaTime);

    internal void QuitInternal()
    {
        var hasAlreadyQuit = Interlocked.CompareExchange(ref _hasQuit, true, false);
        if (hasAlreadyQuit)
        {
            return;
        }

        Quit();
        Window.Dispose();
        Window = null!;

        FileSystem = null!;
    }

    internal bool InitializeInternal(ArenaNativeAllocator allocator)
    {
        Window.Title = Name;
        allocator.Reset();
        var isInitialized = Initialize(allocator);
        allocator.Reset();
        return isInitialized;
    }
}
