// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines a width and a height using 32-bit integer components.
/// </summary>
[PublicAPI]
public record struct Size
{
    /// <summary>
    ///     The width.
    /// </summary>
    public int Width;

    /// <summary>
    ///     The height.
    /// </summary>
    public int Height;
}
