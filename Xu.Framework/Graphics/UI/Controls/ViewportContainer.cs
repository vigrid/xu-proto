using Microsoft.Xna.Framework;

namespace Xu.Graphics.UI.Controls
{
	public class ViewportContainer : Container
	{
		private readonly Game _game;

		internal ViewportContainer(Game game)
		{
			_game = game;
		}

		public override Rectangle ViewportRectangle
		{
			get
			{
				Rectangle result;

				result.X = 0;
				result.Y = 0;
				result.Width = _game.Window.ClientBounds.Width;
				result.Height = _game.Window.ClientBounds.Height;

				return result;
			}
		}
	}
}
