using Microsoft.Xna.Framework;

namespace Xu.Graphics.TextureAtlas
{
	public interface ITextureAtlasCoordinateProvider
	{
		int TextureCount { get; }
		void SetTextureCoord(int textureId, out Vector2 topLeft, out Vector2 bottomRight);
		void SetTextureCoord(int textureId, out Vector2 topLeft, out Vector2 topRight, out Vector2 bottomRight, out Vector2 bottomLeft);
	}
}