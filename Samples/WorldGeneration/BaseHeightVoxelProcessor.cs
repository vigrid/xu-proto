using Xu.Core;
using Xu.Generators;
using Xu.Genres.VoxelBased;
using Xu.Genres.VoxelBased.WorldGeneration;
using Xu.Types;

namespace Sample02.WorldGeneration
{
	public class BaseHeightVoxelProcessor : IVoxelProcessor
	{
		private const float HorizontalScale = 0.005f;
		private const float VerticalScale = 0.02f;
		private readonly PerlinNoise _noise;

		public BaseHeightVoxelProcessor()
		{
			_noise = new PerlinNoise();
			Initialize();
		}

		public BaseHeightVoxelProcessor(int seed)
		{
			_noise = new PerlinNoise(seed);
			Initialize();
		}

		#region IVoxelProcessor Members

		public bool Enabled { get; set; }

		public void Process(IntVector3 chunkPosition, VoxelData[] voxelData)
		{
			IntVector3 position;
			for (position.X = 0; position.X < Chunk.ChunkSizeX; position.X++)
			{
				float fx = (chunkPosition.X * Chunk.ChunkSizeX + position.X) * HorizontalScale;

				for (position.Z = 0; position.Z < Chunk.ChunkSizeZ; position.Z++)
				{
					float fz = (chunkPosition.Z * Chunk.ChunkSizeZ + position.Z) * HorizontalScale;

					float density = _noise.Sample(fx, fz);

					for (position.Y = 0; position.Y < Chunk.ChunkSizeY; position.Y++)
					{
						float fy = (chunkPosition.Y * Chunk.ChunkSizeY + position.Y) * VerticalScale;

						voxelData[Chunk.BlockIndex(position)].Density = XuMath.Clamp(density - fy, -1.0f, 1.0f);
					}
				}
			}
		}

		#endregion

		private void Initialize()
		{
			Enabled = true;
			_noise.GenerateOctaves(3, 0.25f, 0.667f, 2.0f);
		}
	}
}
