// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Numerics;
using bottlenoselabs.Interop;

namespace Examples.MinecraftClone;

#pragma warning disable CA1815

public record struct Vertex
{
    public Vector3 Position;
    public Rgba8U Color;
}
