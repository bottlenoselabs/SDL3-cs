// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E017_MouseEvents : ExampleLazyFoo
{
    private Texture? _textureSpriteSheet;

    private readonly RectangleF[] _spriteSourceRectangles = new RectangleF[4];

    private record struct Sprite
    {
        public RectangleF Rectangle;
        public SpriteSheetIndex SpriteSheetIndex;
    }

    private readonly Sprite[] _sprites;

    public enum SpriteSheetIndex
    {
        MouseOut = 0,
        MouseMotion = 1,
        MouseDown = 2,
        MouseUp = 3
    }

    public E017_MouseEvents()
        : base("17 - Mouse Events", isEnabledCreateRenderer2D: true)
    {
        var screenWidth = Window.Size.Width;
        var screenHeight = Window.Size.Height;
        var screenHalfWidth = screenWidth * 0.5f;
        var screenHalfHeight = screenHeight * 0.5f;

        _sprites =
        [
            new Sprite
            {
                Rectangle = new RectangleF { X = 0, Y = 0, Width = screenHalfWidth, Height = screenHalfHeight }
            },
            new Sprite
            {
                Rectangle = new RectangleF { X = screenHalfWidth, Y = 0, Width = screenHalfWidth, Height = screenHalfHeight }
            },
            new Sprite
            {
                Rectangle = new RectangleF { X = 0, Y = screenHalfHeight, Width = screenHalfWidth, Height = screenHalfHeight }
            },
            new Sprite
            {
                Rectangle = new RectangleF { X = screenHalfWidth, Y = screenHalfHeight, Width = screenHalfWidth, Height = screenHalfHeight }
            }
        ];
    }

    public override bool OnStart()
    {
        var renderer = Window.Renderer!;
        renderer.TrySetVSyncMode(VSyncMode.EnabledEveryRefresh);

        return TryLoadAssets();
    }

    public override void OnExit()
    {
        _textureSpriteSheet?.Dispose();
        _textureSpriteSheet = null;
    }

    public override void OnMouseMove(in MouseMoveEvent e)
    {
        for (var i = 0; i < _sprites.Length; i++)
        {
            ref var sprite = ref _sprites[i];

            if (sprite.Rectangle.Contains(e.Position))
            {
                sprite.SpriteSheetIndex = SpriteSheetIndex.MouseMotion;
            }
            else
            {
                sprite.SpriteSheetIndex = SpriteSheetIndex.MouseOut;
            }
        }
    }

    public override void OnMouseDown(in MouseButtonEvent e)
    {
        for (var i = 0; i < _sprites.Length; i++)
        {
            ref var sprite = ref _sprites[i];

            if (sprite.Rectangle.Contains(e.Position))
            {
                sprite.SpriteSheetIndex = SpriteSheetIndex.MouseDown;
            }
            else
            {
                sprite.SpriteSheetIndex = SpriteSheetIndex.MouseOut;
            }
        }
    }

    public override void OnMouseUp(in MouseButtonEvent e)
    {
        for (var i = 0; i < _sprites.Length; i++)
        {
            ref var sprite = ref _sprites[i];

            if (sprite.Rectangle.Contains(e.Position))
            {
                sprite.SpriteSheetIndex = SpriteSheetIndex.MouseUp;
            }
            else
            {
                sprite.SpriteSheetIndex = SpriteSheetIndex.MouseOut;
            }
        }
    }

    public override void OnUpdate(TimeSpan deltaTime)
    {
    }

    public override void OnDraw(TimeSpan deltaTime)
    {
        var renderer = Window.Renderer!;

        renderer.DrawColor = Rgba8U.CornflowerBlue;
        renderer.Clear();

        if (_textureSpriteSheet != null)
        {
            for (var i = 0; i < _sprites.Length; i++)
            {
                ref var sprite = ref _sprites[i];

                var sourceRectangle = _spriteSourceRectangles[(int)sprite.SpriteSheetIndex];
                var destinationRectangle = sprite.Rectangle;
                renderer.RenderTexture(_textureSpriteSheet, sourceRectangle, destinationRectangle);
            }
        }

        renderer.Present();
    }

    private bool TryLoadAssets()
    {
        var assetsDirectory = Path.Combine(
            AppContext.BaseDirectory, "Examples", nameof(E017_MouseEvents));

        if (!FileSystem.TryLoadImage(
                Path.Combine(assetsDirectory, "button.png"), out var imageSurface))
        {
            return false;
        }

        _textureSpriteSheet = Window.Renderer!.CreateTextureFrom(imageSurface!);
        imageSurface!.Dispose();

        for (var i = 0; i < _spriteSourceRectangles.Length; i++)
        {
            _spriteSourceRectangles[i] = new RectangleF
            {
                X = 0,
                Y = i * 200,
                Width = _textureSpriteSheet.Width,
                Height = 200
            };
        }

        return true;
    }
}
