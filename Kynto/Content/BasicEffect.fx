cbuffer PerFrame
{
    float4x4 WVP; // WorldViewProjection
};
cbuffer PerMaterial
{
    float3 MatDiffuse = { 1.0f, 1.0f, 1.0f };
    float Alpha = 1.0f;
};

Texture2D DiffuseMap;
SamplerState MapSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
    AddressW = Wrap;
};

struct VSInputTx
{
    float3 Position : POSITION;
    float2 TexCoord : TEXCOORD;
};

struct VSInputVc
{
    float3 Position : POSITION;
    float4 VertColor : COLOR;
};

struct VSOutputTx
{
    float4 PositionPS : SV_POSITION;
    float2 TexCoord : TEXCOORD;
};

VSOutputTx VS_Standard_Texture(VSInputTx input)
{
    VSOutputTx output;
    output.PositionPS = mul(float4(input.Position, 1.0), WVP);
    output.TexCoord = input.TexCoord;

    return output;
}

float4 PS_Standard_Texture(VSOutputTx input) : SV_TARGET
{
    float4 albedo = DiffuseMap.Sample(MapSampler, input.TexCoord);

    return float4(MatDiffuse * albedo.rgb, Alpha * albedo.a);
}

technique11 BasicEffect
{
    pass
    {
        SetVertexShader(CompileShader(vs_5_0, VS_Standard_Texture()));
        SetPixelShader(CompileShader(ps_5_0, PS_Standard_Texture()));
    }
}