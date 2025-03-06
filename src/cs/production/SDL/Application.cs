// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Runtime.CompilerServices;

namespace SDL;

/// <summary>
///     Represents an application using SDL.
/// </summary>
[PublicAPI]
public abstract unsafe class Application : Disposable
{
    private GCHandle _gcHandle;
    private bool _isExiting;
    private volatile bool _isInBackground;

    /// <summary>
    ///     Raised when the application is exiting.
    /// </summary>
    public event EventHandler<EventArgs>? IsExiting;

    /// <summary>
    ///     Call this method to initialize the application, begin running the application loop, and start processing
    ///     events for the application.
    /// </summary>
    /// <exception cref="InvalidOperationException">Failed to initialize SDL.</exception>
    public void Run()
    {
        bottlenoselabs.Interop.SDL.Initialize();
        SDL_image.Initialize();

        if (!SDL_Init(SDL_INIT_VIDEO | SDL_INIT_GAMEPAD))
        {
            var errorMessage = Error.GetMessage();
            Console.Error.WriteLine("Failed to initialize SDL: {0}", errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _gcHandle = GCHandle.Alloc(this, GCHandleType.Normal);
        var userDataPointer = GCHandle.ToIntPtr(_gcHandle);
        SDL_AddEventWatch(new SDL_EventFilter(&OnEventWatch), (void*)userDataPointer);

        Initialize();
        Loop();
    }

    /// <summary>
    ///     Call this method to exit the application at the beginning of the next frame.
    /// </summary>
    public void Exit()
    {
        _isExiting = true;
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        Exit();
        _gcHandle.Free();
    }

    /// <summary>
    ///     Called when the application is starting.
    /// </summary>
    protected abstract void Initialize();

    /// <summary>
    ///     Called when the application determines it is time to handle an event. This is where your application would
    ///     handle input and other application events.
    /// </summary>
    /// <param name="e">The event.</param>
    protected abstract void Event(in SDL_Event e);

    /// <summary>
    ///     Called when the application determines it is time to update a frame. This is where your application would
    ///     update its state.
    /// </summary>
    /// <param name="deltaTime">The time in milliseconds passed since the last call to <see cref="Update" />.</param>
    protected abstract void Update(float deltaTime);

    /// <summary>
    ///     Called when the application determines it is time to draw a frame. This is where your application would
    ///     perform rendering.
    /// </summary>
    /// <param name="deltaTime">The time in milliseconds passed since the last call to <see cref="Draw" />.</param>
    protected abstract void Draw(float deltaTime);

    private void Loop()
    {
        var lastTime = 0.0f;

        while (!_isExiting)
        {
            SDL_Event e;
            if (SDL_PollEvent(&e))
            {
                OnEvent(e);
                Event(e);
            }

            var newTime = SDL_GetTicks() / 1000.0f;
            var deltaTime = newTime - lastTime;
            lastTime = newTime;

            Update(deltaTime);

            if (!_isInBackground)
            {
                Draw(deltaTime);
            }
        }

        OnIsExiting();
    }

    private void OnEvent(in SDL_Event e)
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

    private void OnIsExiting()
    {
        IsExiting?.Invoke(this, EventArgs.Empty);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static CBool OnEventWatch(void* userData, SDL_Event* e)
    {
        // NOTE: We may be on different thread than the main thread.
        // NOTE: The return value is ignored for SDL_AddEventWatch. Thus, we always return false.

        var gcHandle = GCHandle.FromIntPtr((IntPtr)userData);
        if (gcHandle.Target is not Application app)
        {
            return false;
        }

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
