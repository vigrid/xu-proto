using Microsoft.Xna.Framework;

namespace Xu.Graphics.UI.Controls
{
	public class ProgressBar : Control
	{
		public Color BackgroundColor { get; set; }
		public Color OutlineColor { get; set; }
		public Color ProgressBarColor { get; set; }
		public float ProgressFraction { get; set; }

		public override void Draw(IControlVisualizer visualizer, GameTime gameTime)
		{
			visualizer.DrawProgressBar(ViewportRectangle, BackgroundColor, OutlineColor, ProgressBarColor, ProgressFraction);
		}
	}
}
