using System.IO;
using System.IO.Compression;
using System.Text;
using MonoGame.Utilities;


namespace Xu.Genres.VoxelBased
{
	public class GZipChunkStore : IChunkStore
	{
		private static readonly StringBuilder ChunkFileNameBuilder = new StringBuilder(64);

		private readonly byte[] _buffer;
		private readonly string _directory;

		public GZipChunkStore(string directory)
		{
			_directory = Path.GetFullPath(directory);
			_buffer = new byte[Chunk.ChunkSizeX * Chunk.ChunkSizeY * Chunk.ChunkSizeZ];

			EnsureDirectory(_directory);
		}

		#region IChunkStore Members

		public bool ContainsChunk(Chunk chunk)
		{
			return File.Exists(GetFullPath(chunk));
		}

		public void LoadChunkData(Chunk chunk)
		{
			lock (_buffer)
			{
				Block[] blocks = chunk.Blocks;
				if (blocks != null)
				{
					using (var fileStream = new FileStream(GetFullPath(chunk), FileMode.Open))
					{
						using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
						{
							gzipStream.Read(_buffer, 0, _buffer.Length);
						}
					}

					int index = 0;
					for (int x = 0; x < Chunk.ChunkSizeX; x++)
					{
						for (int y = 0; y < Chunk.ChunkSizeY; y++)
						{
							for (int z = 0; z < Chunk.ChunkSizeZ; z++)
							{
								blocks[index].Type = (BlockType) _buffer[index];
								index++;
							}
						}
					}
				}
			}
		}

		public void SaveChunkData(Chunk chunk)
		{
			lock (_buffer)
			{
				Block[] blocks = chunk.Blocks;
				if (blocks != null)
				{
					int index = 0;
					for (int x = 0; x < Chunk.ChunkSizeX; x++)
					{
						for (int y = 0; y < Chunk.ChunkSizeY; y++)
						{
							for (int z = 0; z < Chunk.ChunkSizeZ; z++)
							{
								_buffer[index] = (byte) blocks[index].Type;
								index++;
							}
						}
					}

					using (var fileStream = new FileStream(GetFullPath(chunk), FileMode.Create))
					{
						using (var gzipStream = new GZipStream(fileStream, CompressionMode.Compress))
						{
							gzipStream.Write(_buffer, 0, _buffer.Length);
						}
					}
				}
			}
		}

		#endregion

		private static void EnsureDirectory(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		private static string GetFileName(Chunk chunk)
		{
			lock (ChunkFileNameBuilder)
			{
				ChunkFileNameBuilder.Clear();
				ChunkFileNameBuilder.AppendFormat("{0:X8}.", chunk.Position.X);
				ChunkFileNameBuilder.AppendFormat("{0:X8}.", chunk.Position.Y);
				ChunkFileNameBuilder.AppendFormat("{0:X8}.chunk.gz", chunk.Position.Z);
				return ChunkFileNameBuilder.ToString();
			}
		}

		private string GetDirectory(Chunk chunk, int level)
		{
			return chunk.GetHashCode().ToString("X8").Substring(8 - level, 1);
		}

		private string GetFullPath(Chunk chunk)
		{
			string path = Path.Combine(_directory, GetDirectory(chunk, 1), GetDirectory(chunk, 2));
			EnsureDirectory(path);
			return Path.Combine(path, GetFileName(chunk));
		}
	}
}
