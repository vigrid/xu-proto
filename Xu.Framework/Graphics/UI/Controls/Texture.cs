using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xu.Graphics.UI.Controls
{
	public class Texture : Control
	{
		public Texture()
		{
		}

		public Color Color { get; set; }
		public Texture2D Image { get; set; }

		public override void Draw(IControlVisualizer visualizer, GameTime gameTime)
		{
			visualizer.DrawTexture(ViewportRectangle, Image, Color);
		}
	}
}
