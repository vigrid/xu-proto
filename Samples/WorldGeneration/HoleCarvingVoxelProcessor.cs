using Xu.Core;
using Xu.Generators;
using Xu.Genres.VoxelBased;
using Xu.Genres.VoxelBased.WorldGeneration;
using Xu.Types;

namespace Sample02.WorldGeneration
{
	public class HoleCarvingVoxelProcessor : IVoxelProcessor
	{
		private const float Scale = 0.05f;
		private readonly PerlinNoise _noise;

		public HoleCarvingVoxelProcessor()
		{
			_noise = new PerlinNoise();
			Initialize();
		}

		public HoleCarvingVoxelProcessor(int seed)
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
				float fx = (chunkPosition.X * Chunk.ChunkSizeX + position.X) * Scale;

				for (position.Z = 0; position.Z < Chunk.ChunkSizeZ; position.Z++)
				{
					float fz = (chunkPosition.Z * Chunk.ChunkSizeZ + position.Z) * Scale;

					for (position.Y = 0; position.Y < Chunk.ChunkSizeY; position.Y++)
					{
						float fy = (chunkPosition.Y * Chunk.ChunkSizeY + position.Y) * Scale;

						var index = Chunk.BlockIndex(position);
						var density = voxelData[index].Density;
						var effect = 0.2f - XuMath.Clamp(_noise.Sample(fx, fy, fz) * 2.0f, 0.1f, 1.0f);
						density += effect;
						if (effect > -0.1f)
						{
							voxelData[index].B = 1;
						}
						if (effect < -0.5f)
						{
							voxelData[index].B = 2;
						}
						voxelData[index].Density = density;
					}
				}
			}
		}

		#endregion

		private void Initialize()
		{
			Enabled = true;
			_noise.GenerateOctaves(3, 1.0f, 0.667f, 2.0f);
		}
	}
}
