// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E011_ClipRenderingAndSpriteSheets : ExampleLazyFoo
{
    private Texture? _texture;
    private readonly RectangleF[] _spriteSourceRectangles = new RectangleF[4];

    public E011_ClipRenderingAndSpriteSheets()
        : base("11 - Clip Rendering and Sprite Sheets", isEnabledCreateRenderer2D: true)
    {
    }

    public override bool Initialize(INativeAllocator allocator)
    {
        return TryLoadAssets();
    }

    public override void Quit()
    {
        _texture?.Dispose();
        _texture = null;
    }

    public override void KeyboardEvent(in SDL_KeyboardEvent e)
    {
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

        // Render top left sprite
        var destinationRectangle = default(RectangleF);
        destinationRectangle.X = 0;
        destinationRectangle.Y = 0;
        destinationRectangle.Width = 100;
        destinationRectangle.Height = 100;
        renderer.RenderTexture(_texture!, _spriteSourceRectangles[0], destinationRectangle);

        // Render top right sprite
        destinationRectangle.X = ScreenWidth - _spriteSourceRectangles[1].Width;
        destinationRectangle.Y = 0;
        renderer.RenderTexture(_texture!, _spriteSourceRectangles[1], destinationRectangle);

        // Render bottom left sprite
        destinationRectangle.X = 0;
        destinationRectangle.Y = ScreenHeight - _spriteSourceRectangles[2].Height;
        renderer.RenderTexture(_texture!, _spriteSourceRectangles[2], destinationRectangle);

        // Render bottom right sprite
        destinationRectangle.X = ScreenWidth - _spriteSourceRectangles[3].Width;
        destinationRectangle.Y = ScreenHeight - _spriteSourceRectangles[3].Height;
        renderer.RenderTexture(_texture!, _spriteSourceRectangles[3], destinationRectangle);

        // Update screen
        renderer.Present();
    }

    private bool TryLoadAssets()
    {
        var assetsDirectory = Path.Combine(
            AppContext.BaseDirectory, "Examples", nameof(E011_ClipRenderingAndSpriteSheets));

        if (!FileSystem.TryLoadImage(
                Path.Combine(assetsDirectory, "dots.png"), out var imageSurface))
        {
            return false;
        }

        imageSurface!.ColorKey = Rgb8U.Aqua;

        _texture = Window.Renderer!.CreateTextureFrom(imageSurface!);
        imageSurface.Dispose();

        // Set top left sprite
        ref var topLeftSprite = ref _spriteSourceRectangles[0];
        topLeftSprite.X = 0;
        topLeftSprite.Y = 0;
        topLeftSprite.Width = 100;
        topLeftSprite.Height = 100;

        // Set top right sprite
        ref var topRightSprite = ref _spriteSourceRectangles[1];
        topRightSprite.X = 100;
        topRightSprite.Y = 0;
        topRightSprite.Width = 100;
        topRightSprite.Height = 100;

        // Set bottom left sprite
        ref var bottomLeftSprite = ref _spriteSourceRectangles[2];
        bottomLeftSprite.X = 0;
        bottomLeftSprite.Y = 100;
        bottomLeftSprite.Width = 100;
        bottomLeftSprite.Height = 100;

        // Set bottom right sprite
        ref var bottomRightSprite = ref _spriteSourceRectangles[3];
        bottomRightSprite.X = 100;
        bottomRightSprite.Y = 100;
        bottomRightSprite.Width = 100;
        bottomRightSprite.Height = 100;

        return true;
    }
}
