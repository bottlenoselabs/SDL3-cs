// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Represents a font.
/// </summary>
public sealed unsafe class Font : NativeHandle
{
    private TTF_Font* _handle;

    internal Font(IntPtr handle)
        : base(handle)
    {
        _handle = (TTF_Font*)handle;
    }

    /// <summary>
    ///     Renders the specified string at high quality to a new 32-bit ARGB <see cref="Surface" />.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="color">The foreground color of the text.</param>
    /// <param name="allocator">The native allocator to use to allocate the C string.</param>
    /// <param name="surface">The resulting <see cref="Surface" /> if successful; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the <paramref name="surface" /> was successfully created; otherwise, <c>false</c>.</returns>
    public bool TryRenderTextBlended(
        string text,
        Rgba8U color,
        INativeAllocator allocator,
        out Surface? surface)
    {
        var textCString = allocator.AllocateCString(text);
        var surfacePointer = TTF_RenderText_Blended(_handle, textCString, 0, color);
        if (surfacePointer == null)
        {
            Error.NativeFunctionFailed(nameof(TTF_RenderText_Blended));
            surface = null;
            return false;
        }

        surface = new Surface((IntPtr)surfacePointer);
        return true;
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        TTF_CloseFont(_handle);
        _handle = null;
        base.Dispose(isDisposing);
    }
}
