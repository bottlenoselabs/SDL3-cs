// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

#pragma warning disable CA1815

/// <summary>
///     Parameters for creating a <see cref="GpuGraphicsPipeline" /> that describe how a color render-target is used.
/// </summary>
[PublicAPI]
public sealed class GpuGraphicsPipelineColorRenderTargetDescription
{
    /// <summary>
    ///     Gets or sets the texture format of the color render-target.
    /// </summary>
    public GpuTextureFormat Format { get; set; }

    /// <summary>
    ///     Gets or sets the blend state of the color render-target.
    /// </summary>
    public GpuGraphicsPipelineBlendState BlendState { get; set; } = new();

    /// <summary>
    ///     Resets the color render-target description to default values.
    /// </summary>
    public void Reset()
    {
        Format = GpuTextureFormat.Invalid;
        BlendState.Reset();
    }
}
