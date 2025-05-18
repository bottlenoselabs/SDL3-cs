// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using SDL;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E008_GeometryRendering : ExampleLazyFoo
{
    public E008_GeometryRendering()
        : base("8 - Geometry Rendering", isEnabledCreateRenderer2D: true)
    {
    }

    public override bool Initialize(INativeAllocator allocator)
    {
        return true;
    }

    public override void Quit()
    {
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

        // Render red filled quad
        RectangleF fillRect = default;
        fillRect.X = ScreenWidth / 4.0f;
        fillRect.Y = ScreenHeight / 4.0f;
        fillRect.Width = ScreenWidth / 2.0f;
        fillRect.Height = ScreenHeight / 2.0f;
        renderer.DrawColor = Rgba8U.Red;
        renderer.RenderRectangleFill(fillRect);

        // Render green outlined quad
        RectangleF outlineRect = default;
        outlineRect.X = ScreenWidth / 6.0f;
        outlineRect.Y = ScreenHeight / 6.0f;
        outlineRect.Width = ScreenWidth * 2.0f / 3.0f;
        outlineRect.Height = ScreenHeight * 2.0f / 3.0f;
        renderer.DrawColor = Rgba8U.Lime;
        renderer.RenderRectangle(outlineRect);

        // Draw blue horizontal line
        renderer.DrawColor = Rgba8U.Blue;
        renderer.RenderLine(0, ScreenHeight / 2.0f, ScreenWidth, ScreenHeight / 2.0f);

        // Draw vertical line of yellow dots
        renderer.DrawColor = Rgba8U.Yellow;
        for (var i = 0; i < ScreenHeight; i += 4)
        {
            renderer.RenderPoint(ScreenWidth / 2.0f, i);
        }

        // Update screen
        renderer.Present();
    }
}
