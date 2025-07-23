// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL.Disk;
using bottlenoselabs.SDL.GPU;
using bottlenoselabs.SDL.Input;

namespace bottlenoselabs.SDL;

/// <summary>
///     Represents an application using SDL.
/// </summary>
[PublicAPI]
public abstract unsafe partial class Application : Disposable
{
    private bool _isExiting;
    private volatile bool _isInBackground;
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    ///     Gets the logger of the application.
    /// </summary>
    public ILogger<Application> Logger { get; }

    /// <summary>
    ///     Gets the <see cref="bottlenoselabs.SDL.Platform" /> of the application.
    /// </summary>
    public Platform Platform { get; internal set; }

    /// <summary>
    ///     Gets the <see cref="Disk.FileSystem" /> of the application.
    /// </summary>
    public FileSystem FileSystem { get; }

    /// <summary>
    ///     Gets the current rendered frames per second (FPS).
    /// </summary>
    public int FramesPerSecond { get; private set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Application" /> class.
    /// </summary>
    /// <param name="loggerFactory">
    ///     The optional logger factory. If <c>null</c>, a console logger factory is created with
    ///     minimum log level of <see cref="LogLevel.Warning" />.
    /// </param>
    protected Application(ILoggerFactory? loggerFactory = null)
    {
        _loggerFactory = loggerFactory ?? LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Warning);
        });

        Logger = _loggerFactory.CreateLogger<Application>();

        Error.LoggerNativeFunction = _loggerFactory.CreateLogger("Interop");

        FileSystem = new FileSystem(_loggerFactory.CreateLogger<FileSystem>());
    }

    /// <summary>
    ///     Call this method to begin running the application loop and start processing events for the application.
    /// </summary>
    /// <exception cref="InvalidOperationException">Failed to initialize SDL.</exception>
    public void Run()
    {
        Initialize(this);

        if (!SDL_Init(SDL_INIT_VIDEO | SDL_INIT_GAMEPAD))
        {
            Error.NativeFunctionFailed(nameof(SDL_Init), isExceptionThrown: true);
            return;
        }

        if (!SDL_AddEventWatch(new SDL_EventFilter(&OnEventWatch), null))
        {
            Error.NativeFunctionFailed(nameof(SDL_AddEventWatch), isExceptionThrown: true);
            return;
        }

        if (!TTF_Init())
        {
            Error.NativeFunctionFailed(nameof(TTF_Init), isExceptionThrown: true);
        }

        OnStart();
        Loop();

        TTF_Quit();
    }

    /// <summary>
    ///     Call this method to exit the application at the beginning of the next frame.
    /// </summary>
    public void Exit()
    {
        _isExiting = true;
    }

    /// <summary>
    ///     Creates a new <see cref="Window" /> class instance.
    /// </summary>
    /// <param name="options">
    ///     The parameters used to create the window.
    /// </param>
    /// <returns>A new <see cref="Window" /> instance.</returns>
    /// <exception cref="InvalidOperationException">
    ///     <see cref="WindowOptions.IsEnabledCreateSurface" /> and
    ///     <see cref="WindowOptions.IsEnabledCreateRenderer" /> can not both be enabled at the same time.
    /// </exception>
    /// <exception cref="NativeFunctionFailedException">Native function failed.</exception>
    public Window CreateWindow(WindowOptions options)
    {
        return new Window(options);
    }

    /// <summary>
    ///     Creates a new <see cref="GpuDevice" /> class instance.
    /// </summary>
    /// <param name="options">
    ///     The parameters to used to create the <see cref="GpuDevice" />. If <c>null</c>, sensible defaults are used.
    /// </param>
    /// <returns>A new <see cref="GpuDevice" /> instance.</returns>
    /// <exception cref="NativeFunctionFailedException">Native function failed.</exception>
    public GpuDevice CreateGpuDevice(GpuDeviceOptions? options = null)
    {
        var logger = _loggerFactory.CreateLogger<GpuDevice>();
        return new GpuDevice(logger, options);
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        Exit();
    }

    /// <summary>
    ///     Called when the application is starting.
    /// </summary>
    protected abstract void OnStart();

    /// <summary>
    ///     Called when the application is exiting.
    /// </summary>
    protected abstract void OnExit();

    /// <summary>
    ///     Called when the application determines it is time to handle an event. This is where your application would
    ///     handle input and other application events.
    /// </summary>
    /// <param name="e">The event.</param>
    protected abstract void OnEvent(in SDL_Event e);

    /// <summary>
    ///     Called when the application determines it is time to update a frame. This is where your application would
    ///     update its state.
    /// </summary>
    /// <param name="deltaTime">The time passed since the last call to <see cref="OnUpdate" />.</param>
    protected abstract void OnUpdate(TimeSpan deltaTime);

    /// <summary>
    ///     Called when the application determines it is time to draw a frame. This is where your application would
    ///     perform rendering.
    /// </summary>
    /// <param name="deltaTime">The time passed since the last call to <see cref="OnDraw" />.</param>
    protected abstract void OnDraw(TimeSpan deltaTime);

    private void Loop()
    {
        var previousTime = SDL_GetPerformanceCounter();
        var renderedFramesCount = 0;
        var framesCountAccumulatedTime = TimeSpan.Zero;

        while (!_isExiting)
        {
            PollEvents();
            var timeAdvanced = AdvanceTime(ref previousTime);
            OnUpdate(timeAdvanced);

            if (!_isInBackground)
            {
                OnDraw(timeAdvanced);
                renderedFramesCount++;
            }

            CalculateFramesPerSecond(timeAdvanced, ref framesCountAccumulatedTime, ref renderedFramesCount);
        }

        OnExit();
    }

    private void PollEvents()
    {
        SDL_Event e;
        while (SDL_PollEvent(&e))
        {
            ProcessEvent(e);
            OnEvent(e);
        }
    }

    private static TimeSpan AdvanceTime(ref ulong previousTime)
    {
        var currentTime = SDL_GetPerformanceCounter();
        var deltaTime = currentTime - previousTime;
        previousTime = currentTime;
        var deltaSeconds = (double)deltaTime / SDL_GetPerformanceFrequency();
        var deltaTicks = (long)(deltaSeconds * TimeSpan.TicksPerSecond);
        var timeAdvanced = new TimeSpan(deltaTicks);
        return timeAdvanced;
    }

    private void CalculateFramesPerSecond(
        TimeSpan timeAdvanced,
        ref TimeSpan framesCounterAccumulatedTime,
        ref int renderedFramesCount)
    {
        var framesCounterElapsedTime = TimeSpan.FromSeconds(1);

        framesCounterAccumulatedTime += timeAdvanced;
        if (framesCounterAccumulatedTime >= framesCounterElapsedTime)
        {
            framesCounterAccumulatedTime -= framesCounterElapsedTime;
            FramesPerSecond = renderedFramesCount;
            renderedFramesCount = 0;
        }
    }

    private void ProcessEvent(in SDL_Event e)
    {
        var eventType = (SDL_EventType)e.type;
        switch (eventType)
        {
            case SDL_EventType.SDL_EVENT_QUIT:
            {
                _isExiting = true;
                break;
            }

            case SDL_EventType.SDL_EVENT_WINDOW_OCCLUDED:
            {
                Interlocked.Exchange(ref _isInBackground, true);
                break;
            }

            case SDL_EventType.SDL_EVENT_WINDOW_EXPOSED:
            {
                Interlocked.Exchange(ref _isInBackground, false);
                break;
            }
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static CBool OnEventWatch(void* userData, SDL_Event* e)
    {
        // NOTE: We may be on different thread than the main thread.
        // NOTE: The return value is ignored for SDL_AddEventWatch. Thus, we always return false.

        var app = Current;

        var eventType = (SDL_EventType)e->type;
        switch (eventType)
        {
            case SDL_EventType.SDL_EVENT_DID_ENTER_BACKGROUND:
            {
                Interlocked.Exchange(ref app._isInBackground, true);
                break;
            }

            case SDL_EventType.SDL_EVENT_WILL_ENTER_FOREGROUND:
            {
                Interlocked.Exchange(ref app._isInBackground, false);
                break;
            }
        }

        return false;
    }
}
