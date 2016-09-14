using System;

namespace Xu.Input
{
	[Flags]
	public enum KeyType
	{
		Unknown = 0x00000000,
		Printable = 0x00000001,
		Control = 0x00000002,
		Modifier = 0x00000004,
	}
}