// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Defines what happens with the contents of a render-target at the start of a rendering pass.
/// </summary>
[PublicAPI]
public enum RenderTargetLoadOp
{
    /// <summary>
    ///     The previous contents of the render-target will be preserved.
    /// </summary>
    Load = 0,

    /// <summary>
    ///     The contents of the render-target will be cleared to a specified value.
    /// </summary>
    Clear = 1,

    /// <summary>
    ///     The previous contents of the render-target need not be preserved. The contents will be undefined.
    /// </summary>
    DontCare = 2
}
