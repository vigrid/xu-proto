using Microsoft.Xna.Framework;

namespace Xu.Graphics.UI.Controls
{
	public class Box : Control
	{
		public Color Color { get; set; }

		public override void Draw(IControlVisualizer visualizer, GameTime gameTime)
		{
			visualizer.DrawRectangle(ViewportRectangle, Color);
		}
	}
}
