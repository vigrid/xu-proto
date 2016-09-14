using Xu.Types;

namespace Xu.Genres.VoxelBased
{
	public interface IChunkGenerator
	{
		void GenerateBlocks(IntVector3 chunkPosition, Block[] destination);
	}
}
