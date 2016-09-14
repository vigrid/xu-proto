#include "common.fxh"


uniform extern float TextureFactor;

uniform extern float2 TileSize;

uniform extern float4x4 WorldViewProjection;
uniform extern float4x4 WorldInverseTranspose;

uniform extern texture ColorMap;

sampler ColorSampler = sampler_state { Texture = (ColorMap); };


struct Vertex
{
	float4 Position : POSITION0;
	float4 Normal   : NORMAL0;
	float2 UV       : TEXCOORD0;
};

struct Transformed
{
	float4 Position  : POSITION0;
	float3 Normal    : TEXCOORD0;
	float2 UV        : TEXCOORD1;
	float4 TPosition : TEXCOORD2;
	float4 WPosition : TEXCOORD3;
};

Transformed MainVS(Vertex input)
{
	Transformed output = (Transformed) 0;
	
	output.Position = mul(input.Position, WorldViewProjection);
	output.Normal = mul(input.Normal, WorldInverseTranspose);
	output.UV = input.UV;
	output.TPosition = output.Position;
	output.WPosition = input.Position;
	
	return output;
}


float2 ExtractUV(float2 corner, float3 position, float3 normal)
{
	float3 relPosition = position - floor(position);
	float2 offset;

	if (abs(normal.z) > 0.9f)
	{
		return corner + relPosition.xy * TileSize;
	}

	if (abs(normal.y) > 0.9f)
	{
		return corner + relPosition.xz * TileSize;
	}

	return corner + relPosition.yz * TileSize;
}


GBufferTarget MainPS(Transformed input)
{
	GBufferTarget output = (GBufferTarget) 0;
	
	float3 position = input.TPosition.xyz / input.TPosition.w;

	float2 uv = ExtractUV(input.UV, input.WPosition, input.Normal);

	output.Albedo = lerp(float4(1.0f, 1.0f, 1.0f, 1.0f), tex2D(ColorSampler, uv), TextureFactor);
	output.Normal = EncodeNormal(input.Normal);
	output.Depth = position.z;

	return output;
}


technique DefaultTechnique
{
	pass p0
	{
		vertexShader = compile vs_4_0 MainVS();
		pixelShader = compile ps_4_0 MainPS();
	}
}
