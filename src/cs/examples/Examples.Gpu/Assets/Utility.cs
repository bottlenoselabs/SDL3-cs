// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace Gpu.Assets;

public static class Utility
{
    public static string GetShaderFilePath(string name)
    {
        string vertexShaderFilePath;
        if (OperatingSystem.IsWindows())
        {
            vertexShaderFilePath = $"Assets/Shaders/Compiled/DXIL/{name}.dxil";
        }
        else if (OperatingSystem.IsMacOS())
        {
            vertexShaderFilePath = $"Assets/Shaders/Compiled/MSL/{name}.msl";
        }
        else if (OperatingSystem.IsLinux())
        {
            vertexShaderFilePath = $"Assets/Shaders/Compiled/SPIRV/{name}.spv";
        }
        else
        {
            throw new NotImplementedException();
        }

        return vertexShaderFilePath;
    }
}
