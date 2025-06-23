// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL.GPU;
using bottlenoselabs.SDL.IO;

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
    ///     Gets the current <see cref="Platform" />.
    /// </summary>
    public Platform Platform { get; internal set; }

    /// <summary>
    ///     Gets the <see cref="FileSystem" /> of the application.
    /// </summary>
    public FileSystem FileSystem { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Application" /> class.
    /// </summary>
    /// <param name="loggerFactory">
    ///     The optional logger factory. If <c>null</c>, a console logger factory is created with
    ///     minimum log level of <see cref="LogLevel.Warning" />.
    /// </param>
    protected Application(ILoggerFactory? loggerFactory = null)
    {
        InternalInitialize(this);

        _loggerFactory = loggerFactory ?? LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Warning);
        });

        Error.LoggerNativeFunction = _loggerFactory.CreateLogger("Interop");

        FileSystem = new FileSystem(_loggerFactory.CreateLogger<FileSystem>());
    }

    /// <summary>
    ///     Call this method to begin running the application loop and start processing events for the application.
    /// </summary>
    /// <exception cref="InvalidOperationException">Failed to initialize SDL.</exception>
    public void Run()
    {
        if (!SDL_Init(SDL_INIT_VIDEO | SDL_INIT_GAMEPAD))
        {
            Error.NativeFunctionFailed(nameof(SDL_Init));
            return;
        }

        SDL_AddEventWatch(new SDL_EventFilter(&OnEventWatch), null);

        OnStart();
        Loop();
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
    /// <param name="deltaTime">The time in milliseconds passed since the last call to <see cref="OnUpdate" />.</param>
    protected abstract void OnUpdate(float deltaTime);

    /// <summary>
    ///     Called when the application determines it is time to draw a frame. This is where your application would
    ///     perform rendering.
    /// </summary>
    /// <param name="deltaTime">The time in milliseconds passed since the last call to <see cref="OnDraw" />.</param>
    protected abstract void OnDraw(float deltaTime);

    private void Loop()
    {
        var lastTime = 0.0f;

        while (!_isExiting)
        {
            SDL_Event e;
            if (SDL_PollEvent(&e))
            {
                HandleEvent(e);
                OnEvent(e);
            }

            var newTime = SDL_GetTicks() / 1000.0f;
            var deltaTime = newTime - lastTime;
            lastTime = newTime;

            // TODO: Add mechanism to separate rendering timing from updates.
            OnUpdate(deltaTime);

            if (!_isInBackground)
            {
                OnDraw(deltaTime);
            }
        }

        OnExit();
    }

    private void HandleEvent(in SDL_Event e)
    {
        var eventType = (SDL_EventType)e.type;
        switch (eventType)
        {
            case SDL_EventType.SDL_EVENT_QUIT:
            {
                _isExiting = true;
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
