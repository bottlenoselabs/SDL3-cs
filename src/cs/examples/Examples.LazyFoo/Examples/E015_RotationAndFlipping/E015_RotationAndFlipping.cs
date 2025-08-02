// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E015_RotationAndFlipping : ExampleLazyFoo
{
    private float _rotationAngleDegrees;
    private FlipMode _flipMode;

    private Texture? _texture;
    private int _currentSpriteSourceRectangleIndex;

    public E015_RotationAndFlipping()
        : base("15 - Rotation and Flipping", isEnabledCreateRenderer2D: true)
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

    public override void OnKeyDown(in KeyboardEvent e)
    {
        switch (e.Key)
        {
            case KeyboardButton.A:
            {
                _rotationAngleDegrees -= 60;
                break;
            }

            case KeyboardButton.D:
            {
                _rotationAngleDegrees += 60;
                break;
            }

            case KeyboardButton.Q:
            {
                _flipMode = FlipMode.Horizontal;
                break;
            }

            case KeyboardButton.W:
            {
                _flipMode = FlipMode.None;
                break;
            }

            case KeyboardButton.E:
            {
                _flipMode = FlipMode.Vertical;
                break;
            }
        }
    }

    public override void OnUpdate(TimeSpan deltaTime)
    {
        _currentSpriteSourceRectangleIndex = (_currentSpriteSourceRectangleIndex + 1) % 4;
    }

    public override void OnDraw(TimeSpan deltaTime)
    {
        var renderer = Window.Renderer!;

        // Clear screen
        renderer.DrawColor = Rgba8U.CornflowerBlue;
        renderer.Clear();

        if (_texture != null)
        {
            // Render sprite
            RectangleF destinationRectangle;
            destinationRectangle.X = (renderer.Viewport.Width - _texture.Width) / 2f;
            destinationRectangle.Y = (renderer.Viewport.Height - _texture.Height) / 2f;
            destinationRectangle.Width = _texture.Width;
            destinationRectangle.Height = _texture.Height;
            renderer.RenderTextureRotated(
                _texture, null, destinationRectangle, _rotationAngleDegrees, null, _flipMode);
        }

        // Update screen
        renderer.Present();
    }

    private bool TryLoadAssets()
    {
        var assetsDirectory = Path.Combine(
            AppContext.BaseDirectory, "Examples", nameof(E015_RotationAndFlipping));

        if (!FileSystem.TryLoadImage(
                Path.Combine(assetsDirectory, "arrow.png"), out var imageSurface))
        {
            return false;
        }

        imageSurface!.ColorKey = Rgb8U.Black;

        _texture = Window.Renderer!.CreateTextureFrom(imageSurface);
        imageSurface.Dispose();

        return true;
    }
}
