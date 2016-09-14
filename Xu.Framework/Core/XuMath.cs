using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;

namespace Xu.Core
{
	public static class XuMath
	{
		public static int FloorToInt(float f)
		{
			Contract.Requires(!float.IsNaN(f));
			Contract.EndContractBlock();

			return f >= 0.0f ? (int) f : (int) (f - 1.0f);
		}

		public static int Clamp(int value, int min, int max)
		{
			Contract.Requires(min <= max);
			Contract.EndContractBlock();

			return min > value ? min : (max < value ? max : value);
		}

		public static float Clamp(float value, float min, float max)
		{
			Contract.Requires(!float.IsNaN(value));
			Contract.Requires(!float.IsNaN(min));
			Contract.Requires(!float.IsNaN(max));
			Contract.Requires(min <= max);
			Contract.EndContractBlock();

			return min > value ? min : (max < value ? max : value);
		}

		public static float InterpolateLinear(float a, float b, float t)
		{
			Contract.Requires(!float.IsNaN(a));
			Contract.Requires(!float.IsNaN(b));
			Contract.Requires(t >= 0.0f);
			Contract.Requires(t <= 1.0f);
			Contract.EndContractBlock();

			return a + (b - a) * t;
		}

		public static float InterpolateHermite(float a, float b, float t)
		{
			Contract.Requires(!float.IsNaN(a));
			Contract.Requires(!float.IsNaN(b));
			Contract.Requires(t >= 0.0f);
			Contract.Requires(t <= 1.0f);
			Contract.EndContractBlock();

			return InterpolateLinear(a, b, t * t * (3.0f - 2.0f * t));
		}

		public static float InterpolatePolynomial(float a, float b, float t)
		{
			Contract.Requires(!float.IsNaN(a));
			Contract.Requires(!float.IsNaN(b));
			Contract.Requires(t >= 0.0f);
			Contract.Requires(t <= 1.0f);
			Contract.EndContractBlock();

			return InterpolateLinear(a, b, t * t * t * (6.0f * t * t - 15.0f * t + 10.0f));
		}

		public static float DotProduct(ref Vector2 vector, float x, float y)
		{
			Contract.Requires(vector != null);
			Contract.Requires(!float.IsNaN(x));
			Contract.Requires(!float.IsNaN(y));
			Contract.EndContractBlock();

			return vector.X * x + vector.Y * y;
		}

		public static float DotProduct(ref Vector3 vector, float x, float y, float z)
		{
			Contract.Requires(vector != null);
			Contract.Requires(!float.IsNaN(x));
			Contract.Requires(!float.IsNaN(y));
			Contract.Requires(!float.IsNaN(z));
			Contract.EndContractBlock();

			return vector.X * x + vector.Y * y + vector.Z * z;
		}

		public static float DotProduct(ref Vector4 vector, float x, float y, float z, float w)
		{
			Contract.Requires(vector != null);
			Contract.Requires(!float.IsNaN(x));
			Contract.Requires(!float.IsNaN(y));
			Contract.Requires(!float.IsNaN(z));
			Contract.Requires(!float.IsNaN(w));
			Contract.EndContractBlock();

			return vector.X * x + vector.Y * y + vector.Z * z + vector.W * w;
		}

		public static int Modulo(int number, int modulo)
		{
			Contract.Requires(modulo > 0);
			Contract.EndContractBlock();

			int result = number % modulo;
			return result >= 0 ? result : result + modulo;
		}

		public static float Min(float a, float b)
		{
			Contract.Requires(!float.IsNaN(a));
			Contract.Requires(!float.IsNaN(b));
			Contract.EndContractBlock();

			return a < b ? a : b;
		}

		public static float Min(float a, float b, float c)
		{
			Contract.Requires(!float.IsNaN(a));
			Contract.Requires(!float.IsNaN(b));
			Contract.Requires(!float.IsNaN(c));
			Contract.EndContractBlock();

			return a < b ? (a < c ? a : c) : (b < c ? b : c);
		}

		public static float Max(float a, float b)
		{
			Contract.Requires(!float.IsNaN(a));
			Contract.Requires(!float.IsNaN(b));
			Contract.EndContractBlock();

			return a > b ? a : b;
		}

		public static float Max(float a, float b, float c)
		{
			Contract.Requires(!float.IsNaN(a));
			Contract.Requires(!float.IsNaN(b));
			Contract.Requires(!float.IsNaN(c));
			Contract.EndContractBlock();

			return a > b ? (a > c ? a : c) : (b > c ? b : c);
		}
	}
}
