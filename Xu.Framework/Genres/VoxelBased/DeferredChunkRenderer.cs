using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xu.Graphics;
using Xu.Graphics.Cameras;
using Xu.Graphics.Deferred;
using Xu.Graphics.TextureAtlas;

namespace Xu.Genres.VoxelBased
{
	using System;

	using Generators;

	public class DeferredChunkRenderer : DrawableGameComponent
	{
		private SpriteBatch _spriteBatch;
		private readonly TiledTextureAtlasSpecification _atlasSpecification;
		private readonly ICamera _camera;
		private readonly IChunkManager _chunkManager;

		private readonly bool _usePointSampling;

		private Texture2D _atlas;
		private BasicChunkBuffersBuilder _bufferBuilder;

		private DeferredClearEffect _clearEffect;
		private DeferredGeometryEffect _geometryEffect;
		private DeferredCombineEffect _combineEffect;

		private DirectionalLightEffect _directionalLightEffect;
		private PointLightEffect _pointLightEffect;

		private GraphicsHelper _graphicsHelper;

		private int _bufferWidth;
		private int _bufferHeight;
		private RenderTarget2D _albedoTarget;
		private RenderTarget2D _normalTarget;
		private RenderTarget2D _depthTarget;
		private RenderTarget2D _lightTarget;
		private Model _sphereModel;

		private Vector2 _halfPixel;

		public DeferredChunkRenderer(Game game, IChunkManager chunkManager, ICamera camera, TiledTextureAtlasSpecification atlasSpecification, bool usePointSampling)
			: base(game)
		{
			_chunkManager = chunkManager;
			_camera = camera;
			_atlasSpecification = atlasSpecification;
			_usePointSampling = usePointSampling;
		}

		private void Render(Chunk chunk, float textureFactor)
		{
			if (!chunk.HasGraphicsData || chunk.State == ChunkState.DataOutOfSync)
			{
				chunk.DisposeBuffers();

				if (chunk.HasAllNeighbors)
				{
					_bufferBuilder.TryCreateBuffers(chunk, GraphicsDevice);
					chunk.State = ChunkState.DataInSync;
					chunk.HasGraphicsData = true;
				}
			}

			_geometryEffect.TextureFactor = textureFactor;
			_geometryEffect.TileSize = _tileSize;

			_geometryEffect.World = Matrix.CreateTranslation(chunk.Position.X * Chunk.ChunkSizeX, chunk.Position.Y * Chunk.ChunkSizeY, chunk.Position.Z * Chunk.ChunkSizeZ);
			_geometryEffect.View = _camera.View;
			_geometryEffect.Projection = _camera.Projection;

			GraphicsDevice.SetVertexBuffer(chunk.VertexBuffer);
			GraphicsDevice.Indices = chunk.IndexBuffer;

			foreach (EffectPass pass in _geometryEffect.CurrentTechnique.Passes)
			{
				pass.Apply();
				if (chunk.VertexBuffer != null)
				{
					int quadCount = chunk.VertexBuffer.VertexCount / 4;
					GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, quadCount * 4, 0, quadCount * 2);
				}
			}
		}

		private readonly Random _random = new Random();

		private Color[] CreateRandomNormalTexture(int width, int height)
		{
			var vectorGenerator = new RandomVectorGenerator(_random);
			var result = new Color[width * height];
			var offset = new Vector3(0.5f);

			for (int i = 0; i < result.Length; i++)
			{
				Vector3 vector = vectorGenerator.GetVector3();
				
				vector += offset;
				vector /= 2.0f;
				result[i] = new Color(vector);
			}

			return result;
		}

