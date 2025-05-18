// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Represents a developer programmable graphics program used in the vertex stage or fragment stage of the
///     graphics pipeline that transforms large amounts of data in parallel.
/// </summary>
/// <remarks>
///     <para>
///         A vertex shader takes in vertex data (vertices) as input and transforms each vertex as output in parallel.
///         Vertices represent points with attributes such as a position, a color, a normal vector, or texture
///         coordinates (or other developer specified). These vertices form lines or triangles creating the building
///         blocks for modeling any 2D or 3D geometry.
///     </para>
///     <para>
///         A fragment shader takes transformed information from the vertex shader and the hardware rasterization
///         process as input and outputs a fragment: a color value for each rasterized pixel in parallel. When
///         multi-sampling is enabled, the output is multiple fragments (pixels). When multi-sampling is disabled,
///         the output is a single fragment (pixel).
///     </para>
/// </remarks>
[PublicAPI]
public sealed unsafe class GpuGraphicsShader : GpuResource
{
    /// <summary>
    ///     Gets the <see cref="GpuShaderFormats " /> of the graphics shader.
    /// </summary>
    public GpuShaderFormats Format { get; private set; }

    /// <summary>
    ///     Gets the <see cref="GpuGraphicsShaderStage" /> of the graphics shader.
    /// </summary>
    public GpuGraphicsShaderStage Stage { get; private set; }

    internal GpuGraphicsShader(
        GPU.GpuDevice device,
        IntPtr handle,
        GpuShaderFormats format,
        GpuGraphicsShaderStage stage)
        : base(device, handle)
    {
        Format = format;
        Stage = stage;
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        SDL_ReleaseGPUShader((SDL_GPUDevice*)Device.Handle, (SDL_GPUShader*)Handle);
        base.Dispose(isDisposing);
    }
}
