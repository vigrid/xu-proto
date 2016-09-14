using System;
using Microsoft.Xna.Framework.Input;

namespace Xu.Input
{
	public class KeyboardBufferItem
	{
		[Flags]
		public enum Modifiers
		{
			Shift = 0x01,
			Control = 0x02,
			Alt = 0x04,
		}

		public readonly char? Character;
		public readonly Keys? VirtualKey;
		public readonly Modifiers ActiveModifiers;

		public KeyboardBufferItem(char character, Modifiers activeModifiers)
		{
			Character = character;
			VirtualKey = null;
			ActiveModifiers = activeModifiers;
		}

		public KeyboardBufferItem(Keys virtualKey, Modifiers activeModifiers)
		{
			Character = null;
			VirtualKey = virtualKey;
			ActiveModifiers = activeModifiers;
		}
	}
}