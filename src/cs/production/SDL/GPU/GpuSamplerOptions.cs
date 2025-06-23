// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL.GPU;

/// <summary>
///     Parameters for creating a <see cref="GpuSampler" />.
/// </summary>
[PublicAPI]
public class GpuSamplerOptions : BaseOptions
{
    /// <summary>
    ///     Gets or sets the <see cref="GpuSamplerFilter" /> used in sampling when each texel maps onto more than one
    ///     fragment (pixel). Default is <see cref="GpuSamplerFilter.Nearest" />.
    /// </summary>
    public GpuSamplerFilter MagnificationFilter { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="GpuSamplerFilter" /> used in sampling when each texel maps onto less than one
    ///     fragment (pixel). Default is <see cref="GpuSamplerFilter.Nearest" />.
    /// </summary>
    public GpuSamplerFilter MinificationFilter { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="GpuSamplerMipmapMode" /> used in sampling. Default is
    ///     <see cref="GpuSamplerMipmapMode.Nearest" />.
    /// </summary>
    public GpuSamplerMipmapMode MipMapMode { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="GpuSamplerAddressMode" /> used in sampling when the texture width coordinate (U)
    ///     is outside [0, 1). Default is <see cref="GpuSamplerAddressMode.Repeat" />.
    /// </summary>
    public GpuSamplerAddressMode AddressModeU { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="GpuSamplerAddressMode" /> used in sampling when the texture height coordinate (V)
    ///     is outside [0, 1). Default is <see cref="GpuSamplerAddressMode.Repeat" />.
    /// </summary>
    public GpuSamplerAddressMode AddressModeV { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="GpuSamplerAddressMode" /> used in sampling when the texture depth coordinate (W)
    ///     is outside [0, 1). Default is <see cref="GpuSamplerAddressMode.Repeat" />.
    /// </summary>
    public GpuSamplerAddressMode AddressModeW { get; set; }

    /// <summary>
    ///     Gets or sets the maximum number of texels to sample per fragment (pixel) when the viewing angle of a
    ///     texture is at a sharp angle. For best results, use with mipmapping. Must be between <c>1</c> and <c>16</c>,
    ///     inclusively. Default is <c>1</c>. If <see cref="IsEnabledAnisotropy" /> is <c>false</c>,
    ///     <see cref="MaximumAnisotropy" /> is ignored.
    /// </summary>
    public float MaximumAnisotropy { get; set; }

    /// <summary>
    ///     Gets or sets the lower end of the mipmap range to clamp access to where <c>0</c> represents the largest and
    ///     most detailed mipmap and any higher level is less detailed. Default is <c>0.0f</c>.
    /// </summary>
    public float MinimumLevelOfDetailClamp { get; set; }

    /// <summary>
    ///     Gets or sets the higher end of the mipmap range to clamp access to where <c>0</c> represents the largest and
    ///     most detailed mipmap and any higher level is less detailed. Must be greater than or equal to
    ///     <see cref="MinimumLevelOfDetailClamp" />. Default is <see cref="float.MaxValue" />.
    /// </summary>
    public float MaximumLevelOfDetailClamp { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether anisotropic filtering is enabled or disabled. Default is
    ///     <c>false</c>.
    /// </summary>
    public bool IsEnabledAnisotropy;

    /// <summary>
    ///     Gets or sets a value indicating whether depth sampling and comparison against a reference value is enabled
    ///     or disabled. Default is <c>false</c>.
    /// </summary>
    public bool IsEnabledDepthCompare { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="GpuCompareOp" /> used for comparing a sampled depth value from the depth texture
    ///     against a reference value when <see cref="IsEnabledDepthCompare" /> is <c>true</c>. If
    ///     <see cref="IsEnabledDepthCompare" /> is <c>false</c>, <see cref="DepthCompareOp" /> is ignored.
    /// </summary>
    public GpuCompareOp DepthCompareOp { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GpuSamplerOptions" /> class.
    /// </summary>
    /// <param name="allocator">
    ///     The allocator to use for temporary interoperability allocations. If <c>null</c>, a
    ///     <see cref="ArenaNativeAllocator" /> is used with a capacity of 1 KB.
    /// </param>
    public GpuSamplerOptions(INativeAllocator? allocator = null)
        : base(allocator)
    {
    }

    /// <inheritdoc />
    protected override void OnReset()
    {
        MagnificationFilter = GpuSamplerFilter.Nearest;
        MinificationFilter = GpuSamplerFilter.Nearest;
        MipMapMode = GpuSamplerMipmapMode.Nearest;
        AddressModeU = GpuSamplerAddressMode.Repeat;
        AddressModeV = GpuSamplerAddressMode.Repeat;
        AddressModeW = GpuSamplerAddressMode.Repeat;
        MaximumAnisotropy = 1;
        MinimumLevelOfDetailClamp = 0.0f;
        MaximumLevelOfDetailClamp = float.MaxValue;
        IsEnabledAnisotropy = false;
        IsEnabledDepthCompare = false;
        DepthCompareOp = GpuCompareOp.Invalid;
    }
}
