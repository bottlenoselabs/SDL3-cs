// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E009_Viewport : ExampleLazyFoo
{
    private Texture? _texture;

    public E009_Viewport()
        : base("9 - Viewport", isEnabledCreateRenderer2D: true)
    {
    }

    public override bool OnStart()
    {
        return TryLoadAssets();
    }

    public override void OnExit()
    {
        _texture?.Dispose();
        _texture = null;
    }

    public override void OnKeyboardEvent(in SDL_KeyboardEvent e)
    {
    }

    public override void OnUpdate(float deltaTime)
    {
    }

    public override void OnDraw(float deltaTime)
    {
        var renderer = Window.Renderer!;

        // Clear screen
        renderer.DrawColor = Rgba8U.White;
        renderer.Clear();

        // Top left corner viewport
        Rectangle topLeftViewport;
        topLeftViewport.X = 0;
        topLeftViewport.Y = 0;
        topLeftViewport.Width = ScreenWidth / 2;
        topLeftViewport.Height = ScreenHeight / 2;
        renderer.Viewport = topLeftViewport;
        // Render texture to screen
        renderer.RenderTexture(_texture!);

        // Top right viewport
        Rectangle topRightViewport;
        topRightViewport.X = ScreenWidth / 2;
        topRightViewport.Y = 0;
        topRightViewport.Width = ScreenWidth / 2;
        topRightViewport.Height = ScreenHeight / 2;
        renderer.Viewport = topRightViewport;
        // Render texture to screen
        renderer.RenderTexture(_texture!);

        // Bottom viewport
        Rectangle bottomViewport;
        bottomViewport.X = 0;
        bottomViewport.Y = ScreenHeight / 2;
        bottomViewport.Width = ScreenWidth;
        bottomViewport.Height = ScreenHeight / 2;
        renderer.Viewport = bottomViewport;
        // Render texture to screen
        renderer.RenderTexture(_texture!);

        // Update screen
        renderer.Present();
    }

    private bool TryLoadAssets()
    {
        var assetsDirectory = Path.Combine(
            AppContext.BaseDirectory, "Examples", nameof(E009_Viewport));

        if (!FileSystem.TryLoadImage(
                Path.Combine(assetsDirectory, "viewport.png"), out var imageSurface))
        {
            return false;
        }

        _texture = Window.Renderer!.CreateTextureFrom(imageSurface!);
        imageSurface!.Dispose();

        return true;
    }
}
