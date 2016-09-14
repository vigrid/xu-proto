using Xu.Genres.VoxelBased;
using Xu.Genres.VoxelBased.WorldGeneration;
using Xu.Types;

namespace Sample02.WorldGeneration
{
	public class ResourceAwareVoxelClassifier : IVoxelClassifier
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

						VoxelData voxel = voxelData[index];

						BlockType type = BlockType.Empty;

						if (voxel.Density > 0.0f)
						{
							if (voxel.C == 0)
							{
								if (voxel.A == 1)
								{
									if (voxel.B == 0)
									{
										type = BlockType.Solid2;
									}
									else if (voxel.B == 1)
									{
										type = BlockType.Solid3;
									}
									else if (voxel.B == 2)
									{
										type = BlockType.Solid4;
									}
								}
								else
								{
									type = BlockType.Solid1;
								}
							}
							else
							{
								switch (voxel.C)
								{
									case 1:
										type = BlockType.Resource1;
										break;
									case 2:
										type = BlockType.Resource2;
										break;
									default:
										type = BlockType.Resource3;
										break;
								}
							}
						}

						destination[index].Type = type;
					}
				}
			}
		}

		#endregion
	}
}
