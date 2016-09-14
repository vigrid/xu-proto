using Xu.Generators;
using Xu.Types;

namespace Xu.Genres.VoxelBased
{
	public class BasicChunkGenerator : IChunkGenerator
	{
		private const float Scale = 0.01f;
		private readonly PerlinNoise _density;

		public BasicChunkGenerator()
		{
			_density = new PerlinNoise();
			Initialize();
		}

		public BasicChunkGenerator(int seed)
		{
			_density = new PerlinNoise(seed);
			Initialize();
		}

		#region IChunkGenerator Members

		public void GenerateBlocks(IntVector3 chunkPosition, Block[] destination)
		{
			for (int x = 0; x < Chunk.ChunkSizeX; x++)
			{
				float fx = (chunkPosition.X * Chunk.ChunkSizeX + x) * Scale;

				for (int z = 0; z < Chunk.ChunkSizeZ; z++)
				{
					float fz = (chunkPosition.Z * Chunk.ChunkSizeZ + z) * Scale;

					for (int y = 0; y < Chunk.ChunkSizeY; y++)
					{
						float fy = (chunkPosition.Y * Chunk.ChunkSizeY + y) * Scale;

						float density = _density.Sample(fx, fy, fz);

						destination[Chunk.BlockIndex(x, y, z)].Type = (density - fy) > 0.0f ? BlockType.Solid1 : BlockType.Empty;
					}
				}
			}
		}

		#endregion

		private void Initialize()
		{
			_density.GenerateOctaves(4, 1.0f, 0.667f, 2.0f);
		}
	}
}
