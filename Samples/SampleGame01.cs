using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Xu.Async;
using Xu.Genres.VoxelBased;
using Xu.Graphics.Cameras;
using Xu.Graphics.TextureAtlas;
using Xu.Input;

namespace Sample01
{
	public class SampleGame01 : Game
	{
		private readonly GraphicsDeviceManager _graphics;

		private SpriteBatch _spriteBatch;

		private static readonly Color BackgroundColor = Color.CornflowerBlue;
		private SpriteFont _font;
		private BasicCamera _camera;

		private const string AtlasTextureName = "TextureAtlas";
		private const int Width = 1680;
		private const int Height = 1050;
		private const float NearDistance = 0.1f;
		private const float FarDistance = 150.0f;
		private const float Fov = MathHelper.PiOver4 * 1.5f;

		public SampleGame01()
		{
			Content.RootDirectory = "Content";

			_graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = Width;
			_graphics.PreferredBackBufferHeight = Height;

			_graphics.SynchronizeWithVerticalRetrace = false;
			IsFixedTimeStep = false;

			_camera = new BasicCamera(Vector3.Zero, Fov, Width / (float) Height, NearDistance, FarDistance);

			BasicCameraController cameraController = new BasicCameraController(this, _camera);
			cameraController.MovementSpeed = 10.0f;
			cameraController.InvertY = true;
			cameraController.RotationSpeed = Fov / 400.0f;

			IChunkGenerator chunkGenerator = new BasicChunkGenerator(42);
			IChunkStore chunkStore = new NullChunkStore();

			TaskManager taskManager = new TaskManager(this);
			ChunkManager chunkManager = new ChunkManager(this, chunkGenerator, chunkStore, taskManager, _camera);

			TiledTextureAtlasSpecification atlasSpecification = new TiledTextureAtlasSpecification {
				TextureResourceName = AtlasTextureName,
				TileWidth = 32,
				TileHeight = 32,
			};

			DeferredChunkRenderer chunkRenderer = new DeferredChunkRenderer(this, chunkManager, _camera, atlasSpecification, true);

			KeyboardManager keyboardManager = new KeyboardManager(this);
			keyboardManager.BindPress(Keys.Escape, Exit);
			keyboardManager.BindPress(Keys.F12, Debugger.Break);
			keyboardManager.BindHold(Keys.W, () => cameraController.Translate(Vector3.Forward));
			keyboardManager.BindHold(Keys.S, () => cameraController.Translate(Vector3.Backward));
			keyboardManager.BindHold(Keys.A, () => cameraController.Translate(Vector3.Left));
			keyboardManager.BindHold(Keys.D, () => cameraController.Translate(Vector3.Right));
			keyboardManager.BindHold(Keys.Q, () => cameraController.Translate(Vector3.Up));
			keyboardManager.BindHold(Keys.Z, () => cameraController.Translate(Vector3.Down));
			keyboardManager.BindHold(Keys.LeftShift, () => cameraController.Accelerate(10.0f));
			keyboardManager.BindHold(Keys.LeftControl, () => cameraController.Accelerate(0.25f));
			keyboardManager.BindPress(Keys.F1, chunkRenderer.ToggleSsao);

			MouseManager mouseManager = new MouseManager(this);
			mouseManager.BindMoveRelative(MouseManagerMode.Direct, (dx, dy) => cameraController.Rotate(dx, dy, 0.0f));

			Components.Add(keyboardManager);
			Components.Add(mouseManager);

			Components.Add(cameraController);
			Components.Add(chunkManager);
			Components.Add(chunkRenderer);

			Components.Add(taskManager);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(BackgroundColor);

			base.Draw(gameTime);
		}
	}
}
