// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using bottlenoselabs.Interop;
using bottlenoselabs.SDL;
using Examples.MinecraftClone.ECS.Components;
using Examples.MinecraftClone.Engine;
using Examples.MinecraftClone.Engine.ECS;

namespace Examples.MinecraftClone.ECS.Systems.Draw;

public sealed class RenderSystem : System<CamerDrawComponent>
{
    private readonly Window _window;
    private readonly GpuDevice _device;
    private readonly FileSystem _fileSystem;

    private GpuGraphicsPipeline? _pipeline;
    private GpuDataBuffer? _vertexBuffer;
    private GpuDataBuffer? _indexBuffer;

    private bool _canRender;

    public RenderSystem(
        World world,
        Window window,
        FileSystem fileSystem,
        GpuDevice device)
        : base()
    {
        _window = window;
        _fileSystem = fileSystem;
        _device = device;
    }

    public override void OnStart()
    {
        if (!TryCreatePipeline())
        {
            _canRender = false;
            return;
        }

        if (!TryCreateVertexBuffer())
        {
            _canRender = false;
            return;
        }

        if (!TryCreateIndexBuffer())
        {
            _canRender = false;
            return;
        }

        _canRender = true;
    }

    public override void OnStop()
    {
        _pipeline?.Dispose();
        _pipeline = null!;

        _vertexBuffer?.Dispose();
        _vertexBuffer = null!;

        _indexBuffer?.Dispose();
        _indexBuffer = null!;
    }

    protected override void OnExecute(
        in Entity entity,
        ref CamerDrawComponent camera)
    {
        if (!_canRender)
        {
            return;
        }

        var commandBuffer = _device.GetCommandBuffer();
        if (!commandBuffer.TryGetSwapchainTexture(_window, out var swapchainTexture))
        {
            commandBuffer.Cancel();
            return;
        }

        var renderTargetInfoColor = default(GpuRenderTargetInfoColor);
        renderTargetInfoColor.Texture = swapchainTexture;
        renderTargetInfoColor.ClearColor = Rgba32F.CornflowerBlue;
        renderTargetInfoColor.LoadOperation = GpuRenderTargetLoadOperation.Clear;
        renderTargetInfoColor.StoreOp = GpuRenderTargetStoreOp.Store;
        var renderPass = commandBuffer.BeginRenderPass(null, renderTargetInfoColor);

        renderPass.BindPipeline(_pipeline);
        renderPass.BindVertexBuffer(_vertexBuffer);
        renderPass.BindIndexBuffer(_indexBuffer);

        var modelMatrix = Matrix4x4.Identity;
        var modelViewProjectionMatrix = modelMatrix * camera.ViewProjectionMatrix;
        commandBuffer.PushVertexShaderUniformMatrix(modelViewProjectionMatrix);
        commandBuffer.PushFragmentShaderUniformColor(Rgba32F.White);
        renderPass.DrawPrimitivesIndexed(36, 1, 0, 0, 0);

        renderPass.End();
    }

    private bool TryCreatePipeline()
    {
        var vertexShaderFilePath = Assets.Utility.GetShaderFilePath("Minecraft.vert");
        if (!_fileSystem.TryLoadGraphicsShader(
                vertexShaderFilePath, _device, out var vertexShader, uniformBufferCount: 1))
        {
            return false;
        }

        var fragmentShaderFilePath = Assets.Utility.GetShaderFilePath("Minecraft.frag");
        if (!_fileSystem.TryLoadGraphicsShader(
                fragmentShaderFilePath, _device, out var fragmentShader, uniformBufferCount: 1))
        {
            return false;
        }

        using var pipelineDescriptor = new GpuGraphicsPipelineOptions();
        pipelineDescriptor.PrimitiveType = GpuGraphicsPipelineVertexPrimitiveType.TriangleList;
        pipelineDescriptor.VertexShader = vertexShader;
        pipelineDescriptor.FragmentShader = fragmentShader;
        pipelineDescriptor.RasterizerState.FillMode = GpuGraphicsPipelineFillMode.Fill;
        pipelineDescriptor.RasterizerState.CullMode = GpuGraphicsPipelineCullMode.Back;
        pipelineDescriptor.SetVertexAttributes<Vertex>();
        pipelineDescriptor.SetVertexBufferDescription<Vertex>();
        pipelineDescriptor.SetRenderTargetColor(_window.Swapchain!);

        if (!_device.TryCreateGraphicsPipeline(pipelineDescriptor, out _pipeline))
        {
            Console.Error.WriteLine("Failed to create fill pipeline!");
            return false;
        }

        vertexShader?.Dispose();
        fragmentShader?.Dispose();

        return true;
    }

