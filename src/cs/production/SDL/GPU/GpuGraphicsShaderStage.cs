// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines which stage a <see cref="GpuGraphicsShader" /> corresponds to.
/// </summary>
[PublicAPI]
public enum GpuGraphicsShaderStage
{
    /// <summary>
    ///     Shader is a graphics vertex shader.
    /// </summary>
    Vertex = 0,

    /// <summary>
    ///     Shader is a graphics fragment (pixel) shader.
    /// </summary>
    Fragment = 1
}
