// Transformation
uniform extern float4x4 World : WORLD;
uniform extern float4x4 View : VIEW;
uniform extern float4x4 Projection : PROJECTION;
uniform extern float4x4 WorldInverseTranspose : WORLDINVERSETRANSPOSE;

// Ambient
uniform extern float4 AmbientColor;
uniform extern float AmbientIntensity;

// Diffuse Term
uniform extern float3 DiffuseDirection;
uniform extern float4 DiffuseColor;
uniform extern float DiffuseIntensity;

// Fog
uniform extern float4 FogColor;
uniform extern float FogDepth;

// Texture
uniform extern texture2D Texture;
uniform extern sampler TextureSampler = sampler_state { Texture = (Texture); };


struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float2 TexCoord : TEXCOORD0;
};


struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float4 Normal : TEXCOORD1;
	float Depth : TEXCOORD2;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.TexCoord = input.TexCoord;
	output.Normal = mul(input.Normal, WorldInverseTranspose);
	output.Depth = output.Position.z;

	return output;
}


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 ambientTerm = AmbientColor * AmbientIntensity;
	float4 diffuseTerm = DiffuseIntensity * DiffuseColor * saturate(dot(DiffuseDirection, input.Normal));
	float4 textureTerm = tex2D(TextureSampler, input.TexCoord);

	float4 color = textureTerm * (ambientTerm + diffuseTerm);

	return lerp(color, FogColor, saturate(input.Depth / FogDepth));
}


Technique Default
{
	pass Full
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}
