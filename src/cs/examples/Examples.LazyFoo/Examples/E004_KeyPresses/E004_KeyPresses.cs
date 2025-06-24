// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E004_KeyPresses : ExampleLazyFoo
{
    private Surface? _currentKeyPressSurface;
    private readonly Surface?[] _keyPressSurfaces = new Surface[Enum.GetValues<KeyPressSurfaceIndex>().Length];

    public E004_KeyPresses()
        : base("4 - KeyPresses", isEnabledCreateSurface: true)
    {
    }

    public override bool OnStart()
    {
        return TryLoadAssets();
    }

    public override void OnExit()
    {
        for (var i = 0; i < _keyPressSurfaces.Length; i++)
        {
            ref var keyPressSurface = ref _keyPressSurfaces[i];
            keyPressSurface?.Dispose();
            keyPressSurface = null;
        }
    }

    public override void OnKeyboardEvent(in SDL_KeyboardEvent e)
    {
        if (e.down)
        {
            var keyPressSurface = e.scancode switch
            {
                SDL_Scancode.SDL_SCANCODE_UP => KeyPressSurfaceIndex.Up,
                SDL_Scancode.SDL_SCANCODE_DOWN => KeyPressSurfaceIndex.Down,
                SDL_Scancode.SDL_SCANCODE_LEFT => KeyPressSurfaceIndex.Left,
                SDL_Scancode.SDL_SCANCODE_RIGHT => KeyPressSurfaceIndex.Right,
                _ => KeyPressSurfaceIndex.Press
            };

            _currentKeyPressSurface = _keyPressSurfaces[(int)keyPressSurface];
        }
    }

    public override void OnUpdate(TimeSpan deltaTime)
    {
    }

    public override void OnDraw(TimeSpan deltaTime)
    {
        _currentKeyPressSurface!.BlitTo(Window.Surface!);
        Window.Present();
    }

    private bool TryLoadAssets()
    {
        var assetsDirectory = Path.Combine(
            AppContext.BaseDirectory, "Examples", nameof(E004_KeyPresses));

        var enumValues = Enum.GetValues<KeyPressSurfaceIndex>();
        foreach (var enumValue in enumValues)
        {
            var name = enumValue.ToString().ToLowerInvariant();
            var filePath = Path.Combine(assetsDirectory, $"{name}.bmp");
            if (!FileSystem.TryLoadImage(filePath, out var imageSurface))
            {
                return false;
            }

            _keyPressSurfaces[(int)enumValue] = imageSurface!;
        }

        _currentKeyPressSurface = _keyPressSurfaces[0];

        return true;
    }
}
