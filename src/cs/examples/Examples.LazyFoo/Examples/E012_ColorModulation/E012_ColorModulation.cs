// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E012_ColorModulation : ExampleLazyFoo
{
    private Texture? _texture;
    private Rgb8U _color = Rgb8U.White;

    public E012_ColorModulation()
        : base("12 - Color Modulation", isEnabledCreateRenderer2D: true)
    {
    }

    public override bool Initialize(INativeAllocator allocator)
    {
        return LoadAssets();
    }

    public override void Quit()
    {
        _texture?.Dispose();
        _texture = null;
    }

    public override void KeyboardEvent(in SDL_KeyboardEvent e)
    {
        var key = e.scancode;
        switch (key)
        {
            case SDL_Scancode.SDL_SCANCODE_Q:
                _color.R += 32;
                break;
            case SDL_Scancode.SDL_SCANCODE_W:
                _color.G += 32;
                break;
            case SDL_Scancode.SDL_SCANCODE_E:
                _color.B += 32;
                break;
            case SDL_Scancode.SDL_SCANCODE_A:
                _color.B -= 32;
                break;
            case SDL_Scancode.SDL_SCANCODE_S:
                _color.G -= 32;
                break;
            case SDL_Scancode.SDL_SCANCODE_D:
                _color.G -= 32;
                break;
        }
    }

    public override void Update(float deltaTime)
    {
    }

    public override void Draw(float deltaTime)
    {
        var renderer = Window.Renderer!;

        // Clear screen
        renderer.DrawColor = Rgba8U.White;
        renderer.Clear();

        _texture!.Color = _color;
        renderer.RenderTexture(_texture);

        // Update screen
        renderer.Present();
    }

    private bool LoadAssets()
    {
        var assetsDirectory = Path.Combine(
            AppContext.BaseDirectory, "Examples", nameof(E012_ColorModulation));

        if (!FileSystem.TryLoadImage(
                Path.Combine(assetsDirectory, "colors.png"), out var imageSurface))
        {
            return false;
        }

        _texture = Window.Renderer!.CreateTextureFrom(imageSurface!);
        imageSurface!.Dispose();

        return true;
    }
}
