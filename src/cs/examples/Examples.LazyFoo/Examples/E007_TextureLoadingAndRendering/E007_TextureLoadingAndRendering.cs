// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E007_TextureLoadingAndRendering : ExampleLazyFoo
{
    private Texture? _texture;

    public E007_TextureLoadingAndRendering()
        : base("7 - Texture Loading", isEnabledCreateRenderer2D: true)
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

    public override void OnUpdate(TimeSpan deltaTime)
    {
    }

    public override void OnDraw(TimeSpan deltaTime)
    {
        var renderer = Window.Renderer!;
        renderer.Clear();
        renderer.RenderTexture(_texture!);
        renderer.Present();
    }

    private bool TryLoadAssets()
    {
        var assetsDirectory = Path.Combine(
            AppContext.BaseDirectory, "Examples", nameof(E007_TextureLoadingAndRendering));

        if (!FileSystem.TryLoadImage(
                Path.Combine(assetsDirectory, "texture.png"), out var imageSurface))
        {
            return false;
        }

        _texture = Window.Renderer!.CreateTextureFrom(imageSurface!);
        imageSurface!.Dispose();

        return true;
    }
}
