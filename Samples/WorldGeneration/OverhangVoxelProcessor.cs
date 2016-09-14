using Xu.Core;
using Xu.Generators;
using Xu.Genres.VoxelBased;
using Xu.Genres.VoxelBased.WorldGeneration;
using Xu.Types;

namespace Sample02.WorldGeneration
{
	public class OverhangVoxelProcessor : IVoxelProcessor
	{
		private const float HorizontalScale = 0.003f;
		private const float VerticalScale = 0.01f;
		private readonly PerlinNoise _noise;

		public OverhangVoxelProcessor()
		{
			_noise = new PerlinNoise();
			Initialize();
		}

		public OverhangVoxelProcessor(int seed)
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
						if (voxelData[index].A == 1)
						{
							voxelData[index].Density += XuMath.Clamp(_noise.Sample(fx, fy, fz) - fy * 0.25f, 0.0f, 1.0f);
						}
					}
				}
			}
		}

		#endregion

		private void Initialize()
		{
			Enabled = true;
			_noise.GenerateOctaves(3, 1.0f, 0.767f, 2.71f);
		}
	}
}
