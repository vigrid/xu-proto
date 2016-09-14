using Xu.Types;

namespace Xu.Genres.VoxelBased.WorldGeneration
{
	public class BasicVoxelClassifier : IVoxelClassifier
	{
		#region IVoxelClassifier Members

		public void Classify(IntVector3 chunkPosition, VoxelData[] voxelData, Block[] destination)
		{
			IntVector3 position;
			for (position.X = 0; position.X < Chunk.ChunkSizeX; position.X++)
			{
				for (position.Z = 0; position.Z < Chunk.ChunkSizeZ; position.Z++)
				{
					for (position.Y = 0; position.Y < Chunk.ChunkSizeY; position.Y++)
					{
						int index = Chunk.BlockIndex(position);

						destination[index].Type = voxelData[index].Density > 0.0f ? BlockType.Solid1 : BlockType.Empty;
					}
				}
			}
		}

		#endregion
	}
}
