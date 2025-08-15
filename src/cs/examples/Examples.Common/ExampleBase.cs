// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

namespace Common;

public abstract class ExampleBase
{
    private bool _hasExited;

    public Application Application { get; }

    public FileSystem FileSystem => Application.FileSystem;

    public Window Window { get; private set;  }

    public string Name { get; protected init; } = string.Empty;

    public string AssetsDirectory { get; protected init; }

    public int ScreenWidth => Window.Width;

    public int ScreenHeight => Window.Height;

    protected ExampleBase(
        bool isEnabledCreateSurface = false,
        bool isEnabledCreateRenderer2D = false)
    {
        Application = Application.Current;
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

    public abstract bool OnStart();

    public abstract void OnExit();

    public virtual void OnMouseMove(in MouseMoveEvent e)
    {
    }

    public virtual void OnMouseDown(in MouseButtonEvent e)
    {
    }

    public virtual void OnMouseUp(in MouseButtonEvent e)
    {
    }

    public virtual void OnKeyDown(in KeyboardEvent e)
    {
    }

    public virtual void OnKeyUp(in KeyboardEvent e)
    {
    }

    public virtual void OnUpdate(TimeSpan deltaTime)
    {
    }

    public virtual void OnDraw(TimeSpan deltaTime)
    {
    }

    internal void Exit()
    {
        var hasExited = Interlocked.CompareExchange(ref _hasExited, true, false);
        if (hasExited)
        {
            return;
        }

        OnExit();
        Window.Dispose();
        Window = null!;
    }

    internal bool Start(ArenaNativeAllocator allocator)
    {
        Window.Title = Name;
        allocator.Reset();
        var isInitialized = OnStart();
        allocator.Reset();
        return isInitialized;
    }
}
