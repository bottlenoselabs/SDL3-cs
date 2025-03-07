// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E002_ImageOnScreen : ExampleLazyFoo
{
    private SDL_Surface* _surface;

    public E002_ImageOnScreen()
        : base("2 - Image on Screen", createRenderer: false)
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
        var screenSurface = SDL_GetWindowSurface(window);
        _ = SDL_BlitSurface(_surface, null, screenSurface, null);
        _ = SDL_UpdateWindowSurface(window);

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
    }

    private bool LoadAssets(INativeAllocator allocator)
    {
        var assetsDirectory = Path.Combine(AppContext.BaseDirectory, "Examples", nameof(E002_ImageOnScreen));

        var filePath = Path.Combine(assetsDirectory, "hello_world.bmp");
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
