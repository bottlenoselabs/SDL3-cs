// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using Common;
using Microsoft.Extensions.Logging;
using SDL;

namespace LazyFoo;

public abstract unsafe class ExampleLazyFoo : ExampleBase
{
    private readonly bool _createRenderer;

    public SDL_Renderer* Renderer { get; private set; }

    protected ExampleLazyFoo(
        string name,
        bool createRenderer = true,
        WindowOptions? windowOptions = null,
        LogLevel logLevel = LogLevel.Warning)
        : base(windowOptions, logLevel)
    {
        Name = name;
        _createRenderer = createRenderer;
        AssetsDirectory = Path.Combine(AppContext.BaseDirectory, "Examples", GetType().Name);
    }

    public override bool Initialize(INativeAllocator allocator)
    {
        if (_createRenderer)
        {
            Renderer = SDL_CreateRenderer((SDL_Window*)Window.Handle, null);
            if (Renderer == null)
            {
                Console.Error.WriteLine("Failed to create renderer. SDL error: " + SDL_GetError());
                return false;
            }
        }

        return true;
    }

    public override void Quit()
    {
        if (_createRenderer)
        {
            SDL_DestroyRenderer(Renderer);
            Renderer = null;
        }
    }
}
