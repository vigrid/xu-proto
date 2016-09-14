using Xu.Types;

namespace Xu.Genres.VoxelBased.WorldGeneration
{
	public interface IVoxelClassifier
	{
		void Classify(IntVector3 chunkPosition, VoxelData[] voxelData, Block[] destination);
	}
}
