using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Xu.Core;

namespace Xu.Types
{
	[DebuggerDisplay("IntVector3 ({X}, {Y}, {Z})")]
	[StructLayout(LayoutKind.Sequential)]
	public struct IntVector3
	{
		private static readonly IntVector3 _zero = new IntVector3(0, 0, 0);
		private static readonly IntVector3 _one = new IntVector3(1, 1, 1);
		private static readonly IntVector3 _left = new IntVector3(-1, 0, 0);
		private static readonly IntVector3 _right = new IntVector3(1, 0, 0);
		private static readonly IntVector3 _down = new IntVector3(0, -1, 0);
		private static readonly IntVector3 _up = new IntVector3(0, 1, 0);
		private static readonly IntVector3 _forward = new IntVector3(0, 0, -1);
		private static readonly IntVector3 _backward = new IntVector3(0, 0, 1);
		private static readonly IntVector3 _minValue = new IntVector3(Int32.MinValue, Int32.MinValue, Int32.MinValue);
		private static readonly IntVector3 _maxValue = new IntVector3(Int32.MaxValue, Int32.MaxValue, Int32.MaxValue);

		public int X;
		public int Y;
		public int Z;

		public IntVector3(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public IntVector3(Vector3 vector)
		{
			X = XuMath.FloorToInt(vector.X);
			Y = XuMath.FloorToInt(vector.Y);
			Z = XuMath.FloorToInt(vector.Z);
		}

		public static explicit operator IntVector3(Vector3 vector)
		{
			return new IntVector3(vector);
		}

		public static explicit operator Vector3(IntVector3 vector)
		{
			return new Vector3(vector.X, vector.Y, vector.Z);
		}

		public static IntVector3 operator -(IntVector3 value)
		{
			IntVector3 result;
			result.X = -value.X;
			result.Y = -value.Y;
			result.Z = -value.Z;
			return result;
		}

		public int LengthSquared
		{
			get { return X * X + Y * Y + Z * Z; }
		}

		public static bool operator ==(IntVector3 a, IntVector3 b)
		{
			return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
		}

		public static bool operator !=(IntVector3 a, IntVector3 b)
		{
			return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
		}

		public static IntVector3 operator +(IntVector3 value1, IntVector3 value2)
		{
			IntVector3 vector3;
			vector3.X = value1.X + value2.X;
			vector3.Y = value1.Y + value2.Y;
			vector3.Z = value1.Z + value2.Z;
			return vector3;
		}

		public static IntVector3 operator -(IntVector3 value1, IntVector3 value2)
		{
			IntVector3 vector3;
			vector3.X = value1.X - value2.X;
			vector3.Y = value1.Y - value2.Y;
			vector3.Z = value1.Z - value2.Z;
			return vector3;
		}

		public static IntVector3 operator *(IntVector3 value1, IntVector3 value2)
		{
			IntVector3 vector3;
			vector3.X = value1.X * value2.X;
			vector3.Y = value1.Y * value2.Y;
			vector3.Z = value1.Z * value2.Z;
			return vector3;
		}

		public static IntVector3 Zero
		{
			get { return _zero; }
		}

		public static IntVector3 One
		{
			get { return _one; }
		}

		public static IntVector3 Left
		{
			get { return _left; }
		}

		public static IntVector3 Right
		{
			get { return _right; }
		}

		public static IntVector3 Down
		{
			get { return _down; }
		}

		public static IntVector3 Up
		{
			get { return _up; }
		}

		public static IntVector3 Forward
		{
			get { return _forward; }
		}

		public static IntVector3 Backward
		{
			get { return _backward; }
		}

		public static IntVector3 MinValue
		{
			get { return _minValue; }
		}

		public static IntVector3 MaxValue
		{
			get { return _maxValue; }
		}

		public bool Equals(IntVector3 other)
		{
			return other.X == X && other.Y == Y && other.Z == Z;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (obj.GetType() != typeof (IntVector3))
			{
				return false;
			}
			return Equals((IntVector3) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = X;
				result = (result * 397) ^ Y;
				result = (result * 397) ^ Z;
				return result;
			}
		}
	}
}
