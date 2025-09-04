// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace Games.MinecraftClone;

public static class Program
{
    private static void Main()
    {
        using var game = new Game();
        game.Run();
    }
}
