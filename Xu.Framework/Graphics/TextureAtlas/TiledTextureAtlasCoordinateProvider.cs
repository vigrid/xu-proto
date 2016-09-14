using Microsoft.Xna.Framework;

namespace Xu.Graphics.TextureAtlas
{
	public class TiledTextureAtlasCoordinateProvider : ITextureAtlasCoordinateProvider
	{
		private readonly int _horizontalTileCount;
		private readonly int _verticalTileCount;
		private readonly bool _firstTileHasTexture;

		private readonly Vector2 _cornerOffset;
		private readonly Vector2 _textureTileSize;
		private readonly Vector2 _uvTileSize;

		public TiledTextureAtlasCoordinateProvider(int tileWidth, int tileHeight, int horizontalTileCount, int verticalTileCount, bool firstTileHasTexture, bool usePointSampling)
		{
			_horizontalTileCount = horizontalTileCount;
			_verticalTileCount = verticalTileCount;
			_firstTileHasTexture = firstTileHasTexture;
			float epsilon = usePointSampling ? 0.0001f : 0.5f;

			float textureWidth = tileWidth * horizontalTileCount;
			float textureHeight = tileHeight * verticalTileCount;

			_cornerOffset.X = epsilon / textureWidth;
			_cornerOffset.Y = epsilon / textureHeight;
			_textureTileSize.X = tileWidth / textureWidth;
			_textureTileSize.Y = tileHeight / textureHeight;
			_uvTileSize = _textureTileSize - _cornerOffset * 2.0f;
		}

		public int TextureCount
		{
			get { return _horizontalTileCount * _verticalTileCount; }
		}

		private void GetTileBoundaries(int textureId, out float left, out float right, out float top, out float bottom)
		{
			if (_firstTileHasTexture)
			{
				textureId--;
			}

			int tileX = textureId % _horizontalTileCount;
			int tileY = textureId / _horizontalTileCount;
			left = tileX * _textureTileSize.X + _cornerOffset.X;
			top = tileY * _textureTileSize.Y + _cornerOffset.Y;
			right = left + _uvTileSize.X;
			bottom = top + _uvTileSize.Y;
		}

		public void SetTextureCoord(int textureId, out Vector2 topLeft, out Vector2 bottomRight)
		{
			float left, right, top, bottom;
			GetTileBoundaries(textureId, out left, out right, out top, out bottom);
			topLeft.X = left;
			topLeft.Y = top;
			bottomRight.X = right;
			bottomRight.Y = bottom;
		}

		public void SetTextureCoord(int textureId, out Vector2 topLeft, out Vector2 topRight, out Vector2 bottomRight, out Vector2 bottomLeft)
		{
			float left, right, top, bottom;
			GetTileBoundaries(textureId, out left, out right, out top, out bottom);
			topLeft.X = left;
			topLeft.Y = top;
			topRight.X = right;
			topRight.Y = top;
			bottomRight.X = right;
			bottomRight.Y = bottom;
			bottomLeft.X = left;
			bottomLeft.Y = bottom;
		}
	}
}