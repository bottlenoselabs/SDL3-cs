// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     TODO.
/// </summary>
[PublicAPI]
public sealed unsafe class Texture : NativeHandleTyped<SDL_Texture>
{
    /// <summary>
    ///     Gets the width of the texture.
    /// </summary>
    public float Width { get; }

    /// <summary>
    ///     Gets the height of the texture.
    /// </summary>
    public float Height { get; }

    /// <summary>
    ///     Gets or sets the modulated color of the texture.
    /// </summary>
    public Rgb8U Color
    {
        get => GetColor();
        set => SetColor(value);
    }

    /// <summary>
    ///     Gets or sets the modulated alpha of the texture.
    /// </summary>
    public byte Alpha
    {
        get => GetAlpha();
        set => SetAlpha(value);
    }

    /// <summary>
    ///     Gets or sets the blend mode of the texture.
    /// </summary>
    public BlendMode BlendMode
    {
        get => GetBlendMode();
        set => SetBlendMode(value);
    }

    internal Texture(SDL_Texture* handle)
        : base(handle)
    {
        float width;
        float height;
        var isSuccess = SDL_GetTextureSize(HandleTyped, &width, &height);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_GetTextureSize));
        }

        Width = width;
        Height = height;
    }

    private Rgb8U GetColor()
    {
        byte r;
        byte g;
        byte b;

        var isSuccess = SDL_GetTextureColorMod(HandleTyped, &r, &g, &b);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_GetTextureColorMod), isExceptionThrown: true);
        }

        var color = new Rgb8U(r, g, b);
        return color;
    }

    private void SetColor(in Rgb8U color)
    {
        var isSuccess = SDL_SetTextureColorMod(HandleTyped, color.R, color.G, color.B);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_SetTextureColorMod), isExceptionThrown: true);
        }
    }

    private byte GetAlpha()
    {
        byte a;

        var isSuccess = SDL_GetTextureAlphaMod(HandleTyped, &a);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_GetTextureAlphaMod), isExceptionThrown: true);
        }

        return a;
    }

    private void SetAlpha(in byte value)
    {
        var isSuccess = SDL_SetTextureAlphaMod(HandleTyped, value);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_SetTextureAlphaMod), isExceptionThrown: true);
        }
    }

    private BlendMode GetBlendMode()
    {
        BlendMode blendMode;

        var pointer = (SDL_BlendMode*)&blendMode;
        var isSuccess = SDL_GetTextureBlendMode(HandleTyped, pointer);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_GetTextureBlendMode), isExceptionThrown: true);
        }

        return blendMode;
    }

    private void SetBlendMode(BlendMode blendMode)
    {
        var pointer = (SDL_BlendMode*)&blendMode;
        var isSuccess = SDL_SetTextureBlendMode(HandleTyped, *pointer);
        if (!isSuccess)
        {
            Error.NativeFunctionFailed(nameof(SDL_SetTextureBlendMode), isExceptionThrown: true);
        }
    }
}
