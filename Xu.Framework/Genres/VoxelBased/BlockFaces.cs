using System;

namespace Xu.Genres.VoxelBased
{
	[Flags]
	public enum BlockFaces : byte
	{
		None = 0x00,
		Left = 0x01,
		Right = 0x02,
		Up = 0x04,
		Down = 0x08,
		Backward = 0x10,
		Forward = 0x20,
		All = 0x3f,
		Unknown = 0x80,
	}
}
