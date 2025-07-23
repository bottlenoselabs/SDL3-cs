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

    public override bool OnStart()
    {
        return LoadAssets();
    }

    public override void OnExit()
    {
        _texture?.Dispose();
        _texture = null;
    }

    public override void OnKeyDown(in KeyboardEvent e)
    {
        switch (e.Key)
        {
            case KeyboardButton.Q:
            {
                _color.R += 32;
                break;
            }

            case KeyboardButton.W:
            {
                _color.G += 32;
                break;
            }

            case KeyboardButton.E:
            {
                _color.B += 32;
                break;
            }

            case KeyboardButton.A:
            {
                _color.B -= 32;
                break;
            }

            case KeyboardButton.S:
            {
                _color.G -= 32;
                break;
            }

            case KeyboardButton.D:
            {
                _color.G -= 32;
                break;
            }
        }
    }

    public override void OnUpdate(TimeSpan deltaTime)
    {
    }

    public override void OnDraw(TimeSpan deltaTime)
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
