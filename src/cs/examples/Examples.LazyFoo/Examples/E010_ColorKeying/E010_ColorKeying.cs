// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E010_ColorKeying : ExampleLazyFoo
{
    private Texture? _textureFoo;
    private Texture? _textureBackground;

    public E010_ColorKeying()
        : base("10 - Color Keying", isEnabledCreateRenderer2D: true)
    {
    }

    public override bool OnStart()
    {
        return TryLoadAssets();
    }

    public override void OnExit()
    {
        _textureFoo?.Dispose();
        _textureFoo = null;

        _textureBackground?.Dispose();
        _textureBackground = null;
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

        // Clear
        renderer.DrawColor = Rgba8U.White;
        renderer.Clear();

        // Render background texture
        renderer.RenderTexture(_textureBackground!);

        // Render Foo'
        var fooRectangle = default(RectangleF);
        fooRectangle.X = 240;
        fooRectangle.Y = 190;
        fooRectangle.Width = _textureFoo!.Width;
        fooRectangle.Height = _textureFoo.Height;
        renderer.RenderTexture(_textureFoo, null, fooRectangle);

        // Update screen
        renderer.Present();
    }

    private bool TryLoadAssets()
    {
        var assetsDirectory = Path.Combine(
            AppContext.BaseDirectory, "Examples", nameof(E010_ColorKeying));

        if (!FileSystem.TryLoadImage(
                Path.Combine(assetsDirectory, "foo.png"), out var imageSurfaceFoo))
        {
            return false;
        }

        imageSurfaceFoo!.ColorKey = Rgb8U.Aqua;

        _textureFoo = Window.Renderer!.CreateTextureFrom(imageSurfaceFoo!);
        imageSurfaceFoo.Dispose();

        if (!FileSystem.TryLoadImage(
                Path.Combine(assetsDirectory, "background.png"), out var imageSurfaceBackground))
        {
            return false;
        }

        _textureBackground = Window.Renderer!.CreateTextureFrom(imageSurfaceBackground!);
        imageSurfaceBackground!.Dispose();

        return true;
    }
}
