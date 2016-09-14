using System.Runtime.InteropServices;

namespace Xu.Genres.VoxelBased.WorldGeneration
{
	[StructLayout(LayoutKind.Explicit)]
	public struct VoxelData
	{
		[FieldOffset(0)] public float Density;
		[FieldOffset(4)] public byte A;
		[FieldOffset(5)] public byte B;
		[FieldOffset(6)] public byte C;
		[FieldOffset(7)] public byte D;
	}
}
