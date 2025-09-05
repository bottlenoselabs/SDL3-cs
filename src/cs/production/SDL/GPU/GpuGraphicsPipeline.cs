// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Represents the stages (steps) that a GPU performs in-order to convert 2D or 3D geometry into either: pixels that
///     are displayed to the screen or texels written to an off-screen render-target.
/// </summary>
/// <remarks>
///     <para>
///         Some stages are programmed using shaders, others have fixed or configurable behavior. The five main stages
///         of the pipeline in-order include: the per-vertex shading stage, the rasterization stage, depth-test stage,
///         stencil-test stage, and the per-fragment (per-pixel) shading stage. The per-vertex shading stage and
///         per-fragment (per-pixel) shading stage are programmable. The rasterization stage, depth-test stage, and
///         stencil-test stages are fixed but configurable. Remaining other stages are hidden from the perspective of
///         the developer as they are fixed and non-configurable.
///     </para>
///     <para>
///         To create a <see cref="GpuGraphicsPipeline" />, call <see cref="GpuDevice.TryCreateGraphicsPipeline" /> with the
///         specified <see cref="GpuGraphicsPipelineOptions" /> struct.
///     </para>
///     <para>
///         To activate a <see cref="GpuGraphicsPipeline" /> for rendering, and by consequence deactivate any other
///         active <see cref="GpuGraphicsPipeline" />, call <see cref="GpuRenderPass.BindPipeline" />.
///     </para>
/// </remarks>
[PublicAPI]
public sealed unsafe class GpuGraphicsPipeline : GpuResource<SDL_GPUGraphicsPipeline>
{
    internal GpuGraphicsPipeline(GpuDevice device, SDL_GPUGraphicsPipeline* handle)
        : base(device, handle)
    {
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        SDL_ReleaseGPUGraphicsPipeline(Device.HandleTyped, HandleTyped);
        base.Dispose(isDisposing);
    }
}
