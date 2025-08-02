// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E014_AnimatedSpritesAndVSync : ExampleLazyFoo
{
    private readonly RectangleF[] _spriteSourceRectangles = [
        new() { X = 0, Y = 0, Width = 64, Height = 205},
        new() { X = 64, Y = 0, Width = 64, Height = 205},
        new() { X = 128, Y = 0, Width = 64, Height = 205},
        new() { X = 192, Y = 0, Width = 64, Height = 205}
    ];

    private Texture? _texture;
    private int _currentSpriteSourceRectangleIndex;
    private TimeSpan _animationTimer;

    public E014_AnimatedSpritesAndVSync()
        : base("14 - Animated Sprites and VSync", isEnabledCreateRenderer2D: true)
    {
    }

    public override bool OnStart()
    {
        var renderer = Window.Renderer!;
        renderer.TrySetVSyncMode(VSyncMode.EnabledEveryRefresh);

        return TryLoadAssets();
    }

    public override void OnExit()
    {
        _texture?.Dispose();
        _texture = null;
    }

    public override void OnUpdate(TimeSpan deltaTime)
    {
        _animationTimer += deltaTime;
        if (_animationTimer > TimeSpan.FromMilliseconds(100))
        {
            _animationTimer = TimeSpan.Zero;
            _currentSpriteSourceRectangleIndex = (_currentSpriteSourceRectangleIndex + 1) % 4;
        }
    }

    public override void OnDraw(TimeSpan deltaTime)
    {
        var renderer = Window.Renderer!;

        // Clear screen
        renderer.DrawColor = Rgba8U.CornflowerBlue;
        renderer.Clear();

        if (_texture != null)
        {
            // Render animated sprite
            var spriteSourceRectangle = _spriteSourceRectangles[_currentSpriteSourceRectangleIndex];
            RectangleF destinationRectangle;
            destinationRectangle.X = (renderer.Viewport.Width - spriteSourceRectangle.Width) / 2f;
            destinationRectangle.Y = (renderer.Viewport.Height - spriteSourceRectangle.Height) / 2f;
            destinationRectangle.Width = spriteSourceRectangle.Width;
            destinationRectangle.Height = spriteSourceRectangle.Height;
            renderer.RenderTexture(_texture, spriteSourceRectangle, destinationRectangle);
        }

        // Update screen
        renderer.Present();
    }

    private bool TryLoadAssets()
    {
        var assetsDirectory = Path.Combine(
            AppContext.BaseDirectory, "Examples", nameof(E014_AnimatedSpritesAndVSync));

        if (!FileSystem.TryLoadImage(
                Path.Combine(assetsDirectory, "foo.png"), out var imageSurface))
        {
            return false;
        }

        imageSurface!.ColorKey = Rgb8U.Aqua;

        _texture = Window.Renderer!.CreateTextureFrom(imageSurface);
        imageSurface.Dispose();

        return true;
    }
}
