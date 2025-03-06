// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using Microsoft.Extensions.Logging;
using SDL;

namespace Common;

public abstract class ExampleBase
{
    protected readonly ILoggerFactory LoggerFactory;

    public Window Window { get; private set; }

    public FileSystem FileSystem { get; private set; }

    public string AssetsDirectory { get; set; }

    private bool _hasQuit;

    public string Name { get; set; } = string.Empty;

    public int ScreenWidth => Window.Width;

    public int ScreenHeight => Window.Height;

    protected ExampleBase(
        WindowOptions? windowOptions = null,
        LogLevel logLevel = LogLevel.Warning)
    {
        LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(logLevel);
        });

        AssetsDirectory = AppContext.BaseDirectory;
        Window = new Window(windowOptions);
        FileSystem = new FileSystem(new Logger<FileSystem>(LoggerFactory));
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
        FileSystem.Dispose();
        FileSystem = null!;
    }

    internal bool InitializeInternal(ArenaNativeAllocator allocator)
    {
        Window.TrySetTitle(Name);
        allocator.Reset();
        var isInitialized = Initialize(allocator);
        allocator.Reset();
        return isInitialized;
    }
}
