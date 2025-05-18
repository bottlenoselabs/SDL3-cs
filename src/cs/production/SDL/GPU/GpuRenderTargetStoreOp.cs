// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Defines what happens with the contents of a render-target at the end of a rendering pass.
/// </summary>
[PublicAPI]
public enum GpuRenderTargetStoreOp
{
    /// <summary>
    ///     The contents generated during the render pass will be written to memory.
    /// </summary>
    Store = 0,

    /// <summary>
    ///     The contents generated during the render pass are not needed and may be discarded. The contents will be
    ///     undefined.
    /// </summary>
    DontCare = 1,

    /// <summary>
    ///     The multi-sample contents generated during the render pass will be resolved to a non-multi-sample
    ///     render-target. The contents in the multi-sample render target may then be discarded and will be undefined.
    /// </summary>
    MultiSampleResolve = 2,

    /// <summary>
    ///     The multi-sample contents generated during the render pass will be resolved to a non-multi-sample
    ///     render-target. The contents in the multi-sample texture will be written to memory.
    /// </summary>
    MultiSampleResolveAndStore = 3
}
