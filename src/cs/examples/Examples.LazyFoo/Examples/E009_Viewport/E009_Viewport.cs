// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E009_Viewport : ExampleLazyFoo
{
    private SDL_Texture* _texture;

    public E009_Viewport()
        : base("9 - Viewport")
    {
    }

    public override bool Initialize(INativeAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        return LoadAssets(allocator);
    }

    public override void Quit()
    {
        SDL_DestroyTexture(_texture);
        _texture = null;
    }

    public override void KeyboardEvent(in SDL_KeyboardEvent e)
    {
    }

    public override void Update(float deltaTime)
    {
    }

    public override void Draw(float deltaTime)
    {
        // Clear screen
        SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);
        SDL_RenderClear(Renderer);

        // Top left corner viewport
        SDL_Rect topLeftViewport;
        topLeftViewport.x = 0;
        topLeftViewport.y = 0;
        topLeftViewport.w = ScreenWidth / 2;
        topLeftViewport.h = ScreenHeight / 2;
        SDL_SetRenderViewport(Renderer, &topLeftViewport);
        // Render texture to screen
        SDL_RenderTexture(Renderer, _texture, null, null);

        // Top right viewport
        SDL_Rect topRightViewport;
        topRightViewport.x = ScreenWidth / 2;
        topRightViewport.y = 0;
        topRightViewport.w = ScreenWidth / 2;
        topRightViewport.h = ScreenHeight / 2;
        SDL_SetRenderViewport(Renderer, &topRightViewport);
        // Render texture to screen
        SDL_RenderTexture(Renderer, _texture, null, null);

        // Bottom viewport
        SDL_Rect bottomViewport;
        bottomViewport.x = 0;
        bottomViewport.y = ScreenHeight / 2;
        bottomViewport.w = ScreenWidth;
        bottomViewport.h = ScreenHeight / 2;
        SDL_SetRenderViewport(Renderer, &bottomViewport);
        // Render texture to screen
        SDL_RenderTexture(Renderer, _texture, null, null);

        // Update screen
        SDL_RenderPresent(Renderer);
    }

    private bool LoadAssets(INativeAllocator allocator)
    {
        _texture = LoadTexture(allocator, "viewport.png");
        return _texture != null;
    }

    private SDL_Texture* LoadTexture(INativeAllocator allocator, string fileName)
    {
        var filePath = Path.Combine(AssetsDirectory, fileName);
        var filePathC = allocator.AllocateCString(filePath);
        var surface = IMG_Load(filePathC);
        if (surface == null)
        {
            Console.Error.WriteLine("Failed to load image '{0}': {1}", filePath, SDL_GetError());
            return null;
        }

        var texture = SDL_CreateTextureFromSurface(Renderer, surface);
        SDL_DestroySurface(surface);
        if (texture == null)
        {
            Console.Error.WriteLine("Failed to create texture from file '{0}': {1}", filePath, SDL_GetError());
            return null;
        }

        return texture;
    }
}
