using System;
using Microsoft.Xna.Framework.Input;

namespace Xu.Input
{
	public interface IKeyboardManager
	{
		KeyboardManagerMode Mode { get; set; }

		IKeyConsumer BufferedKeyConsumer { get; set; }

		void BindHold(Keys key, Action action);
		void BindPress(Keys key, Action action);
		void UnbindHold(Keys key);
		void UnbindPress(Keys key);
	}
}
