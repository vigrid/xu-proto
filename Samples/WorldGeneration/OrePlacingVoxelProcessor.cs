using Xu.Core;
using Xu.Generators;
using Xu.Genres.VoxelBased;
using Xu.Genres.VoxelBased.WorldGeneration;
using Xu.Types;

namespace Sample02.WorldGeneration
{
	public class OrePlacingVoxelProcessor : IVoxelProcessor
	{
		private const float HorizontalScale = 0.01f;
		private const float VerticalScale = 0.01f;
		private readonly PerlinNoise _noise;

		public OrePlacingVoxelProcessor()
		{
			_noise = new PerlinNoise();
			Initialize();
		}

		public OrePlacingVoxelProcessor(int seed)
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

					for (position.Y = 0; position.Y < Chunk.ChunkSizeY; position.Y++)
					{
						float fy = (chunkPosition.Y * Chunk.ChunkSizeY + position.Y) * VerticalScale;

						var index = Chunk.BlockIndex(position);
						if (voxelData[index].Density > 0.0f && voxelData[index].A != 0)
						{
							var sample = _noise.Sample(fx, fy, fz);
							if (sample < -0.55f)
							{
								voxelData[index].C = 1;
							}
							if (sample > 0.65f)
							{
								voxelData[index].C = 2;
							}
						}
					}
				}
			}
		}

		#endregion

		private void Initialize()
		{
			Enabled = true;
			_noise.GenerateOctaves(3, 1.0f, 1.0f, 4.71f);
		}
	}
}
