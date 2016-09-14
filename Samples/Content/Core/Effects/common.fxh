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


/*
GBufferTarget MainPS(Transformed input)
{
	GBufferTarget output = (GBufferTarget) 0;
	
	float depth = input.T_Position.z / input.T_Position.w;

	float2 scr_xy = input.ScreenPosition.xy / input.ScreenPosition.w;
	float4 scr_xyzw = float4(scr_xy, depth, 1);
	float4 pos = mul(scr_xyzw, ViewProjectionInverse);
	pos /= pos.w;
	
	float4 original = input.WorldPosition;
	float4 restored = pos;
	
	float4 gray = float4(0.5f, 0.5f, 0.5f, 1.0f);
	output.Depth = gray + original - restored;
	
	return output;
}
*/
