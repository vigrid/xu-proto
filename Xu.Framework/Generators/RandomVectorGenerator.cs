using System;
using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;

namespace Xu.Generators
{
	[Serializable]
	public sealed class RandomVectorGenerator : RandomGenerator
	{
		public RandomVectorGenerator()
		{
		}

		public RandomVectorGenerator(int seed) : base(seed)
		{
		}

		public RandomVectorGenerator(Random random) : base(random)
		{
			Contract.Requires(random != null);
			Contract.EndContractBlock();
		}

		public Vector2[] GenerateVector2(int count)
		{
			Contract.Requires(count > 0);
			Contract.EndContractBlock();

			var result = new Vector2[count];
			FillVector2(result, 0, count);
			return result;
		}

		public void FillVector2(Vector2[] destination)
		{
			Contract.Requires(destination != null);
			Contract.EndContractBlock();

			FillVector2(destination, 0, destination.Length);
		}

		public void FillVector2(Vector2[] destination, int startIndex, int count)
		{
			Contract.Requires(destination != null);
			Contract.Requires(startIndex >= 0);
			Contract.Requires(count > 0);
			Contract.Requires(startIndex + count <= destination.Length);
			Contract.EndContractBlock();

			count += startIndex;

			while (--count >= startIndex)
			{
				destination[count] = GetVector2();
			}
		}

		public Vector2 GetVector2()
		{
			Vector2 result;
			double angle = Random.NextDouble() * MathHelper.TwoPi;
			result.X = (float) Math.Cos(angle);
			result.Y = (float) Math.Sin(angle);
			return result;
		}

		public Vector3[] GenerateVector3(int count)
		{
			Contract.Requires(count > 0);
			Contract.EndContractBlock();

			var result = new Vector3[count];
			FillVector3(result, 0, count);
			return result;
		}

		public void FillVector3(Vector3[] destination)
		{
			Contract.Requires(destination != null);
			Contract.EndContractBlock();

			FillVector3(destination, 0, destination.Length);
		}

		public void FillVector3(Vector3[] destination, int startIndex, int count)
		{
			Contract.Requires(destination != null);
			Contract.Requires(startIndex >= 0);
			Contract.Requires(count > 0);
			Contract.Requires(startIndex + count <= destination.Length);
			Contract.EndContractBlock();

			count += startIndex;

			while (--count >= startIndex)
			{
				destination[count] = GetVector3();
			}
		}

		public Vector3 GetVector3()
		{
			Vector3 result;
			double z = Random.NextDouble() * 2.0 - 1.0;
			double r = Math.Sqrt(1.0 - z * z);
			double angle = Random.NextDouble() * MathHelper.TwoPi;
			result.X = (float) (r * Math.Cos(angle));
			result.Y = (float) (r * Math.Sin(angle));
			result.Z = (float) z;
			return result;
		}
	}
}
