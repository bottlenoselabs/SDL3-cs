// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Represents a GPU resource that handles the mapping of texture elements (texels) of a <see cref="Texture" />
///     into fragments (pixels) when rendering with a fragment (pixel) <see cref="GraphicsShader" />.
/// </summary>
/// <remarks>
///     <para>
///         Texture sampling is the process of fetching and filtering texels (texture elements) from a texture map to
///         determine the color or other attributes of a fragment (pixel) being rendered.
///     </para>
///     <para>
///         Texture fetching retrieves texel data from a texture map based on texture coordinates (UV or UVW
///         coordinates) provided by the shader.
///     </para>
///     <para>
///         Texture filtering applies various techniques to determine the final color when texels don't align perfectly
///         with the screen's or the render-target's pixels. See <see cref="SamplerFilter" />.
///     </para>
/// </remarks>
[PublicAPI]
public sealed class Sampler : Resource
{
    internal Sampler(Device device, IntPtr handle)
        : base(device, handle)
    {
    }
}
