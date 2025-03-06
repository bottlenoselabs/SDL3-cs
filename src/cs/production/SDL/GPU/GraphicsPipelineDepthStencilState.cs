// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

#pragma warning disable CA1815

/// <summary>
///     Defines the configuration for the depth-test and stencil-test stages of the graphics pipeline.
/// </summary>
/// <remarks>
///     <para>
///         The depth-test and stencil-test stages both test whether each fragment (pixel) should be kept or discarded
///         for rendering before the per-fragment (per-pixel) shading stage in the graphics pipeline. Depth testing can
///         be used to ensure correct occlusion of primitives (e.g. closer primitives hide farther ones). Stencil
///         testing can be used for selective rendering of primitives' fragments (pixels).
///     </para>
///     <para>
///         Depth testing determines whether a fragment (pixel) is kept or discarded based on its depth (distance to
///         the camera) to existing fragments' (pixels') depth values in the depth buffer (a.k.a. Z buffer). If the new
///         fragment (pixel) is closer (passes the depth-test), the fragment (pixel) is kept and the depth buffer is
///         updated. If the new fragment (pixel) is further away (fails the depth-test), the fragment (pixel) is
///         discarded.
///     </para>
///     <para>
///         Stencil testing determines whether a fragment (pixel) is kept or discarded based on comparing its stencil
///         value in the stencil buffer to a reference value using a specified boolean comparison operator (always true,
///         always false, equal, greater than, etc). If the stencil-test passes for a fragment (pixel), the stencil
///         buffer is updated with the reference value.
///     </para>
/// </remarks>
[PublicAPI]
public sealed class GraphicsPipelineDepthStencilState
{
    /// <summary>
    ///     Gets or sets the boolean comparison operator used for depth testing.
    /// </summary>
    public CompareOp CompareOp { get; set; }

    /// <summary>
    ///     Gets or sets the stencil operation state for back-facing primitives.
    /// </summary>
    public GraphicsPipelineStencilOpState BackStencilState { get; set; } = new();

    /// <summary>
    ///     Gets or sets the stencil operation state for back-facing primitives.
    /// </summary>
    public GraphicsPipelineStencilOpState FrontStencilState { get; set; } = new();

    /// <summary>
    ///     Gets or sets the bitmask that determines the bits of the stencil value that participate in the stencil test.
    /// </summary>
    public byte ReadMask { get; set; }

    /// <summary>
    ///     Gets or sets the bitmask that determines the bits of the stencil value that are updated by the stencil test.
    /// </summary>
    public byte WriteMask { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the depth-test is enabled.
    /// </summary>
    public bool IsEnabledDepthTest { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether depth writes is enabled.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If <see cref="IsEnabledDepthTest" /> is <c>false</c>, then <see cref="IsEnabledDepthWrite" /> is always
    ///         <c>false</c>.
    ///     </para>
    /// </remarks>
    public bool IsEnabledDepthWrite { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the stencil-test is enabled.
    /// </summary>
    public bool IsEnabledStencilTest { get; set; }

    /// <summary>
    ///     Resets the depth stencil state to default values.
    /// </summary>
    public void Reset()
    {
        CompareOp = CompareOp.Invalid;
        BackStencilState.Reset();
        FrontStencilState.Reset();
        ReadMask = 0;
        WriteMask = 0;
        IsEnabledDepthTest = false;
        IsEnabledDepthWrite = false;
    }
}
