using Microsoft.Xna.Framework.Input;

namespace Xu.Input
{
	public class KeyEventArgs
	{
		public Keys Key { get; private set; }
		public KeyType KeyType { get; private set; }
		public bool ControlPressed { get; private set; }
		public bool ShiftPressed { get; private set; }
		public bool AltPressed { get; private set; }

		public KeyEventArgs(Keys key, KeyType keyType, bool control, bool shift, bool alt)
		{
			Key = key;
			KeyType = keyType;
			ControlPressed = control;
			ShiftPressed = shift;
			AltPressed = alt;
		}
	}
}