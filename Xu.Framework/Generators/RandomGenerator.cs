using System;
using System.Diagnostics.Contracts;

namespace Xu.Generators
{
	[Serializable]
	public abstract class RandomGenerator
	{
		protected readonly Random Random;

		protected RandomGenerator()
		{
			Random = new Random();
		}

		protected RandomGenerator(int seed)
		{
			Random = new Random(seed);
		}

		protected RandomGenerator(Random random)
		{
			Contract.Requires(random != null);
			Contract.EndContractBlock();

			Random = random;
		}
	}
}
