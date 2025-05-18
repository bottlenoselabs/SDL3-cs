// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using Common;

namespace LazyFoo;

public abstract class ExampleLazyFoo : ExampleBase
{
    protected ExampleLazyFoo(
        string name, bool isEnabledCreateSurface = false, bool isEnabledCreateRenderer2D = false)
        : base(isEnabledCreateSurface, isEnabledCreateRenderer2D)
    {
        Name = name;
        AssetsDirectory = Path.Combine(AppContext.BaseDirectory, "Examples", GetType().Name);
    }
}
