cbuffer PerFrame
{
    float4x4 WVP; // WorldViewProjection
};

cbuffer PerMaterial
{
    float3 MatDiffuse = { 1.0f, 1.0f, 1.0f };
    float Alpha = 1.0f;
};

float4 VS_Standard_Color(float3 Position : POSITION) : SV_POSITION
{
    return mul(float4(Position, 1.0f), WVP);
}

float4 PS_Standard_Color() : SV_TARGET
{
    return float4(MatDiffuse, Alpha);
}

technique11 BasicEffect
{
    pass
    {
        SetVertexShader(CompileShader(vs_5_0, VS_Standard_Color()));
        SetPixelShader(CompileShader(ps_5_0, PS_Standard_Color()));
    }
}