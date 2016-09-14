using Xu.Types;

namespace Xu.Genres.VoxelBased.WorldGeneration
{
	public interface IVoxelProcessor
	{
		bool Enabled { get; set; }
		void Process(IntVector3 chunkPosition, VoxelData[] voxelData);
	}
}