    private bool TryCreateVertexBuffer()
    {
        const int vertexCount = 24;
        var bufferSize = Unsafe.SizeOf<Vertex>() * vertexCount;

        if (!_device.TryCreateDataBuffer<Vertex>(vertexCount, out _vertexBuffer))
        {
            return false;
        }

        if (!_device.TryCreateUploadTransferBuffer(bufferSize, out var transferBuffer))
        {
            return false;
        }

        var span = transferBuffer!.MapAsSpan();
        var vertices = MemoryMarshal.Cast<byte, Vertex>(span);

        /*
         * NOTE: Model vertices of the cube using standard cartesian coordinate system.
         *      +Z is towards your eyes, -Z is towards the screen
         *      +X is to the right, -X to the left
         *      +Y is towards the sky (up), -Y is towards the ground (down)
         *              (-1, 1,-1)              (1, 1,-1)
         *                  +--------------------+
         *                 /|                   /|
         *               /  |                 /  |
         *             /    |               /    |
         *  (-1,-1, 1)+--------------------+ (1, -1, 1)
         *            |     |              |     |
         *            |     |              |     |
         *            |     | (-1,-1,-1)   |     |
         *            |     +--------------|----=+ (1,-1, -1)
         *            |    /               |    /
         *            |  /                 |  /
         *            |/                   |/
         *            +--------------------+
         *            (-1,-1, 1)           (1,-1, 1)
         *
         * NOTE: Each face of the cube is a rectangle (two triangles), each rectangle is 4 vertices.
         * NOTE: Preferred counter-clockwise vertices for front facing triangles:
         *      Triangle 1: 0 -> 1 -> 2
         *      Triangle 2: 0 -> 2 -> 3
         * 3 ----------- 2
         * |           / |
         * |         /   |
         * |       /     |
         * |     /       |
         * |   /         |
         * | /           |
         * 0 ----------- 1
         *
         * NOTE: Preferred clockwise vertices for back facing triangles:
         *      Triangle 1: 0 -> 1 -> 2
         *      Triangle 2: 0 -> 2 -> 3
         * 2 ----------- 3
         * | \           |
         * |   \         |
         * |     \       |
         * |       \     |
         * |         \   |
         * |           \ |
         * 1 ----------- 0
         */

        const float leftX = -1.0f;
        const float rightX = 1.0f;
        const float bottomY = -1.0f;
        const float topY = 1.0f;
        const float backZ = -1.0f;
        const float frontZ = 1.0f;

        var bottomLeft = new Vector2(leftX, bottomY);
        var bottomRight = new Vector2(rightX, bottomY);
        var topRight = new Vector2(rightX, topY);
        var topLeft = new Vector2(leftX, topY);

        var bottomLeftFront = new Vector3(bottomLeft, frontZ); // (-1, -1, 1)
        var bottomLeftBack = new Vector3(bottomLeft, backZ); // (-1, -1, -1)

        var bottomRightFront = new Vector3(bottomRight, frontZ); // (1, -1, 1)
        var bottomRightBack = new Vector3(bottomRight, backZ); // (1, -1, -1)

        var topLeftFront = new Vector3(topLeft, frontZ); // (-1, 1, 1)
        var topLeftBack = new Vector3(topLeft, backZ); // (-1, 1, -1)

        var topRightFront = new Vector3(topRight, frontZ); // (1, 1, 1)
        var topRightBack = new Vector3(topRight, backZ); // (1, 1, -1)

        // rectangle 1; front
        var color1 = Rgba8U.Red; // #FF0000
        vertices[0].Position = bottomLeftFront;
        vertices[0].Color = color1;
        vertices[1].Position = bottomRightFront;
        vertices[1].Color = color1;
        vertices[2].Position = topRightFront;
        vertices[2].Color = color1;
        vertices[3].Position = topLeftFront;
        vertices[3].Color = color1;

        // rectangle 2; back
        var color2 = Rgba8U.Lime; // NOTE: "lime" is #00FF00; "green" is actually #008000
        vertices[4].Position = bottomRightBack;
        vertices[4].Color = color2;
        vertices[5].Position = bottomLeftBack;
        vertices[5].Color = color2;
        vertices[6].Position = topLeftBack;
        vertices[6].Color = color2;
        vertices[7].Position = topRightBack;
        vertices[7].Color = color2;

        // rectangle 3; left
        var color3 = Rgba8U.Blue; // #0000FF
        vertices[8].Position = bottomLeftBack;
        vertices[8].Color = color3;
        vertices[9].Position = bottomLeftFront;
        vertices[9].Color = color3;
        vertices[10].Position = topLeftFront;
        vertices[10].Color = color3;
        vertices[11].Position = topLeftBack;
        vertices[11].Color = color3;

        // rectangle 4; right
        var color4 = Rgba8U.Yellow; // #FFFF00
        vertices[12].Position = bottomRightFront;
        vertices[12].Color = color4;
        vertices[13].Position = bottomRightBack;
        vertices[13].Color = color4;
        vertices[14].Position = topRightBack;
        vertices[14].Color = color4;
        vertices[15].Position = topRightFront;
        vertices[15].Color = color4;

        // rectangle 5; bottom
        var color5 = Rgba8U.Aqua; // #00FFFF
        vertices[16].Position = bottomLeftBack;
        vertices[16].Color = color5;
        vertices[17].Position = bottomRightBack;
        vertices[17].Color = color5;
        vertices[18].Position = bottomRightFront;
        vertices[18].Color = color5;
        vertices[19].Position = bottomLeftFront;
        vertices[19].Color = color5;

        // rectangle 6; top
        var color6 = Rgba8U.Fuchsia; // #FF00FF
        vertices[20].Position = topLeftFront;
        vertices[20].Color = color6;
        vertices[21].Position = topRightFront;
        vertices[21].Color = color6;
        vertices[22].Position = topRightBack;
        vertices[22].Color = color6;
        vertices[23].Position = topLeftBack;
        vertices[23].Color = color6;

        transferBuffer.Unmap();

        var uploadCommandBuffer = _device.GetCommandBuffer();
        var copyPass = uploadCommandBuffer.BeginCopyPass();

        copyPass.UploadToDataBuffer(
            transferBuffer,
            0,
            _vertexBuffer,
            0,
            bufferSize);

        copyPass.End();
        uploadCommandBuffer.Submit();

        copyPass.Dispose();
        transferBuffer.Dispose();

        return true;
    }

