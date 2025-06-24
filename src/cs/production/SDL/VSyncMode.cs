// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines the vertical synchronization modes.
/// </summary>
/// <remarks>
///     <para>
///         When vertical synchronization is enabled, it forces the GPU to wait for the monitor's refresh cycle
///         (measured in hertz or seconds per cycle) before rendering a new frame which can help prevent possible screen
///         tearing at the cost of possible input lag. Screen tearing is misaligned frames when the GPU outputs rendered
///         frames faster than the monitor can display them causing possible unwanted visual artifacts.
///     </para>
///     <para></para>
/// </remarks>
public enum VSyncMode
{
    /// <summary>
    ///     Vertical synchronization is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     Vertical synchronization is disabled.
    /// </summary>
    Disabled,

    /// <summary>
    ///     Vertical synchronization is enabled for every vertical refresh.
    /// </summary>
    EnabledEveryRefresh,

    /// <summary>
    ///     Vertical synchronization is enabled for every second vertical refresh.
    /// </summary>
    EnabledEverySecondRefresh,

    /// <summary>
    ///     Vertical synchronization flips between enabled or disabled depending on the frame rate.
    /// </summary>
    Adaptive
}
