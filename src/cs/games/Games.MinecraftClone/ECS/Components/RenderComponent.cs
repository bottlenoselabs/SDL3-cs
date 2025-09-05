// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using bottlenoselabs.SDL;

namespace Games.MinecraftClone.ECS.Components;

public record struct RenderComponent
{
    public GpuDevice Device;
    public GpuCommandBuffer CommandBuffer;
    public GpuTexture SwapchainTexture;
    public bool CanRender;
}
