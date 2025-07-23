// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Text.RegularExpressions;
using bottlenoselabs.SDL;
using Common;

namespace Gpu;

public abstract partial class ExampleGpu : ExampleBase
{
    protected GpuDevice Device { get; private set; }

    protected ExampleGpu()
    {
        Name = RegexExampleTypeName().Replace(GetType().Name, match =>
        {
            var number = match.Groups[1].Value.TrimStart('0');
            // Insert spaces between camel case
            var words = RegexWords().Replace(match.Groups[2].Value, "$1 $2");
            return $"{number} - {words}";
        });
        Device = Application.CreateGpuDevice();
        AssetsDirectory = Path.Combine(AppContext.BaseDirectory, "Assets");
    }

    public override bool OnStart()
    {
        var isSuccess = Device.TryClaimWindow(Window);
        return isSuccess;
    }

    public override void OnExit()
    {
        Device.Dispose();
        Device = null!;
    }

    [GeneratedRegex(@"E(\d+)_(\w+)")]
    private static partial Regex RegexExampleTypeName();

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex RegexWords();
}
