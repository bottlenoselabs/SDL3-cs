// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

#pragma warning disable CA1815

/// <summary>
///     Defines how vertex data is fetched from one or more vertex buffers and mapped into the per-vertex stage of the
///     graphics pipeline.
/// </summary>
/// <remarks>
///     <para>
///         Use standard struct allocation and initialization techniques to create a
///         <see cref="GraphicsPipelineDescriptor" />.
///     </para>
/// </remarks>
[PublicAPI]
public struct GraphicsPipelineVertexInputState
{
    /// <summary>
    ///     The vertex buffer descriptions array pointer.
    /// </summary>
    public IntPtr VertexBufferDescriptionsArrayPointer;

    /// <summary>
    ///     The number of vertex buffer descriptions in the <see cref="VertexBufferDescriptionsArrayPointer" />.
    /// </summary>
    public int VertexBufferDescriptionCount;

    /// <summary>
    ///     The vertex attribute descriptions array pointer.
    /// </summary>
    public IntPtr VertexAttributesArrayPointer;

    /// <summary>
    ///     The number of vertex attribute descriptions in the <see cref="VertexAttributesArrayPointer" />.
    /// </summary>
    public int VertexAttributeCount;
}
