// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E001_Hello : ExampleLazyFoo
{
    public E001_Hello()
        : base("1 - Hello", isEnabledCreateSurface: true)
    {
    }

    public override bool OnStart()
    {
        var surface = Window.Surface!;
        var color = Rgb8U.CornflowerBlue;
        var pixelColor = surface.MapRgb(color);
        surface.Fill(pixelColor);
        Window.Present();

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
    }
}
