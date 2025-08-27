// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

namespace Gpu.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed class E011_BasicCompute : ExampleGpu
{
    public override bool OnStart()
    {
        var vertexShaderOptions = new GpuGraphicsShaderOptions();
        if (!FileSystem.TryLoadGraphicsShader(
                GetShaderFilePath("TexturedQuad.vert"),
                Device,
                vertexShaderOptions,
                out var vertexShader))
        {
            return false;
        }

        var fragmentShaderOptions = new GpuGraphicsShaderOptions();
        fragmentShaderOptions.SamplerCount = 1;
        if (!FileSystem.TryLoadGraphicsShader(
                GetShaderFilePath("TexturedQuad.frag"),
                Device,
                fragmentShaderOptions,
                out var fragmentShader))
        {
            return false;
        }

        var computeShaderOptions = new GpuComputeShaderOptions();
        computeShaderOptions.ReadWriteStorageTextureCount = 1;
        computeShaderOptions.ThreadXCount = 8;
        computeShaderOptions.ThreadYCount = 8;
        computeShaderOptions.ThreadZCount = 1;
        if (!FileSystem.TryLoadComputeShader(
                GetShaderFilePath("FillTexture.comp"),
                Device,
                computeShaderOptions,
                out var computeShader))
        {
            return false;
        }

        return true;
    }
}
