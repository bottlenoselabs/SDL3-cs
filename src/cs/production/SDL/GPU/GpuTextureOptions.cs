// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL.GPU;

/// <summary>
///     Parameters for creating a <see cref="GpuTexture" />.
/// </summary>
[PublicAPI]
public class GpuTextureOptions : BaseOptions
{
    /// <summary>
    ///     Gets or sets the optional name of the texture.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="GpuTextureType" /> of the texture.
    /// </summary>
    public GpuTextureType Type { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="GpuTextureFormat" /> of the texture.
    /// </summary>
    public GpuTextureFormat Format { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="GpuTextureUsages" /> of the texture.
    /// </summary>
    public GpuTextureUsages Usage { get; set; }

    /// <summary>
    ///     Gets or sets the width of the texture.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    ///     Gets or sets the height of the texture.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    ///     Gets or sets the layer count or depth of the texture. This value is treated as a layer count on 2D array
    ///     textures, and as a depth value on 3D textures.
    /// </summary>
    public int LayerCountOrDepth { get; set; }

    /// <summary>
    ///     Gets or sets the number of mipmap levels in the texture.
    /// </summary>
    public int MipmapLevelCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of samples per texel. Only applies if the texture is used as a render-target.
    /// </summary>
    public int SampleCount { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GpuTextureOptions" /> class.
    /// </summary>
    /// <param name="allocator">
    ///     The allocator to use for temporary interoperability allocations. If <c>null</c>, a
    ///     <see cref="ArenaNativeAllocator" /> is used with a capacity of 1 KB.
    /// </param>
    public GpuTextureOptions(INativeAllocator? allocator = null)
        : base(allocator)
    {
    }

    /// <inheritdoc />
    protected override void OnReset()
    {
        Name = null;
        Type = GpuTextureType.TwoDimensional;
        Format = GpuTextureFormat.Invalid;
        Usage = GpuTextureUsages.None;
        Width = 1;
        Height = 1;
        LayerCountOrDepth = 1;
        MipmapLevelCount = 0;
        SampleCount = 0;
    }
}
