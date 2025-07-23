// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

#pragma warning disable CA1815

/// <summary>
///     Parameters for creating a <see cref="GpuGraphicsPipeline" /> instance that describe how a single vertex buffer
///     is used to fetch and map vertex data into the per-vertex processing stage of the graphics pipeline.
/// </summary>
/// <remarks>
///     <para>
///         Use standard struct allocation and initialization techniques to create a
///         <see cref="GpuGraphicsPipelineOptions" />.
///     </para>
/// </remarks>
[PublicAPI]
public struct GpuGraphicsPipelineVertexBufferDescription
{
    /// <summary>
    ///     The binding slot of the vertex buffer.
    /// </summary>
    public int Slot;

    /// <summary>
    ///     The number of bytes between consecutive elements in the vertex buffer.
    /// </summary>
    public int Pitch;

    /// <summary>
    ///     The rate for fetching vertex attributes from a vertex buffer.
    /// </summary>
    public GpuGraphicsPipelineVertexInputRate InputRate;

    /// <summary>
    ///     The number of instances to draw using the same per-instance data before advancing the buffer by one element.
    ///     Default is <c>1</c>. Ignored if <see cref="InputRate" /> is
    ///     <see cref="GpuGraphicsPipelineVertexInputRate.PerVertex" />. If the value is equal to <c>1</c>, new data is
    ///     fetched for every instance; if the value is equal to <c>2</c>, new data is fetched for every two
    ///     instances, and so forth.
    /// </summary>
    public int InstanceStepRate;
}
