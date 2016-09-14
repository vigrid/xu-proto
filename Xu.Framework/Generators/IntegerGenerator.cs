using System.Diagnostics.Contracts;

namespace Xu.Generators
{
	public static class IntegerGenerator
	{
		public static int[] GenerateSequence(int start, int end)
		{
			Contract.Requires(start < end);
			Contract.EndContractBlock();

			int length = end - start;
			var result = new int[length];
			for (int i = 0; i < length; i++)
			{
				result[i] = start++;
			}

			return result;
		}

		public static byte[] GenerateSequence(byte start, byte end)
		{
			Contract.Requires(start < end);
			Contract.EndContractBlock();

			int length = end - start;
			var result = new byte[length];
			for (int i = 0; i < length; i++)
			{
				result[i] = start++;
			}

			return result;
		}
	}
}
