// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

#pragma warning disable CA1815

/// <summary>
///     Parameters for creating a <see cref="GpuGraphicsPipeline" /> that describe a color render-target's blend state.
/// </summary>
[PublicAPI]
public sealed class GpuGraphicsPipelineBlendState
{
    /// <summary>
    ///     Gets or sets the value to be multiplied by the source RGB value.
    /// </summary>
    public GpuBlendFactor SourceColorBlendFactor { get; set; }

    /// <summary>
    ///     Gets or sets the value to be multiplied by the destination RGB value.
    /// </summary>
    public GpuBlendFactor DestinationColorBlendFactor { get; set; }

    /// <summary>
    ///     Gets or sets the blend operation for the RGB components.
    /// </summary>
    public SDL_GPUBlendOp ColorBlendOp { get; set; }

    /// <summary>
    ///     Gets or sets the value to be multiplied by the source alpha.
    /// </summary>
    public GpuBlendFactor SourceAlphaBlendFactor { get; set; }

    /// <summary>
    ///     Gets or sets the value to be multiplied by the destination alpha.
    /// </summary>
    public GpuBlendFactor DestinationAlphaBlendFactor { get; set; }

    /// <summary>
    ///     Gets or sets the blend operation for the alpha component.
    /// </summary>
    public SDL_GPUBlendOp AlphaBlendOp { get; set; }

    /// <summary>
    ///     Gets or sets the bitmask specifying which of the RGBA components are enabled for writing. Writes to all
    ///     channels if <see cref="IsEnabledColorWriteMask" /> is <c>false</c>.
    /// </summary>
    public SDL_GPUColorComponentFlags ColorWriteMask { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether blending is enabled for the color render-target.
    /// </summary>
    public bool IsEnabledBlend { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether <see cref="ColorWriteMask" /> is enabled.
    /// </summary>
    public bool IsEnabledColorWriteMask;

    /// <summary>
    ///     Resets the blend state to default values.
    /// </summary>
    public void Reset()
    {
        SourceColorBlendFactor = GpuBlendFactor.Invalid;
        DestinationColorBlendFactor = GpuBlendFactor.Invalid;
        ColorBlendOp = SDL_GPUBlendOp.SDL_GPU_BLENDOP_INVALID;
        SourceAlphaBlendFactor = GpuBlendFactor.Invalid;
        DestinationAlphaBlendFactor = GpuBlendFactor.Invalid;
        AlphaBlendOp = SDL_GPUBlendOp.SDL_GPU_BLENDOP_INVALID;
        ColorWriteMask = 0;
        IsEnabledBlend = false;
        IsEnabledColorWriteMask = false;
    }
}
