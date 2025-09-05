// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

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

    public override bool OnStart()
    {
        return true;
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
        renderer.DrawColor = Rgba8U.White;
        renderer.Clear();

        // Render red filled quad
        RectangleF fillRect = default;
        fillRect.X = Window.Size.Width / 4.0f;
        fillRect.Y = Window.Size.Height / 4.0f;
        fillRect.Width = Window.Size.Width / 2.0f;
        fillRect.Height = Window.Size.Height / 2.0f;
        renderer.DrawColor = Rgba8U.Red;
        renderer.RenderRectangleFill(fillRect);

        // Render green outlined quad
        RectangleF outlineRect = default;
        outlineRect.X = Window.Size.Width / 6.0f;
        outlineRect.Y = Window.Size.Height / 6.0f;
        outlineRect.Width = Window.Size.Width * 2.0f / 3.0f;
        outlineRect.Height = Window.Size.Height * 2.0f / 3.0f;
        renderer.DrawColor = Rgba8U.Lime;
        renderer.RenderRectangle(outlineRect);

        // Draw blue horizontal line
        renderer.DrawColor = Rgba8U.Blue;
        renderer.RenderLine(0, Window.Size.Height / 2.0f, Window.Size.Width, Window.Size.Height / 2.0f);

        // Draw vertical line of yellow dots
        renderer.DrawColor = Rgba8U.Yellow;
        for (var i = 0; i < Window.Size.Height; i += 4)
        {
            renderer.RenderPoint(Window.Size.Width / 2.0f, i);
        }

        // Update screen
        renderer.Present();
    }
}