		protected override void LoadContent()
		{
			_graphicsHelper = new GraphicsHelper(GraphicsDevice, Color.White.ToVector3(), 1000.0f);
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			_font = Game.Content.Load<SpriteFont>("debug");

			_clearEffect = DeferredClearEffect.Create(Game.Content);
			_geometryEffect = DeferredGeometryEffect.Create(Game.Content);
			_combineEffect = DeferredCombineEffect.Create(Game.Content);

			_directionalLightEffect = DirectionalLightEffect.Create(Game.Content);
			_pointLightEffect = PointLightEffect.Create(Game.Content);
			_ssaoLightEffect = SsaoEffect.Create(Game.Content);

			_bufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
			_bufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

			_halfPixel = new Vector2(0.5f / _bufferWidth, 0.5f / _bufferHeight);

			_albedoTarget = new RenderTarget2D(GraphicsDevice, _bufferWidth, _bufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
			_normalTarget = new RenderTarget2D(GraphicsDevice, _bufferWidth, _bufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
			_depthTarget = new RenderTarget2D(GraphicsDevice, _bufferWidth, _bufferHeight, false, SurfaceFormat.Single, DepthFormat.None);
			_lightTarget = new RenderTarget2D(GraphicsDevice, _bufferWidth, _bufferHeight, false, SurfaceFormat.Color, DepthFormat.None);

			_randomMap = new Texture2D(GraphicsDevice, _bufferWidth, _bufferHeight, false, SurfaceFormat.Color);
			_randomMap.SetData(CreateRandomNormalTexture(_randomMap.Width, _randomMap.Height));

			_sphereModel = Game.Content.Load<Model>("Core/Meshes/icosphere-320");

			_atlas = Game.Content.Load<Texture2D>(_atlasSpecification.TextureResourceName);

			_geometryEffect.Texture = _atlas;

			int tilesHorizontal = _atlas.Width / _atlasSpecification.TileWidth;
			int tilesVertical = _atlas.Height / _atlasSpecification.TileHeight;
			var textureAtlasCoordinateProvider = new TiledTextureAtlasCoordinateProvider(_atlasSpecification.TileWidth, _atlasSpecification.TileHeight, tilesHorizontal, tilesVertical, _atlasSpecification.FirstTileHasTexture, _usePointSampling);

			_tileSize = new Vector2(_atlasSpecification.TileWidth / (float) _atlas.Width, _atlasSpecification.TileHeight / (float) _atlas.Height);

			_bufferBuilder = new BasicChunkBuffersBuilder(textureAtlasCoordinateProvider);
		}

		private SsaoEffect _ssaoLightEffect;
		private Texture2D _randomMap;
		private bool _ssaoEnabled = true;
		private bool _drawDebug;

		private Vector2 _tileSize;
		private SpriteFont _font;

		private float _ssaoSize = 2.5f;
		public float SsaoSize
		{
			get { return _ssaoSize; }
			set { _ssaoSize = Math.Max(value, 0.05f); }
		}

		private float _ssaoIntensity = 0.25f;
		public float SsaoIntensity
		{
			get { return _ssaoIntensity; }
			set { _ssaoIntensity = Math.Max(value, 0.05f); }
		}

		private float _textureFactor = 1.0f;
		public float TextureFactor
		{
			get { return _textureFactor; }
			set { _textureFactor = Math.Max(value, 0.05f); }
		}

		public override void Draw(GameTime gameTime)
		{
			GraphicsDevice.SetRenderTargets(_albedoTarget, _normalTarget, _depthTarget);
			ClearGBuffer();
			DrawGeometry(_textureFactor);

			GraphicsDevice.SetRenderTarget(_lightTarget);
			GraphicsDevice.Clear(Color.Transparent);

			if (_ssaoEnabled)
			{
				for (int i = 0; i < SsaoPasses; i++)
				{
					DrawSsao(_ssaoSize, _ssaoIntensity / SsaoPasses);
				}
			}

			DrawDirectionalLight(new Vector3(1, 2, -3), new Vector4(1.0f, 0.6f, 0.35f, 1.0f), 0.6f);
			DrawDirectionalLight(new Vector3(-1, -1, -4), new Vector4(0.3f, 0.8f, 0.65f, 1.0f), 0.2f);

			DrawPointLight(_camera.Position, Vector4.One, 0.25f, 50.0f);

			GraphicsDevice.SetRenderTarget(null);

			DrawCombine();

			if (_drawDebug)
			{
				DrawDebug();
			}

			_spriteBatch.Begin();
			_spriteBatch.DrawString(_font, _camera.Direction.ToString(), Vector2.Zero, Color.Yellow);
			_spriteBatch.DrawString(_font, String.Format("SS: {0}, SI: {1}, SP: {2}", SsaoSize, SsaoIntensity, SsaoPasses), new Vector2(0, 25), Color.Yellow);
			_spriteBatch.DrawString(_font, String.Format("TF: {0}", _textureFactor), new Vector2(0, 50), Color.Yellow);
			_spriteBatch.End();
		}

		protected bool UseTextures { get; set; }

		private int _ssaoPasses = 1;
		private bool _randomize;

		public int SsaoPasses
		{
			get { return _ssaoPasses; }
			set { _ssaoPasses = Math.Max(1, value); }
		}

		private void DrawSsao(float size, float intensity)
		{
			GraphicsDevice.RasterizerState = new RasterizerState
			{
				CullMode = CullMode.None,
				FillMode = FillMode.Solid
			};

			GraphicsDevice.BlendState = new BlendState
			{
				ColorBlendFunction = BlendFunction.Add,
				ColorSourceBlend = Blend.One,
				ColorDestinationBlend = Blend.One,
			};

			GraphicsDevice.DepthStencilState = DepthStencilState.None;

			if (_randomize)
			{
				_ssaoLightEffect.NoiseOffset = new Vector2((float) _random.NextDouble(), (float) _random.NextDouble());
			}
			else
			{
				_ssaoLightEffect.NoiseOffset = new Vector2();
			}
			_ssaoLightEffect.Size = size;
			_ssaoLightEffect.Intensity = intensity;
			_ssaoLightEffect.RandomMap = _randomMap;
			_ssaoLightEffect.NormalMap = _normalTarget;
			_ssaoLightEffect.DepthMap = _depthTarget;
			_ssaoLightEffect.ViewProjection = _camera.View * _camera.Projection;
			_ssaoLightEffect.ViewProjectionInverse = Matrix.Invert(_camera.View * _camera.Projection);

			_ssaoLightEffect.CurrentTechnique.Passes[0].Apply();
			_graphicsHelper.DrawQuad(_halfPixel);
		}

		private void DrawDebug()
		{
			var w = _bufferWidth / 2;
			var h = _bufferHeight / 2;
			Rectangle target = new Rectangle(0, 0, w, h);

			_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
			_spriteBatch.Draw(_albedoTarget, target, Color.White);
			target.X = w;
			_spriteBatch.Draw(_normalTarget, target, Color.White);
			target.Y = h;
			_spriteBatch.Draw(_depthTarget, target, Color.White);
			target.X = 0;
			_spriteBatch.Draw(_lightTarget, target, Color.White);
			_spriteBatch.End();
		}

		private void DrawCombine()
		{
			GraphicsDevice.RasterizerState = new RasterizerState
			{
				CullMode = CullMode.None,
				FillMode = FillMode.Solid
			};

			_combineEffect.Parameters["ColorMap"].SetValue(_albedoTarget);
			_combineEffect.Parameters["LightMap"].SetValue(_lightTarget);

			_combineEffect.CurrentTechnique.Passes[0].Apply();

			GraphicsDevice.BlendState = BlendState.Opaque;
			_graphicsHelper.DrawQuad(_halfPixel);
		}

		private void DrawDirectionalLight(Vector3 direction, Vector4 color, float intensity)
		{
			GraphicsDevice.RasterizerState = new RasterizerState
			{
				CullMode = CullMode.None,
				FillMode = FillMode.Solid
			};

			GraphicsDevice.BlendState = new BlendState
			{
				ColorBlendFunction = BlendFunction.Add,
				ColorSourceBlend = Blend.One,
				ColorDestinationBlend = Blend.One,
			};

			GraphicsDevice.DepthStencilState = DepthStencilState.None;

			_directionalLightEffect.LightDirection = direction;
			_directionalLightEffect.LightColor = color;
			_directionalLightEffect.LightIntensity = intensity;

			_directionalLightEffect.NormalMap = _normalTarget;
			_directionalLightEffect.DepthMap = _depthTarget;
			_directionalLightEffect.ViewProjectionInverse = Matrix.Invert(_camera.View * _camera.Projection);

			_directionalLightEffect.CurrentTechnique.Passes[0].Apply();
			_graphicsHelper.DrawQuad(_halfPixel);
		}

		private void DrawPointLight(Vector3 position, Vector4 color, float intensity, float radius)
		{
			GraphicsDevice.RasterizerState = new RasterizerState
			{
				CullMode = CullMode.CullClockwiseFace,
				FillMode = FillMode.Solid
			};

			GraphicsDevice.BlendState = new BlendState
			{
				ColorBlendFunction = BlendFunction.Add,
				ColorSourceBlend = Blend.One,
				ColorDestinationBlend = Blend.One,
			};

			GraphicsDevice.DepthStencilState = DepthStencilState.None;

			_pointLightEffect.ModelViewProjection = Matrix.CreateScale(radius) * Matrix.CreateTranslation(position) * _camera.View * _camera.Projection;
			_pointLightEffect.ViewProjectionInverse = Matrix.Invert(_camera.View * _camera.Projection);

			_pointLightEffect.LightPosition = position;
			_pointLightEffect.LightColor = color;
			_pointLightEffect.LightIntensity = intensity;
			_pointLightEffect.LightRange = radius;

			_pointLightEffect.NormalMap = _normalTarget;
			_pointLightEffect.DepthMap = _depthTarget;

			_pointLightEffect.HalfPixel = _halfPixel;

			_pointLightEffect.CurrentTechnique.Passes[0].Apply();

			var part = _sphereModel.Meshes[0].MeshParts[0];
			GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
			GraphicsDevice.Indices = part.IndexBuffer;
			GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
		}

		private void DrawGeometry(float textureFactor)
		{
			GraphicsDevice.RasterizerState = new RasterizerState
			{
				CullMode = CullMode.CullCounterClockwiseFace,
				FillMode = FillMode.Solid
			};

			GraphicsDevice.SamplerStates[0] = _usePointSampling ? SamplerState.PointWrap : SamplerState.AnisotropicWrap;

			_graphicsHelper.UpdateForCamera(_camera);

			foreach (Chunk chunk in _chunkManager.ChunksToRender)
			{
				DrawRegular(chunk, textureFactor);
			}
		}

		private void ClearGBuffer()
		{
			GraphicsDevice.RasterizerState = new RasterizerState
			{
				CullMode = CullMode.None,
				FillMode = FillMode.Solid
			};

			GraphicsDevice.BlendState = BlendState.Opaque;
			GraphicsDevice.DepthStencilState = DepthStencilState.None;

			_clearEffect.CurrentTechnique.Passes[0].Apply();

			_graphicsHelper.DrawQuad(_halfPixel);
		}

		private void DrawRegular(Chunk chunk, float textureFactor)
		{
			GraphicsDevice.BlendState = BlendState.Opaque;
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;

			Render(chunk, textureFactor);
		}

		public void ToggleSsao()
		{
			_ssaoEnabled = !_ssaoEnabled;
		}

		public void ToggleDebug()
		{
			_drawDebug = !_drawDebug;
		}

		public void ToggleNoise()
		{
			_randomize = !_randomize;
		}
	}
}
