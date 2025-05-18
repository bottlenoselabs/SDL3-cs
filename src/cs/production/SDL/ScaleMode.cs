// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL;

/// <summary>
///     Defines surface scale modes.
/// </summary>
[PublicAPI]
public enum ScaleMode
{
    /// <summary>
    ///     Invalid.
    /// </summary>
    Invalid = -1,

    /// <summary>
    ///     Nearest pixel sampling.
    /// </summary>
    Nearest = 0,

    /// <summary>
    ///     Linear filtering.
    /// </summary>
    Linear = 1,

    /// <summary>
    ///     Nearest pixel sampling with improved scaling for pixel art.
    /// </summary>
    PixelArt = 2
}
