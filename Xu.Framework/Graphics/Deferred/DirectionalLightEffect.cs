using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Xu.Graphics.Deferred
{
	public class DirectionalLightEffect : Effect
	{
		public static DirectionalLightEffect Create(ContentManager contentManager)
		{
			var effect = contentManager.Load<Effect>("Core/Effects/Light-Directional");
			return new DirectionalLightEffect(effect);
		}

		private DirectionalLightEffect(Effect cloneSource) : base(cloneSource)
		{
		}

		public Vector3 LightDirection { get; set; }
		public Vector4 LightColor { get; set; }
		public float LightIntensity { get; set; }

		public Texture2D NormalMap { get; set; }
		public Texture2D DepthMap { get; set; }
		public Matrix ViewProjectionInverse { get; set; }

		protected override bool OnApply()
		{
			Parameters["NormalMap"].SetValue(NormalMap);
			// Parameters["DepthMap"].SetValue(DepthMap);
			Parameters["ViewProjectionInverse"].SetValue(ViewProjectionInverse);
			Parameters["LightDirection"].SetValue(LightDirection);
			Parameters["LightColor"].SetValue(LightColor);
			Parameters["LightIntensity"].SetValue(LightIntensity);

			return false;
		}
	}
}
