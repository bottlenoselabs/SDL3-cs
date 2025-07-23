// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines the face direction of primitives that will be culled by the graphics pipeline during rasterization.
/// </summary>
[PublicAPI]
public enum GpuGraphicsPipelineCullMode
{
    /// <summary>
    ///     No primitives are culled.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Font-facing primitives are culled.
    /// </summary>
    Front,

    /// <summary>
    ///     Back-facing primitives are culled.
    /// </summary>
    Back
}
