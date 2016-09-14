using System;
using System.Diagnostics.Contracts;

namespace Xu.Extensions
{
	public static class Int32Extensions
	{
		public static UInt32 UnpackBits(this UInt32 number, int startBit, int endBit)
		{
			Contract.Requires(number >= 0);
			Contract.Requires(number < 32);
			Contract.Requires(startBit >= 0);
			Contract.Requires(startBit < 32);
			Contract.Requires(endBit >= startBit);
			Contract.Requires(endBit < 32);
			Contract.EndContractBlock();

			return (number >> startBit) & (UInt32.MaxValue << (endBit - startBit));
		}
	}
}