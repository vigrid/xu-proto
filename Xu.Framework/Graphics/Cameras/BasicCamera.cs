using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xu.Graphics.Cameras
{
	public class BasicCamera : ICamera
	{
		// TODO: Add initial target / location

		private float _fieldOfView;
		private float _aspectRatio;
		private float _nearPlaneDistance;
		private float _farPlaneDistance;

		private Vector3 _position;
		private Vector3 _direction;
		private Quaternion _rotation;

		private float _yaw;
		private float _pitch;
		private float _roll;

		private Matrix _view;
		private Matrix _projection;
		private BoundingFrustum _frustum;


		public BasicCamera(Vector3 position, float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
		{
			Contract.Requires(fieldOfView > 0.0f);
			Contract.Requires(fieldOfView < MathHelper.PiOver2);
			Contract.Requires(aspectRatio > 0.0f);
			Contract.Requires(nearPlaneDistance > 0.0f);
			Contract.Requires(farPlaneDistance > nearPlaneDistance);
			Contract.EndContractBlock();

			_position = position;
			_fieldOfView = fieldOfView;
			_aspectRatio = aspectRatio;
			_nearPlaneDistance = nearPlaneDistance;
			_farPlaneDistance = farPlaneDistance;

			UpdateProperties();
		}

		public float FieldOfView
		{
			get { return _fieldOfView; }
			set
			{
				Contract.Requires(value > 0.0f);
				Contract.Requires(value < MathHelper.PiOver2);
				Contract.EndContractBlock();

				_fieldOfView = value;
			}
		}

		public float AspectRatio
		{
			get { return _aspectRatio; }
			set
			{
				Contract.Requires(value > 0.0f);
				Contract.EndContractBlock();

				_aspectRatio = value;
			}
		}

		public float NearPlaneDistance
		{
			get { return _nearPlaneDistance; }
			set
			{
				Contract.Requires(value > 0.0f);
				Contract.Requires(value < FarPlaneDistance);
				Contract.EndContractBlock();

				_nearPlaneDistance = value;
			}
		}

		public float FarPlaneDistance
		{
			get { return _farPlaneDistance; }
			set
			{
				Contract.Requires(value > NearPlaneDistance);
				Contract.EndContractBlock();

				_farPlaneDistance = value;
			}
		}

		public Quaternion Rotation
		{
			get { return _rotation; }
		}

		#region ICamera Members

		public Matrix View
		{
			get { return _view; }
		}

		public Matrix Projection
		{
			get { return _projection; }
		}

		public BoundingFrustum Frustum
		{
			get { return _frustum; }
		}

		public Vector3 Position
		{
			get { return _position; }
			set { _position = value; }
		}

		public Vector3 Direction
		{
			get { return _direction; }
		}

		public Ray Pick(Vector2 nearPlanePosition, Viewport viewport)
		{
			var near = viewport.Unproject(new Vector3(nearPlanePosition, 0.0f), Projection, View, Matrix.Identity);
			var far = viewport.Unproject(new Vector3(nearPlanePosition, 1.0f), Projection, View, Matrix.Identity);
			return new Ray(Position, Vector3.Normalize(far - near));
		}

		#endregion

		public void Rotate(float yaw, float pitch, float roll)
		{
			// TODO: Crop these angles to [-Pi,+Pi]
			_yaw += yaw;
			_pitch += pitch;

			const float oneMinusEpsilon = 0.999999f;

			if (_pitch > MathHelper.PiOver2 * oneMinusEpsilon)
			{
				_pitch = MathHelper.PiOver2 * oneMinusEpsilon;
			}
			else if (_pitch < -MathHelper.PiOver2 * oneMinusEpsilon)
			{
				_pitch = -MathHelper.PiOver2 * oneMinusEpsilon;
			}

			_roll += roll;

			Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, _roll, out _rotation);

			UpdateProperties();
		}

		public virtual void Translate(Vector3 distance)
		{
			_position += Vector3.Transform(distance, _rotation);

			UpdateProperties();
		}

		private void UpdateProperties()
		{
			_view = Matrix.Invert(Matrix.CreateFromQuaternion(_rotation) * Matrix.CreateTranslation(_position));
			_projection = Matrix.CreatePerspectiveFieldOfView(_fieldOfView, _aspectRatio, _nearPlaneDistance, _farPlaneDistance);
			_direction = Vector3.Transform(Vector3.Forward, _rotation);
			_frustum = new BoundingFrustum(_view * _projection);
		}
	}
}
