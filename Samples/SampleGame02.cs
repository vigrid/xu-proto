using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Sample02.WorldGeneration;
using Xu.Async;
using Xu.Genres.VoxelBased;
using Xu.Genres.VoxelBased.WorldGeneration;
using Xu.Graphics.Cameras;
using Xu.Graphics.TextureAtlas;
using Xu.Input;

namespace Sample02
{
	public class SampleGame02 : Game
	{
		private readonly GraphicsDeviceManager _graphics;

		private static readonly Color BackgroundColor = Color.CornflowerBlue;

		private const string AtlasTextureName = "TextureAtlas";
		private const int Width = 1680;
		private const int Height = 1050;
		private const float NearDistance = 0.1f;
		private const float FarDistance = 150.0f;
		private const float Fov = MathHelper.PiOver4 * 1.5f;

		public SampleGame02()
		{
			Content.RootDirectory = "Content";

			_graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = Width;
			_graphics.PreferredBackBufferHeight = Height;
			_graphics.SynchronizeWithVerticalRetrace = false;
			IsFixedTimeStep = false;

			BasicCamera camera = new BasicCamera(Vector3.Up * 10.0f, Fov, Width / (float) Height, NearDistance, FarDistance);

			BasicCameraController cameraController = new BasicCameraController(this, camera);
			cameraController.MovementSpeed = 10.0f;
			cameraController.InvertY = true;
			cameraController.RotationSpeed = Fov / 400.0f;

			ModularChunkGenerator chunkGenerator = new ModularChunkGenerator();
			var processor1 = new BaseHeightVoxelProcessor(42);
			var processor2 = new CanyonCarvingVoxelProcessor();
			var processor3 = new HoleCarvingVoxelProcessor();
			var processor4 = new OverhangVoxelProcessor();
			var processor5 = new OrePlacingVoxelProcessor();
			chunkGenerator.AddProcessor(processor1);
			chunkGenerator.AddProcessor(processor2);
			chunkGenerator.AddProcessor(processor3);
			chunkGenerator.AddProcessor(processor4);
			chunkGenerator.AddProcessor(processor5);
			chunkGenerator.SetClassifier(new ResourceAwareVoxelClassifier());

			IChunkStore chunkStore = new NullChunkStore();

			TaskManager taskManager = new TaskManager(this);
			ChunkManager chunkManager = new ChunkManager(this, chunkGenerator, chunkStore, taskManager, camera);

			TiledTextureAtlasSpecification atlasSpecification = new TiledTextureAtlasSpecification {
				TextureResourceName = AtlasTextureName,
				TileWidth = 32,
				TileHeight = 32,
			};

			DeferredChunkRenderer chunkRenderer = new DeferredChunkRenderer(this, chunkManager, camera, atlasSpecification, true);

			KeyboardManager keyboardManager = new KeyboardManager(this);
			keyboardManager.BindPress(Keys.Escape, Exit);
			// keyboardManager.BindPress(Keys.F12, Debugger.Break);
			keyboardManager.BindHold(Keys.W, () => cameraController.Translate(Vector3.Forward));
			keyboardManager.BindHold(Keys.S, () => cameraController.Translate(Vector3.Backward));
			keyboardManager.BindHold(Keys.A, () => cameraController.Translate(Vector3.Left));
			keyboardManager.BindHold(Keys.D, () => cameraController.Translate(Vector3.Right));
			keyboardManager.BindHold(Keys.Q, () => cameraController.Translate(Vector3.Up));
			keyboardManager.BindHold(Keys.Z, () => cameraController.Translate(Vector3.Down));
			keyboardManager.BindHold(Keys.LeftShift, () => cameraController.Accelerate(10.0f));
			keyboardManager.BindHold(Keys.LeftControl, () => cameraController.Accelerate(0.25f));

			MouseManager mouseManager = new MouseManager(this);
			mouseManager.BindMoveRelative(MouseManagerMode.Direct, (dx, dy) => cameraController.Rotate(dx, dy, 0.0f));
			mouseManager.BindLeftButton(MouseManagerMode.Direct, () => chunkManager.ShouldPlaceBlock = true, null);
			mouseManager.BindRightButton(MouseManagerMode.Direct, () => chunkManager.ShouldRemoveBlock = true, null);

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
