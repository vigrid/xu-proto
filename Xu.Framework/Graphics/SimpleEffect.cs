using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Xu.Graphics
{
	public class SimpleEffect : Effect
	{
		public static SimpleEffect Create(ContentManager contentManager)
		{
			var effect = contentManager.Load<Effect>("Core/Effects/Simple");
			return new SimpleEffect(effect);
		}

		private SimpleEffect(Effect cloneSource) : base(cloneSource)
		{
		}

		private bool _initialized;

		public Texture2D Texture { get; set; }

		public Matrix World { get; set; }
		public Matrix View { get; set; }
		public Matrix Projection { get; set; }

		public Vector4 AmbientColor { get; set; }
		public float AmbientIntensity { get; set; }

		public Vector3 DiffuseDirection { get; set; }
		public Vector4 DiffuseColor { get; set; }
		public float DiffuseIntensity { get; set; }

		public Vector4 FogColor { get; set; }
		public float FogDepth { get; set; }

		private EffectParameter _texture;

		private EffectParameter _world;
		private EffectParameter _view;
		private EffectParameter _projection;
		private EffectParameter _worldInverseTranspose;

		private EffectParameter _ambientColor;
		private EffectParameter _ambientIntensity;

		private EffectParameter _diffuseDirection;
		private EffectParameter _diffuseColor;
		private EffectParameter _diffuseIntensity;

		private EffectParameter _fogColor;
		private EffectParameter _fogDepth;

		protected override bool OnApply()
		{
			if (!_initialized)
			{
				Initialize();
			}

			Matrix wt = Matrix.Invert(World);
			Matrix wit = Matrix.Transpose(wt);

			_texture.SetValue(Texture);

			_world.SetValue(World);
			_view.SetValue(View);
			_projection.SetValue(Projection);
			_worldInverseTranspose.SetValue(wit);

			_ambientColor.SetValue(AmbientColor);
			_ambientIntensity.SetValue(AmbientIntensity);

			_diffuseDirection.SetValue(DiffuseDirection);
			_diffuseColor.SetValue(DiffuseColor);
			_diffuseIntensity.SetValue(DiffuseIntensity);

			_fogColor.SetValue(FogColor);
			_fogDepth.SetValue(FogDepth);

			return false;
		}

		private void Initialize()
		{
			_texture = Parameters["Texture"];

			_world = Parameters["World"];
			_view = Parameters["View"];
			_projection = Parameters["Projection"];
			_worldInverseTranspose = Parameters["WorldInverseTranspose"];

			_ambientColor = Parameters["AmbientColor"];
			_ambientIntensity = Parameters["AmbientIntensity"];

			_diffuseDirection = Parameters["DiffuseDirection"];
			_diffuseColor = Parameters["DiffuseColor"];
			_diffuseIntensity = Parameters["DiffuseIntensity"];

			_fogColor = Parameters["FogColor"];
			_fogDepth = Parameters["FogDepth"];

			_initialized = true;
		}
	}

	public class DeferredEffect : Effect
	{
		public static DeferredEffect Create(ContentManager contentManager, int width, int height)
		{
			var effect = contentManager.Load<Effect>("Core/Effects/Simple");
			return new DeferredEffect(effect);
		}

		private DeferredEffect(Effect cloneSource) : base(cloneSource)
		{
		}

		private bool _initialized;

		public Texture2D Texture { get; set; }

		public Matrix World { get; set; }
		public Matrix View { get; set; }
		public Matrix Projection { get; set; }

		public Vector4 AmbientColor { get; set; }
		public float AmbientIntensity { get; set; }

		public Vector3 DiffuseDirection { get; set; }
		public Vector4 DiffuseColor { get; set; }
		public float DiffuseIntensity { get; set; }

		public Vector4 FogColor { get; set; }
		public float FogDepth { get; set; }

		private EffectParameter _texture;

		private EffectParameter _world;
		private EffectParameter _view;
		private EffectParameter _projection;
		private EffectParameter _worldInverseTranspose;

		private EffectParameter _ambientColor;
		private EffectParameter _ambientIntensity;

		private EffectParameter _diffuseDirection;
		private EffectParameter _diffuseColor;
		private EffectParameter _diffuseIntensity;

		private EffectParameter _fogColor;
		private EffectParameter _fogDepth;

		protected override bool OnApply()
		{
			if (!_initialized)
			{
				Initialize();
			}

			Matrix wt = Matrix.Invert(World);
			Matrix wit = Matrix.Transpose(wt);

			_texture.SetValue(Texture);

			_world.SetValue(World);
			_view.SetValue(View);
			_projection.SetValue(Projection);
			_worldInverseTranspose.SetValue(wit);

			_ambientColor.SetValue(AmbientColor);
			_ambientIntensity.SetValue(AmbientIntensity);

			_diffuseDirection.SetValue(DiffuseDirection);
			_diffuseColor.SetValue(DiffuseColor);
			_diffuseIntensity.SetValue(DiffuseIntensity);

			_fogColor.SetValue(FogColor);
			_fogDepth.SetValue(FogDepth);

			return false;
		}

		private void Initialize()
		{
			_texture = Parameters["Texture"];

			_world = Parameters["World"];
			_view = Parameters["View"];
			_projection = Parameters["Projection"];
			_worldInverseTranspose = Parameters["WorldInverseTranspose"];

			_ambientColor = Parameters["AmbientColor"];
			_ambientIntensity = Parameters["AmbientIntensity"];

			_diffuseDirection = Parameters["DiffuseDirection"];
			_diffuseColor = Parameters["DiffuseColor"];
			_diffuseIntensity = Parameters["DiffuseIntensity"];

			_fogColor = Parameters["FogColor"];
			_fogDepth = Parameters["FogDepth"];

			_initialized = true;
		}
	}
}
