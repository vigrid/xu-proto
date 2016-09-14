#include "common.fx"


float4 ClearColor = { 0.0f, 0.0f, 0.0f, 0.0f };


struct Vertex
{
	float4 Position : POSITION0;
};

struct Transformed
{
	float4 Position : POSITION0;
};

Transformed MainVS(Vertex input)
{
	Transformed output = (Transformed) 0;
	
	output.Position = input.Position;
	
	return output;
}


GBufferTarget MainPS(Transformed input)
{
	GBufferTarget output;
	
	output.Albedo = ClearColor;
	output.Normal = float4(0.5f, 0.5f, 0.5f, 0.0f);
	output.Depth  = float4(1.0f, 0.0f, 0.0f, 0.0f);
	
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
