using Microsoft.Xna.Framework;
using Xu.Graphics.UI.Layout;

namespace Xu.Graphics.UI.Controls
{
	public class Button : Control
	{
		public Color BackgroundColor { get; set; }
		public Color OutlineColor { get; set; }

		public string Text { get; set; }
		public Color TextColor { get; set; }
		public Alignment TextAlignment { get; set; }

		public override void Draw(IControlVisualizer visualizer, GameTime gameTime)
		{
			visualizer.DrawButton(ViewportRectangle, BackgroundColor, OutlineColor, Text, TextColor, TextAlignment);
		}
	}
}
