using System;
using Xu.Core;
using Xu.Generators;
using Xu.Genres.VoxelBased;
using Xu.Genres.VoxelBased.WorldGeneration;
using Xu.Types;

namespace Sample02.WorldGeneration
{
	public class CanyonCarvingVoxelProcessor : IVoxelProcessor
	{
		private const float Scale = 0.004f;
		private readonly PerlinNoise _noise;

		public CanyonCarvingVoxelProcessor()
		{
			_noise = new PerlinNoise();
			Initialize();
		}

		public CanyonCarvingVoxelProcessor(int seed)
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

					var sample = _noise.Sample(fx, fz);
					float canyonEffect = XuMath.Clamp(1.0f - 1.0f / (float) Math.Pow(sample * 6.0, -4.0), 0.1f, 1.0f);

					for (position.Y = 0; position.Y < Chunk.ChunkSizeY; position.Y++)
					{
						var index = Chunk.BlockIndex(position);
						var density = voxelData[index].Density;
						density -= canyonEffect;
						if (canyonEffect > 0.2f)
						{
							voxelData[index].A = 1;
						}
						if (canyonEffect > 0.4f)
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
