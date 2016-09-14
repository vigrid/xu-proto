#include "common.fxh"


uniform extern float4x4 ViewProjectionInverse;

uniform extern texture ColorMap;
uniform extern texture LightMap;


sampler ColorSampler = sampler_state { Texture = (ColorMap); };
sampler LightSampler = sampler_state { Texture = (LightMap); };


struct Vertex
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct Transformed
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

Transformed MainVS(Vertex input)
{
	Transformed output = (Transformed) 0;
	
	output.Position = input.Position;
	output.TexCoord = input.TexCoord;
	
	return output;
}


float4 MainPS(Transformed input) : COLOR
{
	float4 color = tex2D(ColorSampler, input.TexCoord);
	float4 light = tex2D(LightSampler, input.TexCoord);

	return color * light;
}


technique DefaultTechnique
{
	pass p0
	{
		vertexShader = compile vs_4_0 MainVS();
		pixelShader = compile ps_4_0 MainPS();
	}
}
