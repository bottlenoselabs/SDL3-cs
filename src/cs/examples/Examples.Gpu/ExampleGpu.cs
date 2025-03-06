// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Text.RegularExpressions;
using Common;
using Microsoft.Extensions.Logging;
using SDL.GPU;

namespace Gpu;

public abstract unsafe partial class ExampleGpu : ExampleBase
{
    public Device Device { get; private set; }

    protected ExampleGpu(WindowOptions? windowOptions = null, LogLevel logLevel = LogLevel.Warning)
        : base(windowOptions, logLevel)
    {
        Name = RegexExampleTypeName().Replace(GetType().Name, match =>
        {
            var number = match.Groups[1].Value.TrimStart('0');
            // Insert spaces between camel case
            var words = RegexWords().Replace(match.Groups[2].Value, "$1 $2");
            return $"{number} - {words}";
        });
        Device = new Device(
            new Logger<Device>(LoggerFactory),
            GraphicsShaderFormats.SPIRV | GraphicsShaderFormats.DXIL | GraphicsShaderFormats.MSL);
        AssetsDirectory = Path.Combine(AppContext.BaseDirectory, "Assets");
    }

    public override bool Initialize(INativeAllocator allocator)
    {
        var isSuccess = Device.TryClaimWindow(Window);
        return isSuccess;
    }

    public override void Quit()
    {
        Device.Dispose();
        Device = null!;
    }

    [GeneratedRegex(@"E(\d+)_(\w+)")]
    private static partial Regex RegexExampleTypeName();

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex RegexWords();
}
