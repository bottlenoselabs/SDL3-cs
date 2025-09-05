cbuffer UniformBlock : register(b0, space1)
{
    float4x4 MatrixModelViewProjection : packoffset(c0);
};

struct Input
{
    float4 Position : TEXCOORD0;
    float4 Color : COLOR1;
};

struct Output
{
    float4 Color : COLOR0;
    float4 Position : SV_Position;
};

Output main(Input input)
{
    Output output;
    output.Color = input.Color;
    output.Position = mul(MatrixModelViewProjection, input.Position);
    return output;
}
