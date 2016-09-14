using System.Runtime.InteropServices;

namespace Xu.Genres.VoxelBased
{
	[StructLayout(LayoutKind.Explicit)]
	public struct Block
	{
		public static readonly Block Null;

		[FieldOffset(0)] public BlockType Type;

		public bool IsCollidable
		{
			get { return Type != BlockType.Empty; }
		}
	}
}
