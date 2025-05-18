// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL;

/// <summary>
///     Defines the SDL platforms.
/// </summary>
[PublicAPI]
public enum Platform
{
    /// <summary>
    ///     Unknown native platform.
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     Windows.
    /// </summary>
    Windows,

    /// <summary>
    ///     macOS.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    macOS,

    /// <summary>
    ///     Linux.
    /// </summary>
    Linux,

    /// <summary>
    ///     Apple iOS.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    iOS,

    /// <summary>
    ///     Android.
    /// </summary>
    Android
}
