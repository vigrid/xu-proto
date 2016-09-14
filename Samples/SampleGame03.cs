using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xu.Graphics.UI;
using Xu.Graphics.UI.Controls;
using Xu.Graphics.UI.Layout;
using Xu.Input;

namespace Sample03
{
	public class SampleGame03 : Game
	{
		private readonly GraphicsDeviceManager _graphics;

		public SampleGame03()
		{
			Content.RootDirectory = "Content";

			_graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = 640;
			_graphics.PreferredBackBufferHeight = 360;

			var keyboardManager = new KeyboardManager(this);
			var mouseManager = new MouseManager(this);

			keyboardManager.BindPress(Keys.Escape, Exit);

			mouseManager.Mode = MouseManagerMode.UI;

			var uiManager = new UIManager(this, keyboardManager, mouseManager);
			uiManager.Root.AddControl(new Panel
			{
				Bounds = Bounds.DockBottom(40).Expanded(-20, 0).Translated(0, -20),
				BackgroundColor = Color.White,
				OutlineColor = Color.Black
			});

			Components.Add(keyboardManager);
			Components.Add(mouseManager);

			Components.Add(uiManager);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			base.Draw(gameTime);
		}
	}
}
