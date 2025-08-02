// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E013_AlphaBlending : ExampleLazyFoo
{
    private Texture? _textureFadeOut;
    private Texture? _textureFadeIn;
    private byte _a = 255;

    public E013_AlphaBlending()
        : base("13 - Alpha Blending", isEnabledCreateRenderer2D: true)
    {
    }

    public override bool OnStart()
    {
        return TryLoadAssets();
    }

    public override void OnExit()
    {
        _textureFadeOut?.Dispose();
        _textureFadeOut = null;

        _textureFadeIn?.Dispose();
        _textureFadeIn = null;
    }

    public override void OnKeyDown(in KeyboardEvent e)
    {
        switch (e.Key)
        {
            case KeyboardButton.W:
            {
                if (_a + 32 > 255)
                {
                    _a = 255;
                }
                else
                {
                    _a += 32;
                }

                break;
            }

            case KeyboardButton.S:
            {
                if (_a - 32 < 0)
                {
                    _a = 0;
                }
                else
                {
                    _a -= 32;
                }

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

        renderer.RenderTexture(_textureFadeIn!);

        _textureFadeOut!.Alpha = _a;
        renderer.RenderTexture(_textureFadeOut);

        // Update screen
        renderer.Present();
    }

    private bool TryLoadAssets()
    {
        var assetsDirectory = Path.Combine(
            AppContext.BaseDirectory, "Examples", nameof(E013_AlphaBlending));

        if (!FileSystem.TryLoadImage(
                Path.Combine(assetsDirectory, "fadein.png"), out var imageSurfaceFadeIn))
        {
            return false;
        }

        _textureFadeIn = Window.Renderer!.CreateTextureFrom(imageSurfaceFadeIn!);
        imageSurfaceFadeIn!.Dispose();

        if (!FileSystem.TryLoadImage(
                Path.Combine(assetsDirectory, "fadeout.png"), out var imageSurfaceFadeOut))
        {
            return false;
        }

        _textureFadeOut = Window.Renderer!.CreateTextureFrom(imageSurfaceFadeOut!);
        imageSurfaceFadeOut!.Dispose();

        _textureFadeOut.BlendMode = BlendMode.Alpha;

        return true;
    }
}
