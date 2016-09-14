uniform extern float3 LightDirection;
uniform extern float4 LightColor;
uniform extern float LightIntensity;

uniform extern float4x4 ViewProjectionInverse;

uniform extern texture DepthMap;
uniform extern texture NormalMap;


sampler DepthSampler = sampler_state { Texture = (DepthMap); };
sampler NormalSampler = sampler_state { Texture = (NormalMap); };


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
	float depth = tex2D(DepthSampler, input.TexCoord).x;
	float3 normal = (tex2D(NormalSampler, input.TexCoord) * 2.0f - 1.0f).xyz;

	float i = saturate(dot(LightDirection, normal)) * LightIntensity;

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
