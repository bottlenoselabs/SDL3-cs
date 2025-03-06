// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL;

#pragma warning disable CA1815

/// <summary>
///     Parameters for creating a <see cref="Window" />.
/// </summary>
[PublicAPI]
public class WindowOptions
{
    /// <summary>
    ///     Gets or sets the text title of the window.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    ///     Gets or sets the width of the window.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    ///     Gets or sets the height of the window.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the window is resizable.
    /// </summary>
    public bool IsResizable { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the window starts maximized.
    /// </summary>
    public bool IsStartingMaximized { get; set; }
}
