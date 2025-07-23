// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E016_TrueTypeFonts : ExampleLazyFoo
{
    private Texture _texture = null!;

    public E016_TrueTypeFonts()
        : base("16 - True Type Fonts", isEnabledCreateRenderer2D: true)
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
    }

    public override void OnUpdate(TimeSpan deltaTime)
    {
    }

    public override void OnDraw(TimeSpan deltaTime)
    {
        var renderer = Window.Renderer!;

        // Clear screen
        renderer.DrawColor = Rgba8U.CornflowerBlue;
        renderer.Clear();

        // Render text
        RectangleF destinationRectangle;
        destinationRectangle.X = (renderer.Viewport.Width - _texture.Width) / 2f;
        destinationRectangle.Y = (renderer.Viewport.Height - _texture.Height) / 2f;
        destinationRectangle.Width = _texture.Width;
        destinationRectangle.Height = _texture.Height;
        renderer.RenderTexture(_texture, null, destinationRectangle);

        // Update screen
        renderer.Present();
    }

    private bool TryLoadAssets()
    {
        using var allocator = new ArenaNativeAllocator(1024);
        var assetsDirectory = Path.Combine(
            AppContext.BaseDirectory, "Examples", nameof(E016_TrueTypeFonts));

        if (!FileSystem.TryLoadFont(
                Path.Combine(assetsDirectory, "lazy.ttf"), out var font, 28))
        {
            return false;
        }

        var textColor = Rgba8U.White;
        if (!font!.TryRenderTextBlended(
                "The quick brown fox jumps over the lazy dog", textColor, allocator, out var surface))
        {
            return false;
        }

        _texture = Window.Renderer!.CreateTextureFrom(surface!);
        surface!.Dispose();

        return true;
    }
}
