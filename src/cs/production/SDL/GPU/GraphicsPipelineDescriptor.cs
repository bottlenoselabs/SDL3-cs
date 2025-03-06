// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.GPU;

/// <summary>
///     Parameters for creating a <see cref="GraphicsPipeline" /> instance.
/// </summary>
[PublicAPI]
public class GraphicsPipelineDescriptor : Descriptor
{
    /// <summary>
    ///     Gets or sets the <see cref="GraphicsShader" /> instance used in the per-vertex stage of the graphics
    ///     pipeline.
    /// </summary>
    public GraphicsShader? VertexShader { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="GraphicsShader" /> instance used in the per-fragment (per-pixel) stage of the
    ///     graphics pipeline.
    /// </summary>
    public GraphicsShader? FragmentShader { get; set; }

    /// <summary>
    ///     The <see cref="GraphicsPipelineVertexInputState" /> used in per-vertex stage of the graphics pipeline.
    /// </summary>
    public GraphicsPipelineVertexInputState VertexInputState;

    /// <summary>
    ///     Gets or sets <see cref="GraphicsPipelineVertexPrimitiveType" /> used in multiple stages of the graphics
    ///     pipeline.
    /// </summary>
    public GraphicsPipelineVertexPrimitiveType PrimitiveType { get; set; }

    /// <summary>
    ///     Gets or sets the configuration for the rasterization stage of the graphics pipeline.
    /// </summary>
    public GraphicsPipelineRasterizerState RasterizerState { get; set; } = new();

    /// <summary>
    ///     The multi-sample state of the graphics pipeline.
    /// </summary>
    public SDL_GPUMultisampleState MultiSampleState;

    /// <summary>
    ///     Gets or sets the depth-stencil state of the graphics pipeline.
    /// </summary>
    public GraphicsPipelineDepthStencilState DepthStencilState { get; set; } = new();

    /// <summary>
    ///     Gets or sets the texel format of the depth-stencil render-target. Ignored if
    ///     <see cref="IsEnabledDepthStencilRenderTarget" /> is <c>false</c>.
    /// </summary>
    public TextureFormat DepthStencilRenderTargetFormat { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the graphics pipeline has a depth-stencil render-target.
    /// </summary>
    public bool IsEnabledDepthStencilRenderTarget { get; set; }

    /// <summary>
    ///     Gets the span of color render-target descriptions.
    /// </summary>
    public ImmutableArray<GraphicsPipelineColorRenderTargetDescription> ColorRenderTargets { get; private set; } = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="GraphicsPipelineDescriptor" /> class.
    /// </summary>
    /// <param name="allocator">
    ///     The allocator to use for temporary interoperability allocations. If <c>null</c>, a
    ///     <see cref="ArenaNativeAllocator" /> is used with a capacity of 1 KB.
    /// </param>
    public GraphicsPipelineDescriptor(
        INativeAllocator? allocator = null)
        : base(allocator)
    {
    }

    /// <summary>
    ///     Sets the graphics pipeline to use a single render-target as color format of the swapchain.
    /// </summary>
    /// <param name="swapchain">The swapchain.</param>
    public void SetRenderTargetColor(Swapchain swapchain)
    {
        var colorRenderTarget = new GraphicsPipelineColorRenderTargetDescription();
        colorRenderTarget.Format = swapchain.TextureFormat;
        ColorRenderTargets = [colorRenderTarget];
    }

    /// <summary>
    ///     Sets <see cref="VertexInputState" /> with the vertex data attributes that controls how vertices are fetched
    ///     from one or more vertex buffers and mapped into the per-vertex stage of the graphics pipeline.
    /// </summary>
    /// <typeparam name="TVertex">The type of vertex.</typeparam>
    /// <exception cref="ArgumentException"><typeparamref name="TVertex" /> has no fields.</exception>
    public unsafe void SetVertexAttributes<TVertex>()
        where TVertex : unmanaged
    {
        var vertexType = typeof(TVertex);
        var vertexFields = vertexType.GetFields().ToImmutableArray();

        if (vertexFields.IsDefaultOrEmpty)
        {
            throw new ArgumentException("Vertex type has no fields.");
        }

        ref var vertexInputState = ref VertexInputState;
        vertexInputState.VertexAttributeCount = vertexFields.Length;
        var vertexAttributes = Allocator.AllocateArray<SDL_GPUVertexAttribute>(vertexFields.Length);
        vertexInputState.VertexAttributesArrayPointer = (IntPtr)vertexAttributes;

        for (var i = 0; i < vertexFields.Length; i++)
        {
            var vertexField = vertexFields[i];
            ref var vertexAttribute = ref vertexAttributes[i];
            vertexAttribute.location = (uint)i;

            if (vertexField.FieldType == typeof(Vector2))
            {
                vertexAttribute.format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT2;
            }
            else if (vertexField.FieldType == typeof(Vector3))
            {
                vertexAttribute.format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT3;
            }
            else if (vertexField.FieldType == typeof(Rgba8U))
            {
                vertexAttribute.format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_UBYTE4_NORM;
            }
            else
            {
                throw new NotImplementedException();
            }

            vertexAttribute.offset = (uint)Marshal.OffsetOf(vertexType, vertexField.Name);
        }
    }

    /// <summary>
    ///     Sets <see cref="VertexInputState" /> with layout information of a single vertex buffer that controls how
    ///     vertices are fetched and mapped into the per-vertex stage of the graphics pipeline.
    /// </summary>
    /// <typeparam name="TVertex">The type of vertex.</typeparam>
    public unsafe void SetVertexBufferDescription<TVertex>()
        where TVertex : unmanaged
    {
        ref var vertexInputState = ref VertexInputState;
        vertexInputState.VertexBufferDescriptionCount = 1;
        var vertexBufferDescriptions = Allocator
            .AllocateArray<SDL_GPUVertexBufferDescription>(1);
        vertexInputState.VertexBufferDescriptionsArrayPointer = (IntPtr)vertexBufferDescriptions;

        ref var vertexBufferDescription = ref vertexBufferDescriptions[0];
        vertexBufferDescription.slot = 0;
        vertexBufferDescription.input_rate = SDL_GPUVertexInputRate.SDL_GPU_VERTEXINPUTRATE_VERTEX;
        vertexBufferDescription.instance_step_rate = 0;
        vertexBufferDescription.pitch = (uint)sizeof(TVertex);
    }

    /// <inheritdoc />
    protected override void OnReset()
    {
        VertexShader = null;
        FragmentShader = null;
        VertexInputState = default;
        PrimitiveType = GraphicsPipelineVertexPrimitiveType.TriangleList;
        RasterizerState.Reset();
        MultiSampleState = default;
        DepthStencilState.Reset();
        IsEnabledDepthStencilRenderTarget = false;
        DepthStencilRenderTargetFormat = TextureFormat.Invalid;

        foreach (var colorRenderTarget in ColorRenderTargets)
        {
            colorRenderTarget.Reset();
        }

        ColorRenderTargets = [];
    }
}