    private bool TryCreateIndexBuffer()
    {
        // NOTE: The indices for front and back facing triangles are the same because back facing triangles
        //      are specified by different vertex data (position).
        Span<ushort> indices =
        [
            0, 1, 2, 0, 2, 3, // rectangle 1 of cube, front, counter-clockwise, base vertex: 0
            4, 5, 6, 4, 6, 7, // rectangle 2 of cube, back, clockwise, base vertex: 4
            8, 9, 10, 8, 10, 11, // rectangle 3 of cube, left, counter-clockwise, base vertex: 8
            12, 13, 14, 12, 14, 15, // rectangle 4 of cube, right, clockwise, base vertex: 12
            16, 17, 18, 16, 18, 19, // rectangle 5 of cube, top, counter-clockwise, base vertex: 16
            20, 21, 22, 20, 22, 23 // rectangle 6 of cube, bottom, clockwise, base vertex: 20
        ];

        const int indexCount = 36;
        var bufferSize = Unsafe.SizeOf<ushort>() * indexCount;

        if (!_device.TryCreateDataBuffer<ushort>(indexCount, out _indexBuffer))
        {
            return false;
        }

        if (!_device.TryCreateUploadTransferBuffer(bufferSize, out var transferBuffer))
        {
            return false;
        }

        var span = transferBuffer!.MapAsSpan();
        var indicesMapped = MemoryMarshal.Cast<byte, ushort>(span);
        indices.CopyTo(indicesMapped);

        transferBuffer.Unmap();

        var uploadCommandBuffer = _device.GetCommandBuffer();
        var copyPass = uploadCommandBuffer.BeginCopyPass();

        copyPass.UploadToDataBuffer(
            transferBuffer,
            0,
            _indexBuffer,
            0,
            bufferSize);

        copyPass.End();
        uploadCommandBuffer.Submit();
        transferBuffer.Dispose();
        copyPass.Dispose();

        return true;
    }
}
