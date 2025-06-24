// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E002_ImageOnScreen : ExampleLazyFoo
{
    private Surface? _imageSurface;

    public E002_ImageOnScreen()
        : base("2 - Image on Screen", isEnabledCreateSurface: true)
    {
    }

    public override bool OnStart()
    {
        if (!TryLoadAssets())
        {
            return false;
        }

        _imageSurface!.BlitTo(Window.Surface!);
        Window.Present();

        return true;
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
    }

    private bool TryLoadAssets()
    {
        var assetsDirectory = Path.Combine(AppContext.BaseDirectory, "Examples", nameof(E002_ImageOnScreen));
        var filePath = Path.Combine(assetsDirectory, "hello_world.bmp");
        return FileSystem.TryLoadImage(filePath, out _imageSurface!);
    }
}
