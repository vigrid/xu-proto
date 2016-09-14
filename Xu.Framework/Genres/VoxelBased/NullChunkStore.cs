using System;

namespace Xu.Genres.VoxelBased
{
	public class NullChunkStore : IChunkStore
	{
		#region IChunkStore Members

		public bool ContainsChunk(Chunk chunk)
		{
			return false;
		}

		public void LoadChunkData(Chunk chunk)
		{
			throw new NotSupportedException("NullChunkStore does not support chunk loading");
		}

		public void SaveChunkData(Chunk chunk)
		{
			// Intentionally empty
		}

		#endregion
	}
}
