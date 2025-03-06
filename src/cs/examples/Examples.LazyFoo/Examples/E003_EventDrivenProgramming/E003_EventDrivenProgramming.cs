// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E003_EventDrivenProgramming : ExampleLazyFoo
{
    private SDL_Surface* _screenSurface;
    private SDL_Surface* _surface;

    public E003_EventDrivenProgramming()
        : base("3 - Event Driven Programming", createRenderer: false)
    {
    }

    public override bool Initialize(INativeAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        if (!LoadAssets(allocator))
        {
            return false;
        }

        var window = (SDL_Window*)Window.Handle;
        _screenSurface = SDL_GetWindowSurface(window);
        return true;
    }

    public override void Quit()
    {
        SDL_DestroySurface(_surface);
        _surface = null;
    }

    public override void KeyboardEvent(in SDL_KeyboardEvent e)
    {
    }

    public override void Update(float deltaTime)
    {
    }

    public override void Draw(float deltaTime)
    {
        var window = (SDL_Window*)Window.Handle;

        _ = SDL_BlitSurface(_surface, null, _screenSurface, null);
        _ = SDL_UpdateWindowSurface(window); // flip back and front buffer
    }

    private bool LoadAssets(INativeAllocator allocator)
    {
        var assetsDirectory = Path.Combine(
            AppContext.BaseDirectory, "Examples", nameof(E003_EventDrivenProgramming));

        var filePath = Path.Combine(assetsDirectory, "x.bmp");
        var filePathC = allocator.AllocateCString(filePath);
        _surface = SDL_LoadBMP(filePathC);
        if (_surface == null)
        {
            Console.Error.WriteLine("Failed to load image '{0}': {1}", filePath, SDL_GetError());
            return false;
        }

        return true;
    }
}
