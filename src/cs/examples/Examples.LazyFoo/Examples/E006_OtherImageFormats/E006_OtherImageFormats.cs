// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E006_OtherImageFormats : ExampleLazyFoo
{
    private Surface? _imageSurface;

    public E006_OtherImageFormats()
        : base("6 - Other Image Formats", isEnabledCreateSurface: true)
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

    public override void OnUpdate(float deltaTime)
    {
    }

    public override void OnDraw(float deltaTime)
    {
        _imageSurface!.BlitTo(Window.Surface!, ScaleMode.Nearest);
        Window.Present();
    }

    private bool TryLoadAssets()
    {
        var assetsDirectory = Path.Combine(
            AppContext.BaseDirectory, "Examples", nameof(E006_OtherImageFormats));

        return FileSystem.TryLoadImage(
            Path.Combine(assetsDirectory, "loaded.png"), out _imageSurface);
    }
}
