using System;
using System.Diagnostics.Contracts;

namespace Xu.Extensions
{
	public static class ArrayExtensions
	{
		public static void Shuffle<T>(this T[] array, Random random)
		{
			Contract.Requires(array != null);
			Contract.Requires(random != null);
			Contract.EndContractBlock();

			int length = array.Length;

			for (int i = 0; i < length; i++)
			{
				int j = random.Next(length);
				T t = array[i];
				array[i] = array[j];
				array[j] = t;
			}
		}
	}
}
