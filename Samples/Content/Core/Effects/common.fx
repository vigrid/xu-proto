struct GBufferTarget
{
	half4 Albedo : COLOR0; // 8 bit R, 8 bit G, 8 bit G, 8 bit unused
	half4 Normal : COLOR1; // 8 bit X, 8 bit Y, 8 bit Z, 8 bit unused
	half4 Depth  : COLOR2; // 32 bit depth
};

float4 EncodeNormal(float3 normal)
{
	return float4(normal.xyz * 0.5f + 0.5f, 0.0f);
}
