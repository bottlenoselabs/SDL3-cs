// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E005_OptimizedSurfaceLoadingAndSoftStretching : ExampleLazyFoo
{
    private Surface? _imageSurface;

    public E005_OptimizedSurfaceLoadingAndSoftStretching()
        : base("5 - Optimized Surface Loading and Soft Stretching", isEnabledCreateSurface: true)
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

    public override void OnUpdate(TimeSpan deltaTime)
    {
    }

    public override void OnDraw(TimeSpan deltaTime)
    {
        _imageSurface!.BlitTo(Window.Surface!, ScaleMode.Nearest);
        Window.Present();
    }

    private bool TryLoadAssets()
    {
        var assetsDirectory = Path.Combine(
            AppContext.BaseDirectory, "Examples", nameof(E005_OptimizedSurfaceLoadingAndSoftStretching));

        return FileSystem.TryLoadImage(
            Path.Combine(assetsDirectory, "stretch.bmp"), out _imageSurface!, Window.Surface!.PixelFormat);
    }
}
