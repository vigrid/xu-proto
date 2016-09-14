using Microsoft.Xna.Framework;

namespace Xu.Graphics.UI.Controls
{
	public class Image : Control
	{
		public Image(string imageAssetName)
		{
			ImageAssetName = imageAssetName;
		}

		public Color Color { get; set; }
		public string ImageAssetName { get; set; }

		public override void Draw(IControlVisualizer visualizer, GameTime gameTime)
		{
			visualizer.DrawImage(ViewportRectangle, ImageAssetName, Color);
		}
	}
}
