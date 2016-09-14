namespace Xu.Genres.VoxelBased
{
	public interface IChunkStore
	{
		bool ContainsChunk(Chunk chunk);
		void LoadChunkData(Chunk chunk);
		void SaveChunkData(Chunk chunk);
	}
}
