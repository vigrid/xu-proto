using Microsoft.Xna.Framework;

namespace Xu.Graphics.UI.Controls
{
	public class RadioButton : Control
	{
		public Color BackgroundColor { get; set; }
		public Color OutlineColor { get; set; }
		public bool IsChecked { get; set; }

		public override void Draw(IControlVisualizer visualizer, GameTime gameTime)
		{
			visualizer.DrawRadioButton(ViewportRectangle, BackgroundColor, OutlineColor, IsChecked);
		}
	}
}
