// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Runtime.CompilerServices;

namespace Examples.MinecraftClone.Engine.ECS;

public record struct NativeType
{
    public int Size;

    public static NativeType Of<T>()
    {
        return new NativeType
        {
            Size = Unsafe.SizeOf<T>()
        };
    }
}
