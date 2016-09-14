using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xu.Graphics.Cameras;

namespace Xu.Graphics
{
	public class GraphicsHelper
	{
		private static readonly short[] BoundingBoxWireframeIndexBuffer = new short[]
		{
			0, 1, 1, 2, 2, 3, 3, 0, 0, 4, 1, 5, 2, 6, 3, 7, 4, 5, 5, 6, 6, 7, 7, 4
		};

		private static readonly short[] BoundingBoxSolidIndexBuffer = new short[]
		{
			0, 1, 2, 1, 5, 6, 5, 4, 7, 4, 0, 3, 3, 0, 2, 2, 1, 6, 6, 5, 7, 7, 4, 3, 4, 5, 1, 4, 1, 0, 7, 3, 2, 7, 2, 6
		};

		private static readonly VertexPositionTexture[] QuadVertices = new[] {
			new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)),
			new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0)),
			new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1)),
			new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
		};

		private static readonly short[] QuadIndices = new short[] {0, 1, 2, 0, 2, 3};

		private readonly GraphicsDevice _graphicsDevice;
		private readonly BasicEffect _effect;

		private readonly VertexPositionColor[] _boundingBoxVertices = new VertexPositionColor[8];

		public GraphicsHelper(GraphicsDevice graphicsDevice, Vector3 fogColor, float farDistance)
		{
			_graphicsDevice = graphicsDevice;
			_effect = new BasicEffect(_graphicsDevice)
			{
				VertexColorEnabled = true,
				LightingEnabled = false,
				FogEnabled = true,
				FogStart = 0.0f,
				FogEnd = farDistance,
				FogColor = fogColor
			};
		}

		public void UpdateForCamera(ICamera camera)
		{
			_effect.View = camera.View;
			_effect.Projection = camera.Projection;
		}

		public void Draw(BoundingBox boundingBox, Color color, bool solid)
		{
			Vector3[] corners = boundingBox.GetCorners();
			for (int i = 0; i < 8; i++)
			{
				_boundingBoxVertices[i].Position = corners[i];
				_boundingBoxVertices[i].Color = color;
			}

			foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				if (solid)
				{
					_graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _boundingBoxVertices, 0, 8, BoundingBoxSolidIndexBuffer, 0, (short) (BoundingBoxSolidIndexBuffer.Length / 3));
				}
				else
				{
					_graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, _boundingBoxVertices, 0, 8, BoundingBoxWireframeIndexBuffer, 0, (short) (BoundingBoxWireframeIndexBuffer.Length / 2));
				}
			}
		}

		public void DrawQuad(Vector2 halfPixel)
		{
			QuadVertices[0].TextureCoordinate.X = halfPixel.X;
			QuadVertices[0].TextureCoordinate.Y = halfPixel.Y;
			QuadVertices[1].TextureCoordinate.X = 1.0f + halfPixel.X;
			QuadVertices[1].TextureCoordinate.Y = halfPixel.Y;
			QuadVertices[2].TextureCoordinate.X = 1.0f + halfPixel.X;
			QuadVertices[2].TextureCoordinate.Y = 1.0f + halfPixel.Y;
			QuadVertices[3].TextureCoordinate.X = halfPixel.X;
			QuadVertices[3].TextureCoordinate.Y = 1.0f + halfPixel.Y;

			_graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, QuadVertices, 0, 4, QuadIndices, 0, 2);
		}
	}
}
