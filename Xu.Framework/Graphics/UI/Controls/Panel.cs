using Microsoft.Xna.Framework;

namespace Xu.Graphics.UI.Controls
{
	public class Panel : Control
	{
		public Color BackgroundColor { get; set; }
		public Color OutlineColor { get; set; }

		public override void Draw(IControlVisualizer visualizer, GameTime gameTime)
		{
			visualizer.DrawPanel(ViewportRectangle, BackgroundColor, OutlineColor);
		}
	}
}
