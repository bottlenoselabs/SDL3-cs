// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E001_Hello : ExampleLazyFoo
{
    public E001_Hello()
        : base("1 - Hello", createRenderer: false)
    {
    }

    public override bool Initialize(INativeAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        _ = Window.TryGetSurface(out var surface);
        var color = Rgb8U.CornflowerBlue;
        var pixelColor = surface!.MapRgb(color);
        _ = surface.TryFill(pixelColor);
        _ = Window.TryUpdateFromSurface();

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
    }
}
