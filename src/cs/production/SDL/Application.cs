// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

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
    ///     Gets the <see cref="bottlenoselabs.SDL.FileSystem" /> of the application.
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
    ///     Called when the mouse moves.
    /// </summary>
    /// <param name="e">The event.</param>
    protected abstract void OnMouseMove(in MouseMoveEvent e);

    /// <summary>
    ///     Called when a mouse button is pressed down.
    /// </summary>
    /// <param name="e">The event.</param>
    protected abstract void OnMouseDown(in MouseButtonEvent e);

    /// <summary>
    ///     Called when a mouse button is released.
    /// </summary>
    /// <param name="e">The event.</param>
    protected abstract void OnMouseUp(in MouseButtonEvent e);

    /// <summary>
    ///     Called when a keyboard key is pressed down.
    /// </summary>
    /// <param name="e">The event.</param>
    protected abstract void OnKeyDown(in KeyboardEvent e);

    /// <summary>
    ///     Called when a keyboard key is released.
    /// </summary>
    /// <param name="e">The event.</param>
    protected abstract void OnKeyUp(in KeyboardEvent e);

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
            HandleEvent(e);
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

            case SDL_EventType.SDL_EVENT_MOUSE_MOTION:
            {
                HandleMouseMotionEvent(e.motion);
                break;
            }

            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_UP:
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_DOWN:
            {
                HandleMouseButtonEvent(e.button);
                break;
            }

            case SDL_EventType.SDL_EVENT_KEY_DOWN:
            case SDL_EventType.SDL_EVENT_KEY_UP:
            {
                HandleKeyboardEvent(e.key);
                break;
            }
        }
    }

    private void HandleMouseMotionEvent(in SDL_MouseMotionEvent mouseMotionEvent)
    {
        if (!WindowsById.TryGetValue(mouseMotionEvent.windowID, out var window))
        {
            // NOTE: Window must be created outside of managed C# code.
            window = null!;
        }

        var position = new Vector2(mouseMotionEvent.x, mouseMotionEvent.y);
        var e = new MouseMoveEvent(window, position);

        OnMouseMove(e);
    }

    private void HandleMouseButtonEvent(in SDL_MouseButtonEvent mouseButtonEvent)
    {
        var isDown = (bool)mouseButtonEvent.down;

        if (!WindowsById.TryGetValue(mouseButtonEvent.windowID, out var window))
        {
            // NOTE: Window must be created outside of managed C# code.
            window = null!;
        }

        var mouseButton = MouseButton.Unknown;
        if (mouseButtonEvent.button == SDL_BUTTON_LEFT)
        {
            mouseButton = MouseButton.Left;
        }
        else if (mouseButtonEvent.button == SDL_BUTTON_MIDDLE)
        {
            mouseButton = MouseButton.Middle;
        }
        else if (mouseButtonEvent.button == SDL_BUTTON_RIGHT)
        {
            mouseButton = MouseButton.Right;
        }
        else if (mouseButtonEvent.button == SDL_BUTTON_X1)
        {
            mouseButton = MouseButton.X1;
        }
        else if (mouseButtonEvent.button == SDL_BUTTON_X2)
        {
            mouseButton = MouseButton.X2;
        }

        var e = new MouseButtonEvent(
            window,
            mouseButton,
            isDown,
            mouseButtonEvent.clicks,
            new Vector2(mouseButtonEvent.x, mouseButtonEvent.y));

        if (isDown)
        {
            OnMouseDown(e);
        }
        else
        {
            OnMouseUp(e);
        }
    }

    private void HandleKeyboardEvent(SDL_KeyboardEvent keyboardEvent)
    {
        var isDown = (bool)keyboardEvent.down;
        var isRepeat = (bool)keyboardEvent.repeat;

        if (!WindowsById.TryGetValue(keyboardEvent.windowID, out var window))
        {
            // NOTE: Window must be created outside of managed C# code.
            window = null!;
        }

        // NOTE: Scancode Key is for keyboard layout independent buttons. Virtual keys are for layout dependent buttons.
        var key = keyboardEvent.scancode switch
        {
            SDL_Scancode.SDL_SCANCODE_LEFT => KeyboardButton.Left,
            SDL_Scancode.SDL_SCANCODE_RIGHT => KeyboardButton.Right,
            SDL_Scancode.SDL_SCANCODE_UP => KeyboardButton.Up,
            SDL_Scancode.SDL_SCANCODE_DOWN => KeyboardButton.Down,
            SDL_Scancode.SDL_SCANCODE_A => KeyboardButton.A,
            SDL_Scancode.SDL_SCANCODE_B => KeyboardButton.B,
            SDL_Scancode.SDL_SCANCODE_C => KeyboardButton.C,
            SDL_Scancode.SDL_SCANCODE_D => KeyboardButton.D,
            SDL_Scancode.SDL_SCANCODE_E => KeyboardButton.E,
            SDL_Scancode.SDL_SCANCODE_F => KeyboardButton.F,
            SDL_Scancode.SDL_SCANCODE_G => KeyboardButton.G,
            SDL_Scancode.SDL_SCANCODE_H => KeyboardButton.H,
            SDL_Scancode.SDL_SCANCODE_I => KeyboardButton.I,
            SDL_Scancode.SDL_SCANCODE_J => KeyboardButton.J,
            SDL_Scancode.SDL_SCANCODE_K => KeyboardButton.K,
            SDL_Scancode.SDL_SCANCODE_L => KeyboardButton.L,
            SDL_Scancode.SDL_SCANCODE_M => KeyboardButton.M,
            SDL_Scancode.SDL_SCANCODE_N => KeyboardButton.N,
            SDL_Scancode.SDL_SCANCODE_O => KeyboardButton.O,
            SDL_Scancode.SDL_SCANCODE_P => KeyboardButton.P,
            SDL_Scancode.SDL_SCANCODE_Q => KeyboardButton.Q,
            SDL_Scancode.SDL_SCANCODE_R => KeyboardButton.R,
            SDL_Scancode.SDL_SCANCODE_S => KeyboardButton.S,
            SDL_Scancode.SDL_SCANCODE_T => KeyboardButton.T,
            SDL_Scancode.SDL_SCANCODE_U => KeyboardButton.U,
            SDL_Scancode.SDL_SCANCODE_V => KeyboardButton.V,
            SDL_Scancode.SDL_SCANCODE_W => KeyboardButton.W,
            SDL_Scancode.SDL_SCANCODE_X => KeyboardButton.X,
            SDL_Scancode.SDL_SCANCODE_Y => KeyboardButton.Y,
            SDL_Scancode.SDL_SCANCODE_Z => KeyboardButton.Z,
            SDL_Scancode.SDL_SCANCODE_1 => KeyboardButton.Number1,
            SDL_Scancode.SDL_SCANCODE_2 => KeyboardButton.Number2,
            SDL_Scancode.SDL_SCANCODE_3 => KeyboardButton.Number3,
            SDL_Scancode.SDL_SCANCODE_4 => KeyboardButton.Number4,
            SDL_Scancode.SDL_SCANCODE_5 => KeyboardButton.Number5,
            SDL_Scancode.SDL_SCANCODE_6 => KeyboardButton.Number6,
            SDL_Scancode.SDL_SCANCODE_7 => KeyboardButton.Number7,
            SDL_Scancode.SDL_SCANCODE_8 => KeyboardButton.Number8,
            SDL_Scancode.SDL_SCANCODE_9 => KeyboardButton.Number9,
            SDL_Scancode.SDL_SCANCODE_0 => KeyboardButton.Number0,
            _ => KeyboardButton.Unknown
        };

        var e = new KeyboardEvent(window, key, isDown, isRepeat);

        if (isDown)
        {
            OnKeyDown(e);
        }
        else
        {
            OnKeyUp(e);
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
