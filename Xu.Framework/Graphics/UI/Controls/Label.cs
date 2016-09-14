using Microsoft.Xna.Framework;
using Xu.Graphics.UI.Layout;

namespace Xu.Graphics.UI.Controls
{
	public class Label : Control
	{
		#region LabelType enum

		public enum LabelType
		{
			Title,
			Main,
		}

		#endregion

		public string Text { get; set; }
		public LabelType Type { get; set; }
		public Color TextColor { get; set; }
		public Alignment TextAlignment { get; set; }

		public override void Draw(IControlVisualizer visualizer, GameTime gameTime)
		{
			visualizer.DrawLabel(ViewportRectangle, Text, Type, TextColor, TextAlignment);
		}
	}
}
