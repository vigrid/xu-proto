uniform extern float Intensity;
uniform extern float Size;

uniform extern float2 NoiseOffset;

uniform extern float4x4 ViewProjection;
uniform extern float4x4 ViewProjectionInverse;

uniform extern texture DepthMap;
uniform extern texture NormalMap;
uniform extern texture RandomMap;


sampler DepthSampler = sampler_state { Texture = (DepthMap); MipFilter = Point; MinFilter = Point; MagFilter = Point; AddressU = Clamp; AddressV = Clamp; AddressW = Clamp; };
sampler NormalSampler = sampler_state { Texture = (NormalMap); MipFilter = Point; MinFilter = Point; MagFilter = Point; AddressU = Clamp; AddressV = Clamp; AddressW = Clamp; };
sampler RandomSampler = sampler_state { Texture = (RandomMap); MipFilter = Point; MinFilter = Point; MagFilter = Point; };


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

float3 GetXYZ(float2 uv)
{
	float depth = tex2D(DepthSampler, uv).x;
	float2 xy;
	xy.x = uv.x * 2.0f - 1.0f;
	xy.y = uv.y * 2.0f - 1.0f;
	xy.y *= -1;

	float4 p = float4(xy, depth, 1);
	float4 q = mul(p, ViewProjectionInverse);
	return q.xyz / q.w;
}


float3 GetNormal(float2 uv)
{
	return (tex2D(NormalSampler, uv) * 2.0f - 1.0f).xyz;
}


float4 MainPS(Transformed input) : COLOR
{
	float3 samples[16] =
	{
		float3(0.01537562, 0.01389096, 0.02276565),
		float3(-0.0332658, -0.2151698, -0.0660736),
		float3(-0.06420016, -0.1919067, 0.5329634),
		float3(-0.05896204, -0.04509097, -0.03611697),
		float3(-0.1302175, 0.01034653, 0.01543675),
		float3(0.3168565, -0.182557, -0.01421785),
		float3(-0.02134448, -0.1056605, 0.00576055),
		float3(-0.3502164, 0.281433, -0.2245609),
		float3(-0.00123525, 0.00151868, 0.02614773),
		float3(0.1814744, 0.05798516, -0.02362876),
		float3(0.07945167, -0.08302628, 0.4423518),
		float3(0.321987, -0.05670302, -0.05418307),
		float3(-0.00165138, -0.00410309, 0.00537362),
		float3(0.01687791, 0.03189049, -0.04060405),
		float3(-0.04335613, -0.00530749, 0.06443053),
		float3(0.8474263, -0.3590308, -0.02318038),
	};

	float depth = tex2D(DepthSampler, input.TexCoord);
	float3 position = GetXYZ(input.TexCoord);
	float3 normal = GetNormal(input.TexCoord);

	float occlusion = 1.0f;

	float3 reflectionRay = tex2D(RandomSampler, input.TexCoord + NoiseOffset) * 2.0f - 1.0f;

	for (int i = 0; i < 16; i++)
	{
		float3 sampleXYZ = position + reflect(samples[i], reflectionRay) * Size;

		float4 screenXYZW = mul(float4(sampleXYZ, 1.0f), ViewProjection);
		float3 screenXYZ = screenXYZW.xyz / screenXYZW.w;
		float2 sampleUV = float2(screenXYZ.x * 0.5f + 0.5f, 1.0f - (screenXYZ.y * 0.5f + 0.5f));

		float frontMostDepthAtSample = tex2D(DepthSampler, sampleUV).x;

		if (frontMostDepthAtSample < screenXYZ.z)
		{
			occlusion -= 1.0f / 16.0f;
		}
	}

	return float4(occlusion * Intensity * float3(1.0f, 1.0f, 1.0f), 1.0f);
}


technique DefaultTechnique
{
	pass p0
	{
		vertexShader = compile vs_4_0 MainVS();
		pixelShader = compile ps_4_0 MainPS();
	}
}
