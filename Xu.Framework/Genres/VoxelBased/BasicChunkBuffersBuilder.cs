using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xu.Graphics;
using Xu.Graphics.TextureAtlas;
using Xu.Types;

namespace Xu.Genres.VoxelBased
{
	public class BasicChunkBuffersBuilder
	{
		private readonly Vector3[] _basePositions = Primitives.UnitCubeBoundingBox.GetCorners();

		private readonly short[] _indexBuffer = new short[Chunk.ChunkSizeX * Chunk.ChunkSizeY * Chunk.ChunkSizeZ * 24];

		private readonly short[] _quadIndexes = new short[]
		{
			0, 1, 2, 2, 3, 0
		};

		private readonly Vector2[,] _textureCoordinates;
		private readonly VertexPositionNormalTexture[] _vertexBuffer = new VertexPositionNormalTexture[Chunk.ChunkSizeX * Chunk.ChunkSizeY * Chunk.ChunkSizeZ * 24];

		public BasicChunkBuffersBuilder(ITextureAtlasCoordinateProvider textureAtlasCoordinateProvider)
		{
			_textureCoordinates = new Vector2[textureAtlasCoordinateProvider.TextureCount,4];

			for (int i = 0; i < textureAtlasCoordinateProvider.TextureCount; i++)
			{
				textureAtlasCoordinateProvider.SetTextureCoord(i, out _textureCoordinates[i, 0], out _textureCoordinates[i, 1], out _textureCoordinates[i, 2], out _textureCoordinates[i, 3]);
			}
		}

		public bool TryCreateBuffers(Chunk chunk, GraphicsDevice device)
		{
			int vertexCount = 0;
			int indexCount = 0;

			IntVector3 blockCoord;

			for (blockCoord.X = 0; blockCoord.X < Chunk.ChunkSizeX; blockCoord.X++)
			{
				for (blockCoord.Y = 0; blockCoord.Y < Chunk.ChunkSizeY; blockCoord.Y++)
				{
					for (blockCoord.Z = 0; blockCoord.Z < Chunk.ChunkSizeZ; blockCoord.Z++)
					{
						Block block;
						if (!chunk.TryGetBlock(blockCoord, out block))
						{
							throw new InvalidOperationException("Should never happen");
						}

						if (block.Type == BlockType.Empty)
						{
							continue;
						}

						BlockFaces facesToDraw = chunk.GetVisibleFaces(blockCoord.X, blockCoord.Y, blockCoord.Z);

						if (facesToDraw != BlockFaces.None)
						{
							PushFaces(facesToDraw, chunk, blockCoord, ref vertexCount, ref indexCount);
						}
					}
				}
			}

			if (vertexCount > 0)
			{
				if (chunk.VertexBuffer != null)
				{
					chunk.VertexBuffer.Dispose();
				}

				chunk.VertexBuffer = new VertexBuffer(device, typeof (VertexPositionNormalTexture), vertexCount, BufferUsage.WriteOnly);
				chunk.VertexBuffer.SetData(_vertexBuffer, 0, vertexCount);
				chunk.IndexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, indexCount, BufferUsage.WriteOnly);
				chunk.IndexBuffer.SetData(_indexBuffer, 0, indexCount);

				return true;
			}

			return false;
		}

		private void PushFaces(BlockFaces facesToDraw, Chunk chunk, IntVector3 blockCoord, ref int vertexIndex, ref int indexIndex)
		{
			var offset = (Vector3) blockCoord;

			var textureId = (int) chunk.Blocks[Chunk.BlockIndex(blockCoord)].Type;

			if ((facesToDraw & BlockFaces.Left) == BlockFaces.Left)
			{
				PushVertexes(4, 0, 3, 7, offset, Vector3.Left, textureId, ref vertexIndex);
				int indexOffset = (indexIndex / 6) * 4;
				foreach (short baseIndex in _quadIndexes)
				{
					_indexBuffer[indexIndex++] = (short) (baseIndex + indexOffset);
				}
			}
			if ((facesToDraw & BlockFaces.Right) == BlockFaces.Right)
			{
				PushVertexes(1, 5, 6, 2, offset, Vector3.Right, textureId, ref vertexIndex);
				int indexOffset = (indexIndex / 6) * 4;
				foreach (short baseIndex in _quadIndexes)
				{
					_indexBuffer[indexIndex++] = (short) (baseIndex + indexOffset);
				}
			}

			if ((facesToDraw & BlockFaces.Up) == BlockFaces.Up)
			{
				PushVertexes(4, 5, 1, 0, offset, Vector3.Up, textureId, ref vertexIndex);
				int indexOffset = (indexIndex / 6) * 4;
				foreach (short baseIndex in _quadIndexes)
				{
					_indexBuffer[indexIndex++] = (short) (baseIndex + indexOffset);
				}
			}
			if ((facesToDraw & BlockFaces.Down) == BlockFaces.Down)
			{
				PushVertexes(3, 2, 6, 7, offset, Vector3.Down, textureId, ref vertexIndex);
				int indexOffset = (indexIndex / 6) * 4;
				foreach (short baseIndex in _quadIndexes)
				{
					_indexBuffer[indexIndex++] = (short) (baseIndex + indexOffset);
				}
			}

			if ((facesToDraw & BlockFaces.Backward) == BlockFaces.Backward)
			{
				PushVertexes(0, 1, 2, 3, offset, Vector3.Backward, textureId, ref vertexIndex);
				int indexOffset = (indexIndex / 6) * 4;
				foreach (short baseIndex in _quadIndexes)
				{
					_indexBuffer[indexIndex++] = (short) (baseIndex + indexOffset);
				}
			}
			if ((facesToDraw & BlockFaces.Forward) == BlockFaces.Forward)
			{
				PushVertexes(5, 4, 7, 6, offset, Vector3.Forward, textureId, ref vertexIndex);
				int indexOffset = (indexIndex / 6) * 4;
				foreach (short baseIndex in _quadIndexes)
				{
					_indexBuffer[indexIndex++] = (short) (baseIndex + indexOffset);
				}
			}
		}

		private void PushVertexes(int a, int b, int c, int d, Vector3 offset, Vector3 normal, int textureId, ref int vertexIndex)
		{
			_vertexBuffer[vertexIndex].Position = _basePositions[a] + offset;
			_vertexBuffer[vertexIndex].TextureCoordinate = _textureCoordinates[textureId, 0];
			_vertexBuffer[vertexIndex++].Normal = normal;

			_vertexBuffer[vertexIndex].Position = _basePositions[b] + offset;
			_vertexBuffer[vertexIndex].TextureCoordinate = _textureCoordinates[textureId, 0];
			_vertexBuffer[vertexIndex++].Normal = normal;

			_vertexBuffer[vertexIndex].Position = _basePositions[c] + offset;
			_vertexBuffer[vertexIndex].TextureCoordinate = _textureCoordinates[textureId, 0];
			_vertexBuffer[vertexIndex++].Normal = normal;

			_vertexBuffer[vertexIndex].Position = _basePositions[d] + offset;
			_vertexBuffer[vertexIndex].TextureCoordinate = _textureCoordinates[textureId, 0];
			_vertexBuffer[vertexIndex++].Normal = normal;
		}
	}
}
