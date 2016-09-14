using Xu.Generators;
using Xu.Types;

namespace Xu.Genres.VoxelBased.WorldGeneration
{
	public class BasicVoxelProcessor : IVoxelProcessor
	{
		private const float Scale = 0.01f;
		private readonly PerlinNoise _density;

		public BasicVoxelProcessor()
		{
			_density = new PerlinNoise();
			Initialize();
		}

		public BasicVoxelProcessor(int seed)
		{
			_density = new PerlinNoise(seed);
			Initialize();
		}

		#region IVoxelProcessor Members

		public bool Enabled { get; set; }

		public void Process(IntVector3 chunkPosition, VoxelData[] voxelData)
		{
			IntVector3 position;
			for (position.X = 0; position.X < Chunk.ChunkSizeX; position.X++)
			{
				float fx = (chunkPosition.X * Chunk.ChunkSizeX + position.X) * Scale;

				for (position.Z = 0; position.Z < Chunk.ChunkSizeZ; position.Z++)
				{
					float fz = (chunkPosition.Z * Chunk.ChunkSizeZ + position.Z) * Scale;

					for (position.Y = 0; position.Y < Chunk.ChunkSizeY; position.Y++)
					{
						float fy = (chunkPosition.Y * Chunk.ChunkSizeY + position.Y) * Scale;

						float density = _density.Sample(fx, fy, fz);

						voxelData[Chunk.BlockIndex(position)].Density = density - fy;
					}
				}
			}
		}

		#endregion

		private void Initialize()
		{
			Enabled = true;
			_density.GenerateOctaves(4, 1.0f, 0.667f, 2.0f);
		}
	}
}
