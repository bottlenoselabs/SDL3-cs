// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E011_BasicCompute : ExampleGpu
{
    public override bool OnStart()
    {
        if (!FileSystem.TryLoadGraphicsShader(
                GetShaderFilePath("TexturedQuad.vert"),
                Device,
                out var vertexShader))
        {
            return false;
        }

        if (!FileSystem.TryLoadGraphicsShader(
                GetShaderFilePath("TexturedQuad.frag"),
                Device,
                out var fragmentShader,
                samplerCount: 1))
        {
            return false;
        }

        if (!FileSystem.TryLoadComputeShader(
                GetShaderFilePath("FillTexture.comp"),
                Device,
                out var computePipeline))
        {
            return false;
        }

        return true;
    }
}
