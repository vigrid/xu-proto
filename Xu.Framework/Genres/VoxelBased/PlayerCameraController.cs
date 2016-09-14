using System;
using Microsoft.Xna.Framework;
using Xu.Graphics.Cameras;
using Xu.Input;
using Xu.Types;

namespace Xu.Genres.VoxelBased
{
	public class PlayerCameraController : BasicCameraController
	{
		private readonly ChunkManager _chunkManager;
		private const float _gravity = 15.0f;
		private Vector3 _velocity;

		public PlayerCameraController(Game game, BasicCamera camera, ChunkManager chunkManager) : base(game, camera)
		{
			_chunkManager = chunkManager;
		}

		private Vector3 _movement;
		private bool _onGround;

		private Vector3[] _colliders = new[] {Vector3.Left, Vector3.Right, Vector3.Forward, Vector3.Backward};

		public override void Update(GameTime gameTime)
		{
			float time = (float) gameTime.ElapsedGameTime.TotalSeconds;
			if (_movement.Length() > 0.0f)
			{
				_movement.Normalize();
			}
			_velocity += time * _movement * MovementSpeed * 10.0f;
			if (!_onGround)
			{
				_velocity.Y -= _gravity*time;
			}

			float damp = (float) Math.Pow(0.1f, time * 10.0f);
			_velocity.X *= damp;
			_velocity.Z *= damp;

			_camera.Position += _velocity * time;

			IntVector3 cameraBlock = new IntVector3(_camera.Position);
			var cameraPosition = _camera.Position;

			Block block;
			if (_chunkManager.TryGetBlock(new IntVector3(_camera.Position + Vector3.Down * 1.5f), out block))
			{
				if (block.IsCollidable)
				{
					_velocity.Y = 0.0f;
					cameraPosition.Y = cameraBlock.Y + 0.5f;
					_onGround = true;
				}
			}
			if (_chunkManager.TryGetBlock(new IntVector3(_camera.Position + Vector3.Up * 0.15f), out block))
			{
				if (block.IsCollidable)
				{
					_velocity.Y = 0.0f;
					cameraPosition.Y = cameraBlock.Y + 0.65f;
					_onGround = true;
				}
			}
			if (_chunkManager.TryGetBlock(new IntVector3(_camera.Position + Vector3.Down * 1.55f), out block))
			{
				if (!block.IsCollidable)
				{
					_onGround = false;
				}
			}

			_camera.Position = cameraPosition;

			for (int i = 0; i < _colliders.Length; i++)
			{
				var size = 0.3f;
				var offset = _colliders[i] * size;
				Vector3 newPosition = _camera.Position;

				if (_chunkManager.TryGetBlock(new IntVector3(_camera.Position + Vector3.Down * 1.25f + offset), out block))
				{
					if (block.IsCollidable)
					{
						switch (i)
						{
							case 0:
								newPosition.X = cameraBlock.X + size;
								break;
							case 1:
								newPosition.X = cameraBlock.X + 1 - size;
								break;
							case 2:
								newPosition.Z = cameraBlock.Z + size;
								break;
							case 3:
								newPosition.Z = cameraBlock.Z + 1 - size;
								break;
						}
					}
				}

				if (_chunkManager.TryGetBlock(new IntVector3(_camera.Position + offset), out block))
				{
					if (block.IsCollidable)
					{
						switch (i)
						{
							case 0:
								newPosition.X = cameraBlock.X + size;
								break;
							case 1:
								newPosition.X = cameraBlock.X + 1 - size;
								break;
							case 2:
								newPosition.Z = cameraBlock.Z + size;
								break;
							case 3:
								newPosition.Z = cameraBlock.Z + 1 - size;
								break;
						}
						_camera.Position = newPosition;
					}
				}

				_camera.Position = newPosition;
			}

			var mx = InvertX ? 1.0f : -1.0f;
			var my = InvertY ? 1.0f : -1.0f;
			var mz = InvertZ ? 1.0f : -1.0f;

			// _camera.Translate(_accumulatedTranslation * (float) gameTime.ElapsedGameTime.TotalSeconds * _accumulatedSpeedMultiplier * MovementSpeed);
			_camera.Rotate(mx * _accumulatedRotation.X * RotationSpeed, my * _accumulatedRotation.Y * RotationSpeed, mz * _accumulatedRotation.Z * RotationSpeed);

			Reset();

			_movement = Vector3.Zero;

			base.Update(gameTime);
		}

		public void MoveCharacter(Vector3 direction)
		{
			var horizontalDirection = _camera.Direction;
			horizontalDirection.Y = 0.0f;
			horizontalDirection.Normalize();

			if (direction == Vector3.Forward)
			{
				
			}
			else if (direction == Vector3.Backward)
			{
				horizontalDirection *= -1.0f;
			}
			else if (direction == Vector3.Right)
			{
				horizontalDirection = new Vector3(-horizontalDirection.Z, 0.0f, horizontalDirection.X);
			}
			else if (direction == Vector3.Left)
			{
				horizontalDirection = new Vector3(horizontalDirection.Z, 0.0f, -horizontalDirection.X);
			}

			_movement += horizontalDirection;
		}

		public void JumpCharacter()
		{
			if (_onGround)
			{
				_velocity.Y += 6.0f;
				_onGround = false;
			}
		}
	}
}
