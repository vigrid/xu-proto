using System;
using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;
using Xu.Core;
using Xu.Types;

namespace Xu.Genres.VoxelBased
{
	public class VoxelTraverser
	{
		private Ray _ray;

		private readonly float _maxDistance;
		private float _distanceTraversed;

		private IntVector3 _currentVoxel;
		private IntVector3 _step;
		private IntVector3 _offset;

		public VoxelTraverser(Ray ray, float maxDistance)
		{
			Contract.Requires(maxDistance > 0.0f);
			Contract.EndContractBlock();

			_ray = ray;
			_maxDistance = maxDistance;
		}

		public bool Initialize(out IntVector3 voxel)
		{
			_currentVoxel = new IntVector3(_ray.Position);
			_step = new IntVector3(Math.Sign(_ray.Direction.X), Math.Sign(_ray.Direction.Y), Math.Sign(_ray.Direction.Z));
			_offset = new IntVector3(_step.X > 0 ? 1 : 0, _step.Y > 0 ? 1 : 0, _step.Z > 0 ? 1 : 0);

			voxel = _currentVoxel;
			return true;
		}

		public bool Traverse(out IntVector3 voxel)
		{
			float tx = _step.X != 0 ? (_currentVoxel.X + _offset.X - _ray.Position.X) / _ray.Direction.X : float.MaxValue;
			float ty = _step.Y != 0 ? (_currentVoxel.Y + _offset.Y - _ray.Position.Y) / _ray.Direction.Y : float.MaxValue;
			float tz = _step.Z != 0 ? (_currentVoxel.Z + _offset.Z - _ray.Position.Z) / _ray.Direction.Z : float.MaxValue;

			float t = XuMath.Min(tx, ty, tz);

			if (t.Equals(tx))
			{
				_currentVoxel.X += _step.X;
			}
			else if (t.Equals(ty))
			{
				_currentVoxel.Y += _step.Y;
			}
			else
			{
				_currentVoxel.Z += _step.Z;
			}

			_ray.Position += t * _ray.Direction;
			_distanceTraversed += t;

			voxel = _currentVoxel;
			return _distanceTraversed < _maxDistance;
		}
	}
}
