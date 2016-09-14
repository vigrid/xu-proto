using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Sample02.WorldGeneration;
using Xu.Async;
using Xu.Genres.VoxelBased;
using Xu.Genres.VoxelBased.WorldGeneration;
using Xu.Graphics.Cameras;
using Xu.Graphics.TextureAtlas;
using Xu.Graphics.UI;
using Xu.Graphics.UI.Controls;
using Xu.Graphics.UI.Layout;
using Xu.Input;

namespace Sample04
{
	public class SampleGame04 : Game
	{
		private const string AtlasTextureName = "TextureAtlas";
		private const int Width = 1680;
		private const int Height = 1050;
		private const float NearDistance = 0.1f;
		private const float FarDistance = 150.0f;
		private const float Fov = MathHelper.PiOver4 * 1.5f;
		private static readonly Color BackgroundColor = Color.CornflowerBlue;

		private readonly GraphicsDeviceManager _graphics;
		private readonly KeyboardManager _keyboardManager;
		private readonly MouseManager _mouseManager;
		private readonly ChunkManager _chunkManager;
		private BasicCamera _camera;

		public SampleGame04()
		{
			Content.RootDirectory = "Content";

			_graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = Width;
			_graphics.PreferredBackBufferHeight = Height;
			_graphics.SynchronizeWithVerticalRetrace = true;
			_graphics.PreferMultiSampling = true;
			IsFixedTimeStep = true;

			_camera = new BasicCamera(Vector3.Zero, Fov, Width / (float) Height, NearDistance, FarDistance);

			var chunkGenerator = new ModularChunkGenerator();
			var processor1 = new BaseHeightVoxelProcessor();
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

			var taskManager = new TaskManager(this);
			_chunkManager = new ChunkManager(this, chunkGenerator, chunkStore, taskManager, _camera);

			var cameraController = new PlayerCameraController(this, _camera, _chunkManager);
			cameraController.MovementSpeed = 10.0f;
			cameraController.InvertY = true;
			cameraController.RotationSpeed = Fov / 400.0f;

			var atlasSpecification = new TiledTextureAtlasSpecification
			{
				TextureResourceName = AtlasTextureName,
				TileWidth = 32,
				TileHeight = 32,
			};

			var chunkRenderer = new DeferredChunkRenderer(this, _chunkManager, _camera, atlasSpecification, true);

			_keyboardManager = new KeyboardManager(this);
			_keyboardManager.BindPress(Keys.Escape, Exit);
			_keyboardManager.BindHold(Keys.W, () => cameraController.MoveCharacter(Vector3.Forward));
			_keyboardManager.BindHold(Keys.S, () => cameraController.MoveCharacter(Vector3.Backward));
			_keyboardManager.BindHold(Keys.A, () => cameraController.MoveCharacter(Vector3.Left));
			_keyboardManager.BindHold(Keys.D, () => cameraController.MoveCharacter(Vector3.Right));
			_keyboardManager.BindHold(Keys.Space, () => cameraController.JumpCharacter());

			_keyboardManager.BindPress(Keys.F1, chunkRenderer.ToggleSsao);
			_keyboardManager.BindPress(Keys.F2, chunkRenderer.ToggleDebug);

			_keyboardManager.BindPress(Keys.D1, () => _chunkManager.ActiveBlockType = BlockType.GridWhite);
			_keyboardManager.BindPress(Keys.D2, () => _chunkManager.ActiveBlockType = BlockType.GridRed);
			_keyboardManager.BindPress(Keys.D3, () => _chunkManager.ActiveBlockType = BlockType.GridGreen);
			_keyboardManager.BindPress(Keys.D4, () => _chunkManager.ActiveBlockType = BlockType.GridBlue);

			_keyboardManager.BindPress(Keys.F3, () => chunkRenderer.SsaoIntensity -= 0.05f);
			_keyboardManager.BindPress(Keys.F4, () => chunkRenderer.SsaoIntensity += 0.05f);

			_keyboardManager.BindPress(Keys.F5, () => chunkRenderer.SsaoSize -= 0.05f);
			_keyboardManager.BindPress(Keys.F6, () => chunkRenderer.SsaoSize += 0.05f);

			_keyboardManager.BindPress(Keys.F7, () => chunkRenderer.TextureFactor -= 0.05f);
			_keyboardManager.BindPress(Keys.F8, () => chunkRenderer.TextureFactor += 0.05f);

			_keyboardManager.BindPress(Keys.PageUp, () => chunkRenderer.SsaoPasses++);
			_keyboardManager.BindPress(Keys.PageDown, () => chunkRenderer.SsaoPasses--);

			_keyboardManager.BindPress(Keys.M, () => cameraController.InvertY = !cameraController.InvertY);
			_keyboardManager.BindPress(Keys.N, () => chunkRenderer.ToggleNoise());
            chunkRenderer.ToggleNoise();

			_mouseManager = new MouseManager(this);
			_mouseManager.BindMoveRelative(MouseManagerMode.Direct, (dx, dy) => cameraController.Rotate(dx, dy, 0.0f));
			_mouseManager.BindLeftButton(MouseManagerMode.Direct, () => _chunkManager.ShouldRemoveBlock = true, null);
			_mouseManager.BindRightButton(MouseManagerMode.Direct, () => _chunkManager.ShouldPlaceBlock = true, null);

			_keyboardManager.BindPress(Keys.Tab, () => _mouseManager.Mode = _mouseManager.Mode == MouseManagerMode.Direct ? MouseManagerMode.UI : MouseManagerMode.Direct);

			var uiManager = new UIManager(this, _keyboardManager, _mouseManager);
			uiManager.Root.AddControl(new Image("crosshair")
			{
				Bounds = Bounds.Centered(32, 32),
				Color = Color.White,
			});

			Components.Add(_keyboardManager);
			Components.Add(_mouseManager);
			Components.Add(cameraController);

			Components.Add(_chunkManager);
			Components.Add(chunkRenderer);

			Components.Add(taskManager);

			Components.Add(uiManager);
		}

		protected override void Update(GameTime gameTime)
		{
			_chunkManager.PickingRay = _camera.Pick(new Vector2(_mouseManager.State.X, _mouseManager.State.Y), GraphicsDevice.Viewport);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(BackgroundColor);

			base.Draw(gameTime);
		}
	}
}
