// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace Gpu;

public record struct VertexPositionColor
{
    public Vector3 Position;
    public Rgba8U Color;

    public static int SizeOf => Unsafe.SizeOf<VertexPositionColor>();
}
