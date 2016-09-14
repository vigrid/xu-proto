using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Xu.Graphics.Deferred
{
	public class DeferredGeometryEffect : Effect
	{
		public static DeferredGeometryEffect Create(ContentManager contentManager)
		{
			var effect = contentManager.Load<Effect>("Core/Effects/Render-GBuffer");
			return new DeferredGeometryEffect(effect);
		}

		private DeferredGeometryEffect(Effect cloneSource) : base(cloneSource)
		{
		}

		public Matrix World { get; set; }
		public Matrix View { get; set; }
		public Matrix Projection { get; set; }

		public Texture2D Texture { get; set; }

		public Vector2 TileSize { get; set; }

		public float TextureFactor { get; set; }

		protected override bool OnApply()
		{
			Matrix wt = Matrix.Invert(World);
			Matrix wit = Matrix.Transpose(wt);

			Parameters["TextureFactor"].SetValue(TextureFactor);
			Parameters["TileSize"].SetValue(TileSize);
			Parameters["ColorMap"].SetValue(Texture);
			Parameters["WorldViewProjection"].SetValue(World * View * Projection);
			Parameters["WorldInverseTranspose"].SetValue(wit);

			return false;
		}
	}
}