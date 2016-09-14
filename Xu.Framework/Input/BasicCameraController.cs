using Microsoft.Xna.Framework;
using Xu.Graphics.Cameras;

namespace Xu.Input
{
	public class BasicCameraController : GameComponent, ICameraController
	{
		protected readonly BasicCamera _camera;

		protected Vector3 _accumulatedTranslation;
		protected Vector3 _accumulatedRotation;
		protected float _accumulatedSpeedMultiplier;

		public BasicCameraController(Game game, BasicCamera camera) : base(game)
		{
			_camera = camera;
		}

		public float MovementSpeed { get; set; }
		public float RotationSpeed { get; set; }

		public bool InvertX { get; set; }
		public bool InvertY { get; set; }
		public bool InvertZ { get; set; }

		#region ICameraController Members

		public virtual void Translate(Vector3 deltaTranslation)
		{
			_accumulatedTranslation += deltaTranslation;
		}

		public virtual void Translate(float deltaX, float deltaY, float deltaZ)
		{
			_accumulatedTranslation.X += deltaX;
			_accumulatedTranslation.Y += deltaY;
			_accumulatedTranslation.Z += deltaZ;
		}

		public virtual void Rotate(Vector3 deltaRotation)
		{
			_accumulatedRotation += deltaRotation;
		}

		public virtual void Rotate(float deltaYaw, float deltaPitch, float deltaRoll)
		{
			_accumulatedRotation.X += deltaYaw;
			_accumulatedRotation.Y += deltaPitch;
			_accumulatedRotation.Z += deltaRoll;
		}

		public virtual void Accelerate(float multiplier)
		{
			_accumulatedSpeedMultiplier *= multiplier;
		}

		#endregion

		protected void Reset()
		{
			_accumulatedTranslation = Vector3.Zero;
			_accumulatedRotation = Vector3.Zero;
			_accumulatedSpeedMultiplier = 1.0f;
		}

		public override void Initialize()
		{
			Reset();

			base.Initialize();
		}

		public override void Update(GameTime gameTime)
		{
			var mx = InvertX ? 1.0f : -1.0f;
			var my = InvertY ? 1.0f : -1.0f;
			var mz = InvertZ ? 1.0f : -1.0f;

			_camera.Translate(_accumulatedTranslation * (float) gameTime.ElapsedGameTime.TotalSeconds * _accumulatedSpeedMultiplier * MovementSpeed);
			_camera.Rotate(mx * _accumulatedRotation.X * RotationSpeed, my * _accumulatedRotation.Y * RotationSpeed, mz * _accumulatedRotation.Z * RotationSpeed);

			Reset();

			base.Update(gameTime);
		}
	}
}
