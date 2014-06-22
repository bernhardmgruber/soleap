struct VertexShaderInput {
    float3 position : POSITION;
    float3 normal : NORMAL;
};

struct VertexShaderOutput {
    float4 position : SV_POSITION;
    float3 normal : NORMAL;
    float3 color : COLOR;
};

cbuffer FrameConstantsBuffer : register(b0) {
    float4x4 View;
    float4x4 Projection;
}

cbuffer ObjectConstantsBuffer : register(b1) {
    float4x4 World;
    float3 Color;
}

VertexShaderOutput VertexShaderMain(VertexShaderInput input) {
    VertexShaderOutput output;

    output.position = mul(mul(mul(float4(input.position, 1), World), View), Projection);
    output.normal = input.normal;
    output.color = Color;

    return output;
}

float4 PixelShaderMain(VertexShaderOutput input) : SV_TARGET
{
    return float4(input.color, 1.0f);
}