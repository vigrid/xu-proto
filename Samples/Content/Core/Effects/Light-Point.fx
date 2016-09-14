#include "common.fxh"

uniform float2 HalfPixel;

uniform extern float3 LightPosition;
uniform extern float4 LightColor;
uniform extern float LightIntensity;
uniform extern float LightRange;

uniform extern float4x4 ModelViewProjection;
uniform extern float4x4 ViewProjectionInverse;

uniform extern texture DepthMap;
uniform extern texture NormalMap;


sampler DepthSampler = sampler_state { Texture = (DepthMap); };
sampler NormalSampler = sampler_state { Texture = (NormalMap); };


struct Vertex
{
	float4 Position : POSITION0;
};

struct Transformed
{
	float4 Position  : POSITION0;
	float4 TPosition : TEXCOORD0;
};

Transformed MainVS(Vertex input)
{
	Transformed output = (Transformed) 0;
	
	float4 position = mul(input.Position, ModelViewProjection);

	output.Position = position;
	output.TPosition = position;
	
	return output;
}


float4 MainPS(Transformed input) : COLOR
{
	float3 posXYZ = input.TPosition.xyz / input.TPosition.w;
	float2 uv = float2(posXYZ.x, -posXYZ.y) * 0.5f + 0.5f;
	uv += HalfPixel;

	float depth = tex2D(DepthSampler, uv);
	if (depth >= posXYZ.z)
	{
		discard;
	}

	float3 normal = tex2D(NormalSampler, uv) * 2.0f - 1.0f;

	float4 posXYZW = float4(posXYZ.xy, depth, 1.0f);
	float4 worldPosition = mul(posXYZW, ViewProjectionInverse);
	worldPosition /= worldPosition.w;

	float3 lightVector = LightPosition - worldPosition.xyz;
	float3 lightDirection = normalize(lightVector);
	float dotProduct = dot(lightDirection, normalize(normal));
	if (dotProduct < 0.0f)
	{
		discard;
	}

	float lightDistance = length(lightVector);
	float falloff = saturate(1.0f - lightDistance / LightRange);
	float i = saturate(dotProduct) * LightIntensity * falloff * falloff;

	return LightColor * i;
}


technique DefaultTechnique
{
	pass p0
	{
		vertexShader = compile vs_4_0 MainVS();
		pixelShader = compile ps_4_0 MainPS();
	}
}
