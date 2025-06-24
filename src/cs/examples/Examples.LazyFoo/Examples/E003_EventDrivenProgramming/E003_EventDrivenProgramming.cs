// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E003_EventDrivenProgramming : ExampleLazyFoo
{
    private Surface? _imageSurface;

    public E003_EventDrivenProgramming()
        : base("3 - Event Driven Programming", isEnabledCreateSurface: true)
    {
    }

    public override bool OnStart()
    {
        return TryLoadAssets();
    }

    public override void OnExit()
    {
        _imageSurface?.Dispose();
        _imageSurface = null;
    }

    public override void OnKeyboardEvent(in SDL_KeyboardEvent e)
    {
    }

    public override void OnUpdate(TimeSpan deltaTime)
    {
    }

    public override void OnDraw(TimeSpan deltaTime)
    {
        _imageSurface!.BlitTo(Window.Surface!);
        Window.Present();
    }

    private bool TryLoadAssets()
    {
        var assetsDirectory = Path.Combine(
            AppContext.BaseDirectory, "Examples", nameof(E003_EventDrivenProgramming));
        var filePath = Path.Combine(assetsDirectory, "x.bmp");
        return FileSystem.TryLoadImage(filePath, out _imageSurface!);
    }
}
