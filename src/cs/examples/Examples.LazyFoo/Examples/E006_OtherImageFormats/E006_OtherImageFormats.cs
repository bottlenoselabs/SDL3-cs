// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E006_OtherImageFormats : ExampleLazyFoo
{
    private SDL_Surface* _screenSurface;
    private SDL_Surface* _surface;

    public E006_OtherImageFormats()
        : base("6 - Other Image Formats", createRenderer: false)
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
        // Apply the image stretched
        SDL_Rect stretchRectangle;
        stretchRectangle.x = 0;
        stretchRectangle.y = 0;
        stretchRectangle.w = ScreenWidth;
        stretchRectangle.h = ScreenHeight;
        _ = SDL_BlitSurfaceScaled(
            _surface,
            null,
            _screenSurface,
            &stretchRectangle,
            SDL_ScaleMode.SDL_SCALEMODE_NEAREST);

        // flip back and front buffer
        var window = (SDL_Window*)Window.Handle;
        _ = SDL_UpdateWindowSurface(window);
    }

    private bool LoadAssets(INativeAllocator allocator)
    {
        _surface = LoadSurface(allocator, "loaded.png");
        return _surface != null;
    }

    private SDL_Surface* LoadSurface(INativeAllocator allocator, string fileName)
    {
        var filePath = Path.Combine(AssetsDirectory, fileName);
        var filePathC = allocator.AllocateCString(filePath);
        var surface = IMG_Load(filePathC);
        if (surface == null)
        {
            Console.Error.WriteLine("Failed to load image '{0}': {1}", filePath, SDL_GetError());
            return null;
        }

        return surface;
    }
}
