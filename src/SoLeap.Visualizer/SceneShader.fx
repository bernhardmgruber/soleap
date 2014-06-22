#define MaxLights 4

struct VertexShaderInput 
{
    float3 position : POSITION;
    float3 normal : NORMAL;
};

struct VertexShaderOutput 
{
    float4 position : SV_POSITION;
    float3 worldPosition : TEXCOORD1;
    float3 normal : NORMAL;
};

struct DirectionalLight
{
    float3 Direction;
    float3 Color;
    bool Enabled;
};

cbuffer FrameConstantsBuffer 
    : register(b0) 
{
    float4x4 View;
    float4x4 Projection;
    float3 EyePosition;
    float3 AmbientLightColor;
    DirectionalLight Lights[MaxLights];
}

cbuffer ObjectConstantsBuffer 
    : register(b1)
{
    float4x4 World;
    float4x4 WorldInverseTranspose;
    float3 AmbientColor;
    float3 DiffuseColor;
    float3 SpecularColor;
    float SpecularPower;
}

float3 Phong(VertexShaderOutput input, float3 diffuseMaterialColor, float3 ambientMaterialColor, float3 specularMaterialColor, float specularMaterialPower) 
{
    float3 ambientColor = ambientMaterialColor * AmbientLightColor;

    float3 diffuseLightColor = 0.0f;
    float3 specularLightColor = 0.0f;

    for (int i = 0; i < MaxLights; i++) {
        if (!Lights[i].Enabled) continue;

        float3 lightDir = normalize(-Lights[i].Direction);

        float diffuseIntensity = saturate(dot(input.normal, lightDir));
        diffuseLightColor += diffuseMaterialColor * Lights[i].Color * diffuseIntensity;

        float3 reflection = reflect(lightDir, input.normal);
        float3 eyeDirection = normalize(input.worldPosition - EyePosition);
        float specularIntensity = saturate(dot(reflection, eyeDirection));
        specularIntensity = pow(specularIntensity, specularMaterialPower);
        specularLightColor += specularMaterialColor * Lights[i].Color * specularIntensity;
    }

    return saturate(ambientColor + diffuseLightColor + specularLightColor);
}

VertexShaderOutput VertexShaderMain(VertexShaderInput input) 
{
    VertexShaderOutput output;

    float4x4 viewProjection = mul(View, Projection);

    float4 worldPosition = mul(float4(input.position, 1.0f), World);

    output.position = mul(worldPosition, viewProjection);
    output.worldPosition = worldPosition;
    output.normal = mul(input.normal, WorldInverseTranspose);

    return output;
}

float4 PixelShaderMain(VertexShaderOutput input) : SV_TARGET
{
    float3 color = Phong(input, DiffuseColor, AmbientColor, SpecularColor, SpecularPower);

    return float4(color, 1.0f);
}