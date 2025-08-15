// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines the different strategies for how texels of a <see cref="GpuTexture" /> are mapped into fragments
///     (pixels), also known as sampling. Depending on the strategy chosen the pixels will show a varying degree of
///     blurriness, detail, and aliasing.
/// </summary>
/// <remarks>
///     <para>
///         There are 3 different cases for filtering.
///         <list type="bullet">
///             <item>
///                 <description>Each texel maps onto more than one fragment (pixel). This is known as magnification.</description>
///             </item>
///             <item>
///                 <description>Each texel maps exactly onto one fragment (pixel). Filtering does not apply in this case.</description>
///             </item>
///             <item>
///                 <description>Each texel maps onto less than one fragment (pixel). This is known as minification.</description>
///             </item>
///         </list>
///         Magnification and minification can be set for a <see cref="GpuTexture" /> in the
///         <see cref="GpuTextureOptions" /> when calling <see cref="GpuDevice.TryCreateTexture" />.
///     </para>
/// </remarks>
[PublicAPI]
public enum GpuSamplerFilterMode
{
    /// <summary>
    ///     Uses the closest texel (in Manhattan distance) to the center of fragment (pixel) being rendered.
    /// </summary>
    Nearest = 0,

    /// <summary>
    ///     Interpolates between the four nearest texels (in Manhattan distance) to the center of the fragment (pixel)
    ///     being rendered.
    /// </summary>
    Linear = 1
}
