// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E018_KeyStates : ExampleLazyFoo
{
    private Texture? _texturePress;
    private Texture? _textureDown;
    private Texture? _textureUp;
    private Texture? _textureLeft;
    private Texture? _textureRight;

    private Texture? _textureCurrent;

    public E018_KeyStates()
        : base("18 - Key States", isEnabledCreateRenderer2D: true)
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
        _texturePress?.Dispose();
        _texturePress = null;

        _textureDown?.Dispose();
        _textureDown = null;

        _textureUp?.Dispose();
        _textureUp = null;

        _textureLeft?.Dispose();
        _textureLeft = null;

        _textureRight?.Dispose();
        _textureRight = null;

        _textureCurrent = null;
    }

    public override void OnUpdate(TimeSpan deltaTime)
    {
        // NOTE: Not convinced that this will be useful for a safe API.
        unsafe
        {
            var keyboardState = SDL_GetKeyboardState(null);
            if (keyboardState[(int)SDL_Scancode.SDL_SCANCODE_DOWN])
            {
                _textureCurrent = _textureDown;
            }
            else if (keyboardState[(int)SDL_Scancode.SDL_SCANCODE_UP])
            {
                _textureCurrent = _textureUp;
            }
            else if (keyboardState[(int)SDL_Scancode.SDL_SCANCODE_LEFT])
            {
                _textureCurrent = _textureLeft;
            }
            else if (keyboardState[(int)SDL_Scancode.SDL_SCANCODE_RIGHT])
            {
                _textureCurrent = _textureRight;
            }
            else
            {
                _textureCurrent = _texturePress;
            }
        }
    }

    public override void OnDraw(TimeSpan deltaTime)
    {
        var renderer = Window.Renderer!;

        renderer.DrawColor = Rgba8U.CornflowerBlue;
        renderer.Clear();

        if (_textureCurrent != null)
        {
            renderer.RenderTexture(_textureCurrent);
        }

        renderer.Present();
    }

    private bool TryLoadAssets()
    {
        var assetsDirectory = Path.Combine(
            AppContext.BaseDirectory, "Examples", nameof(E018_KeyStates));

        if (!TryLoadTexture(assetsDirectory, "press.png", out _texturePress))
        {
            return false;
        }

        if (!TryLoadTexture(assetsDirectory, "down.png", out _textureDown))
        {
            return false;
        }

        if (!TryLoadTexture(assetsDirectory, "up.png", out _textureUp))
        {
            return false;
        }

        if (!TryLoadTexture(assetsDirectory, "left.png", out _textureLeft))
        {
            return false;
        }

        if (!TryLoadTexture(assetsDirectory, "right.png", out _textureRight))
        {
            return false;
        }

        return true;
    }

    private bool TryLoadTexture(
        string assetsDirectory,
        string fileName,
        out Texture? texture)
    {
        var filePath = Path.Combine(assetsDirectory, fileName);
        if (!FileSystem.TryLoadImage(filePath, out var imageSurface))
        {
            texture = null;
            return false;
        }

        texture = Window.Renderer!.CreateTextureFrom(imageSurface!);
        imageSurface!.Dispose();
        return true;
    }
}
