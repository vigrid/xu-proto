using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Xu.Graphics.Deferred
{
	public class SsaoEffect : Effect
	{
		public float Intensity { get; set; }

		public static SsaoEffect Create(ContentManager contentManager)
		{
			var effect = contentManager.Load<Effect>("Core/Effects/SSAO");
			return new SsaoEffect(effect);
		}

		private SsaoEffect(Effect cloneSource) : base(cloneSource)
		{
		}

		public Texture2D NormalMap { get; set; }
		public Texture2D DepthMap { get; set; }
		public Matrix ViewProjectionInverse { get; set; }

		public Matrix ViewProjection { get; set; }

		public Texture2D RandomMap { get; set; }

		public Vector2 NoiseOffset { get; set; }

		public float Size { get; set; }

		protected override bool OnApply()
		{
			Parameters["Size"].SetValue(Size);
			Parameters["NoiseOffset"].SetValue(NoiseOffset);
			Parameters["Intensity"].SetValue(Intensity);
			Parameters["RandomMap"].SetValue(RandomMap);
			// Parameters["NormalMap"].SetValue(NormalMap);
			Parameters["DepthMap"].SetValue(DepthMap);
			Parameters["ViewProjectionInverse"].SetValue(ViewProjectionInverse);
			Parameters["ViewProjection"].SetValue(ViewProjection);

			return false;
		}
	}
}