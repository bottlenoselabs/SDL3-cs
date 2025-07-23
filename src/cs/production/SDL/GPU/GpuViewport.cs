// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

#pragma warning disable CA1815

/// <summary>
///     Defines the view bounds of the screen in pixels, or the view bounds of an off-screen render-target in texels,
///     where a <see cref="GpuRenderPass" /> instance is rendering to.
/// </summary>
/// <remarks>
///     <para>
///         The <see cref="GpuViewport" /> is used to map Normalized Device Coordinates (NDCs) to the screen's pixel
///         coordinates (or an off-screen render-target's texel coordinates).
///     </para>
/// </remarks>
[PublicAPI]
public struct GpuViewport
{
    /// <summary>
    ///     The X coordinate of the upper-left corner of the view bounds.
    /// </summary>
    public float X;

    /// <summary>
    ///     The Y coordinate of the upper-left corner of the view bounds.
    /// </summary>
    public float Y;

    /// <summary>
    ///     The width of the view bounds.
    /// </summary>
    public float Width;

    /// <summary>
    ///     The height of the view bounds.
    /// </summary>
    public float Height;

    /// <summary>
    ///     The lower depth limit of the view bounds.
    /// </summary>
    public float MinDepth;

    /// <summary>
    ///     The upper depth limit of the view bounds.
    /// </summary>
    public float MaxDepth;
}
